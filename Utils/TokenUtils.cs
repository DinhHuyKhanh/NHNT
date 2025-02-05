using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using NHNT.Constants;
using NHNT.Constants.Statuses;
using NHNT.Exceptions;
using NHNT.Models;

namespace NHNT.Utils
{
    public static class TokenUtils
    {
        private static TokenValidationParameters TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = AppSettingConfig.Issuer,
            ValidateAudience = true,
            ValidAudience = AppSettingConfig.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(AppSettingConfig.SecretByte),
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = false
        };

        public static JwtSecurityToken GetJwtTokenHandler(User user)
        {
            if (user == null)
                throw new DataRuntimeException(StatusServer.USER_INFO_HAS_ERROR);

            var claims = new List<Claim>()
            {
                new Claim("id", user.Id.ToString()),
                new Claim("username", user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // giúp đạnh danh token, làm cho token là duy nhất, có thể dựa vào đây để thu hồi token
            };

            ICollection<UserRole> userRoles = user.UserRoles;
            if (userRoles == null || !userRoles.Any())
                throw new DataRuntimeException(StatusServer.USER_ROLES_HAS_ERROR);

            foreach (var userRole in user.UserRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
            }

            var key = new SymmetricSecurityKey(AppSettingConfig.SecretByte);
            var creds = new SigningCredentials(key, AppSettingConfig.Algorithms);

            return new JwtSecurityToken(
                issuer: AppSettingConfig.Issuer,
                audience: AppSettingConfig.Audience,
                expires: DateTime.UtcNow.AddMilliseconds(AppSettingConfig.TokenExpiredTime),
                claims: claims,
                signingCredentials: creds
            );
        }

        public static string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }

        public static TokenValidationParameters GetTokenValidationParameters()
        {
            return TokenValidationParameters;
        }
    }
}