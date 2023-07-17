
using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using NHNT.Dtos;
using NHNT.Models;
using NHNT.Repositories;
using NHNT.Repositories.Implement;

namespace NHNT.Services.Implement
{
    public class ImageService : IImageService
    {

        private readonly IImageRepository _imageRepository;
        public ImageService()
        {
        }

        public void SaveImage(IFormFile images, int departmentId)
        {
            string relativePath = "static/images/";
            // Lưu trữ hình ảnh và trả về đường dẫn lưu trữ
            if (images != null && images.Length > 0)
            {
                string uploadPath = Path.GetFullPath(relativePath);

                string fileName = DateTime.Now.Ticks.ToString() + "_" + images.FileName;

                string filePath = Path.Combine(uploadPath, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    images.CopyTo(fileStream);
                }
                Image new_image = new Image();
                new_image.CreatedAt = DateTime.Now;
                new_image.Path = fileName;
                new_image.DepartmentId = departmentId;
                _imageRepository.Add(new_image);
            }

            throw new NotImplementedException();
        }

    }
}