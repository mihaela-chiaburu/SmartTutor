using Lib.DataAccess.Repository.IRepository;
using Lib.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace SmartTutor.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(List<int> categoryIds)
        {
            // Fetch all categories for the filter form
            var categories = _unitOfWork.CourseCategory.GetAll().ToList();
            ViewBag.Categories = categories;

            // Fetch courses based on selected categories
            IEnumerable<Course> courseList;
            if (categoryIds != null && categoryIds.Any())
            {
                courseList = _unitOfWork.Course.GetAll(
                    filter: c => categoryIds.Contains(c.CategoryId),
                    includeProperties: "Category,CourseImages"
                );
            }
            else
            {
                courseList = _unitOfWork.Course.GetAll(includeProperties: "Category,CourseImages");
            }

            return View(courseList);
        }

        public IActionResult Details(int courseId)
        {
            Course course = _unitOfWork.Course.Get(u => u.CourseId == courseId, includeProperties: "Category,Chapters");

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}