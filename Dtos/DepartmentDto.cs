using System;
using System.Collections.Generic;
using System.Linq;
using NHNT.Constants;
using NHNT.Models;
using System.ComponentModel.DataAnnotations;


namespace NHNT.Dtos
{
    public class DepartmentDto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public Decimal Price { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        public Decimal Acreage { get; set; }

        public DepartmentStatus Status { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public bool IsAvailable { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        [Required]
        public UserDto User { get; set; }

        [Required]
        public DepartmentGroupDto Group { get; set; }

        public ICollection<ImageDto> Images { get; set; }

        public DepartmentDto(Department department)
        {
            if (department == null)
            {
                return;
            }

            this.Id = department.Id;
            this.Address = department.Address;
            this.Price = department.Price;
            this.PhoneNumber = department.PhoneNumber;
            this.Acreage = department.Acreage;
            this.Status = department.Status;
            this.Description = department.Description;
            this.IsAvailable = department.IsAvailable ?? false;
            this.CreatedAt = department.CreatedAt;
            this.UpdatedAt = department.UpdatedAt;
            this.User = new UserDto(department.User);
            this.Group = new DepartmentGroupDto(department.Group);

            if (department.Images != null && department.Images.Any())
            {
                this.Images = new List<ImageDto>();
                foreach (Image image in department.Images)
                {
                    Images.Add(new ImageDto(image));
                }
            }
        }
    }
}