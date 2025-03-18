using Lib.DataAccess.Repository.IRepository;
using Lib.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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

        public IActionResult Index()
        {
            // Fetch only 3 courses
            IEnumerable<Course> courseList = _unitOfWork.Course.GetAll(includeProperties: "Category")
                                                .Take(3);
            return View(courseList);
        }

        public IActionResult Details(int courseId)
        {
            Course course = _unitOfWork.Course.Get(u => u.CourseId == courseId, includeProperties: "Category");

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
