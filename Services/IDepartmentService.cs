
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using NHNT.Dtos;

namespace NHNT.Services
{
    public interface IDepartmentService
    {
        DepartmentDto[] List(int page, int limit);

        DepartmentDto register(DepartmentDto department, List<IFormFile> images);
    }
}