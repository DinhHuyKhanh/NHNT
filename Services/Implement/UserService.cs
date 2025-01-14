using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using NHNT.Constants;
using NHNT.Constants.Statuses;
using NHNT.Dtos;
using NHNT.Exceptions;
using NHNT.Models;
using NHNT.Repositories;
using NHNT.Utils;

namespace NHNT.Services.Implement
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public UserService(IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public UserDto GetUserById(int id)
        {
            User user = _userRepository.GetById(id);
            if (user == null)
            {
                throw new DataRuntimeException(StatusNotExist.UserId);
            }

            return new UserDto(user);
        }

        public UserDto GetUserByUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new DataRuntimeException(StatusWrongFormat.USERNAME_IS_EMPTY);
            }

            User user = _userRepository.GetByUsername(username);
            if (user == null)
            {
                throw new DataRuntimeException(StatusNotExist.UserId);
            }

            return new UserDto(user);
        }

        public TokenDto Login(string username, string password)
        {
            User user = _userRepository.GetByUsernameAndPassowrd(username, password);
            if (user == null)
            {
                throw new DataRuntimeException(StatusWrongFormat.USERNAME_OR_PASSWORD_WRONG_FROMAT);
            }

            JwtSecurityToken jwtSecurityToken = TokenUtils.GetJwtTokenHandler(user);

            string refreshTokenValue = TokenUtils.GenerateRefreshToken();

            RefreshToken refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                Value = refreshTokenValue,
                JwtId = jwtSecurityToken.Id,
                IsUsed = false,
                IsRevoked = false,
                IssuedAt = DateTime.UtcNow,
                ExpiredAt = DateTime.UtcNow.AddMilliseconds(AppSettingConfig.RefreshTokenExpiredTime)
            };

            _refreshTokenRepository.Add(refreshTokenEntity);

            return new TokenDto
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                ExpiredTime = jwtSecurityToken.ValidTo,
                RefreshToken = refreshTokenValue
            };
        }

        public TokenDto RefreshToken(string accessTokenOld, string refreshToken)
        {
            if (string.IsNullOrEmpty(accessTokenOld))
            {
                throw new DataRuntimeException(StatusWrongFormat.ACCESS_TOKEN_EMPTY);
            }

            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new DataRuntimeException(StatusWrongFormat.REFRESH_TOKEN_EMPTY);
            }

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            // kiểm tra xem token đúng định dạng và lấy ra các claimsPrincipal
            ClaimsPrincipal claimsPrincipal = jwtSecurityTokenHandler.ValidateToken(accessTokenOld, TokenUtils.GetTokenValidationParameters(), out securityToken);

            // kiểm tra thuật toán
            bool validAlgorithm = ((JwtSecurityToken)securityToken).Header.Alg.Equals(AppSettingConfig.Algorithms, StringComparison.InvariantCultureIgnoreCase);
            if (!validAlgorithm)
            {
                throw new DataRuntimeException(StatusServer.TOKEN_INVALID);
            }

            // kiểm tra thời gian hết hạn của token

            // kiểm tra thông tin refresh token
            RefreshToken refreshTokenDb = _refreshTokenRepository.GetByRefreshTokenValue(refreshToken);
            if (refreshTokenDb == null)
            {
                throw new DataRuntimeException(StatusNotExist.REFRESH_TOKEN);
            }

            if (refreshTokenDb.IsRevoked ?? true)
            {
                throw new DataRuntimeException(StatusWrongFormat.REFRESH_TOKEN_IS_REVOKED);
            }

            // kiểm tra AccessToken id == JwtId in RefreshToken
            string accessTokenId = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
            if (accessTokenId != refreshTokenDb.JwtId)
            {
                throw new DataRuntimeException(StatusWrongFormat.ACCESS_TOKEN_ID_NOT_MATCH_JTI);
            }

            // tạo acceess token mới 
            User user = _userRepository.GetById(refreshTokenDb.UserId);
            JwtSecurityToken jwtSecurityToken = TokenUtils.GetJwtTokenHandler(user);
            TokenDto tokenNew = new TokenDto
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                ExpiredTime = jwtSecurityToken.ValidTo,
                RefreshToken = refreshToken,
            };

            // cập nhập lại thông tin refresh token trong database
            refreshTokenDb.IsUsed = true;
            refreshTokenDb.JwtId = jwtSecurityToken.Id;
            //_refreshTokenRepository.Update(refreshTokenDb);

            return tokenNew;
        }
    }
}