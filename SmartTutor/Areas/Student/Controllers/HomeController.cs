using SmartTuror.DataAccess.Repository.IRepository;
using SmartTuror.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace SmartTutor.Areas.Student.Controllers
{
    [Area("Student")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public IActionResult Index(List<int> categoryIds)
        {
            var categories = _unitOfWork.CourseCategory.GetAll().ToList();
            ViewBag.Categories = categories;

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

        public async Task<IActionResult> Details(int courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Course course = _unitOfWork.Course.Get(
                u => u.CourseId == courseId,
                includeProperties: "Category,Chapters,CourseImages"
            );

            if (course == null)
            {
                return NotFound();
            }

            if (userId != null)
            {
                var enrollment = await _unitOfWork.CourseEnrollment.GetAsync(
                    e => e.UserId == userId && e.CourseId == courseId,
                    includeProperties: "Progress"
                );

                ViewBag.Enrollment = enrollment;
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
                return NotFound(); 
            }

            return View(chapter);
        }

        [HttpPost]
        public IActionResult SubmitQuiz(int courseId, Dictionary<int, int> answers)
        {
            var course = _unitOfWork.Course.Get(u => u.CourseId == courseId, includeProperties: "Quizzes,Quizzes.Questions,Quizzes.Answers");

            if (course == null || !course.Quizzes.Any())
            {
                return NotFound();
            }

            var quiz = course.Quizzes.FirstOrDefault(); 
            if (quiz == null)
            {
                return NotFound(); 
            }

            var correctAnswersCount = 0;

            foreach (var question in quiz.Questions)
            {
                var userAnswerId = answers.FirstOrDefault(a => a.Key == question.Id).Value;
                var correctAnswer = question.Answers.FirstOrDefault(a => a.Id == userAnswerId && a.IsCorrect == true); 

                if (correctAnswer != null)
                {
                    correctAnswersCount++;
                }
            }

            var totalQuestions = quiz.Questions.Count;
            var score = (double)correctAnswersCount / totalQuestions * 100;

            return View("QuizResult", new { Score = score });
        }

        [HttpPost]
        public async Task<IActionResult> Enroll(int courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            UserProgress progress = null;
            try
            {
                var existingEnrollment = await _unitOfWork.CourseEnrollment.GetAsync(
                    e => e.UserId == userId && e.CourseId == courseId
                );

                if (existingEnrollment != null)
                {
                    TempData["error"] = "You are already enrolled in this course.";
                    return RedirectToAction("Details", new { courseId });
                }

                var course = await _unitOfWork.Course.GetAsync(
                    c => c.CourseId == courseId,
                    includeProperties: "Chapters"
                );

                if (course == null)
                {
                    TempData["error"] = "Course not found.";
                    return RedirectToAction("Index");
                }

                try
                {
                    progress = new UserProgress
                    {
                        UserId = userId,
                        CourseId = courseId,
                        ProgressPercentage = 0,
                        LastAccessed = DateTime.Now
                    };

                    _unitOfWork.UserProgress.Add(progress);
                    await _unitOfWork.SaveAsync();
                    _logger.LogInformation("Created UserProgress with ID: {ProgressId}", progress.Id);

                    var enrollment = new CourseEnrollment
                    {
                        UserId = userId,
                        CourseId = courseId,
                        EnrollmentDate = DateTime.Now,
                        Status = EnrollmentStatus.Active,
                        ProgressId = progress.Id 
                    };

                    _unitOfWork.CourseEnrollment.Add(enrollment);
                    await _unitOfWork.SaveAsync();
                    _logger.LogInformation("Created CourseEnrollment with ID: {EnrollmentId}", enrollment.Id);

                    foreach (var chapter in course.Chapters)
                    {
                        var chapterProgress = new ChapterProgress
                        {
                            UserId = userId,
                            ChapterId = chapter.ChapterId,
                            UserProgressId = progress.Id,
                            IsCompleted = false,
                            LastAccessed = DateTime.Now
                        };
                        _unitOfWork.ChapterProgress.Add(chapterProgress);
                    }

                    await _unitOfWork.SaveAsync();
                    _logger.LogInformation("Created ChapterProgress entries for course {CourseId}", courseId);

                    TempData["success"] = "Successfully enrolled in the course!";
                    return RedirectToAction("Details", new { courseId });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during enrollment process for course {CourseId} and user {UserId}. Progress ID: {ProgressId}", 
                        courseId, userId, progress?.Id);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enrolling in course {CourseId} for user {UserId}. Error details: {ErrorMessage}", 
                    courseId, userId, ex.Message);
                TempData["error"] = $"An error occurred while enrolling in the course: {ex.Message}";
                return RedirectToAction("Details", new { courseId });
            }
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