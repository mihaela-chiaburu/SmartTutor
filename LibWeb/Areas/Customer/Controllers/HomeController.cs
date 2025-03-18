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
            Course course = _unitOfWork.Course.Get(
                u => u.CourseId == courseId,
                includeProperties: "Category,Chapters,CourseImages"
            );

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        public IActionResult ViewChapter(int id)
        {
            var chapter = _unitOfWork.Chapter.Get(
                c => c.ChapterId == id,
                includeProperties: "Course,Course.Quizzes,Course.Quizzes.Questions,Course.Quizzes.Questions.Answers"
            );

            if (chapter == null)
            {
                return NotFound(); // If chapter doesn't exist, return 404
            }

            return View(chapter);
        }




        [HttpPost]
        public IActionResult SubmitQuiz(int courseId, Dictionary<int, int> answers)
        {
            var course = _unitOfWork.Course.Get(u => u.CourseId == courseId, includeProperties: "Quizzes,Quizzes.Questions,Quizzes.Answers");

            if (course == null || !course.Quizzes.Any())
            {
                return NotFound(); // If no quizzes are associated with the course, return 404
            }

            // Assuming we need the first quiz, but if you want a specific quiz, you can adjust accordingly
            var quiz = course.Quizzes.FirstOrDefault(); // Assuming you are dealing with one quiz, but modify if needed
            if (quiz == null)
            {
                return NotFound(); // If no quiz is found, return 404
            }

            var correctAnswersCount = 0;

            foreach (var question in quiz.Questions)
            {
                // Check if the answer submitted for this question is correct
                var userAnswerId = answers.FirstOrDefault(a => a.Key == question.Id).Value;
                var correctAnswer = question.Answers.FirstOrDefault(a => a.Id == userAnswerId && a.IsCorrect);

                if (correctAnswer != null)
                {
                    correctAnswersCount++;
                }
            }

            // You can calculate the score as needed (e.g., percentage)
            var totalQuestions = quiz.Questions.Count;
            var score = (double)correctAnswersCount / totalQuestions * 100;

            // Pass the score to the view or show a result page
            return View("QuizResult", new { Score = score });
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