using Microsoft.AspNetCore.Http;
using NHNT.Dtos;
using NHNT.Models;

namespace NHNT.Services
{
    public interface IImageService
    {
        public void SaveImage(IFormFile images, int departmentId);
    }
}