using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
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
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IImageService _imageService;

        public DepartmentService(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }


        public DepartmentDto[] List(int page, int limit)
        {
            var departments = _departmentRepository.List(page, limit);
            DepartmentDto[] result = { };

            return result;
        }

        public DepartmentDto register(DepartmentDto department, List<IFormFile> images)
        {
            department 
            foreach (var image in images)
            {
                // Lưu trữ hình ảnh và cập nhật đường dẫn trong model
                _imageService.SaveImage(image, department.Id);
            }
            return department;
        }
    }
}