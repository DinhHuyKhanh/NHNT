using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHNT.Dtos;
using NHNT.Models;
using NHNT.Services;
using NHNT.Services.Implement;
using NHNT.Repositories;
using Microsoft.AspNetCore.Http;

namespace NHNT.Controllers
{
    public class DepartmentController : ControllerCustom
    {
        private readonly ILogger<HomeController> _logger;
        private IDepartmentService _departmentService;
        private IDepartmentRepository _departmentRepository;

        public DepartmentController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult register(DepartmentDto department, List<IFormFile> images)
        {
            _departmentService.register(department, images);
            return View();
        }


    }
}
