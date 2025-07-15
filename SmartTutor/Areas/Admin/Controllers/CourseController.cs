using SmartTuror.DataAccess.Repository.IRepository;
using SmartTuror.Models;
using SmartTuror.Models.ViewModels;
using SmartTuror.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using System.Linq;
using System;

namespace SmartTutor.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CourseController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly AIService _aiService;


        public CourseController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, AIService aiService)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _aiService = aiService;

        }

        /*[HttpPost]
        public async Task<IActionResult> GenerateChapterContent([FromBody] GenerateChapterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return Json(new { success = false, message = "Title is required." });
            }

            try
            {
                // Clear and specific AI prompt
               /*var prompt = $"You'll receive a chapter title as input. Your task is to write a short, clear, and structured chapter on this topic. " +
                             $"Avoid any introductory phrases. Go straight to the point and provide valuable, detailed information. " +
                             $"The title is: \"{request.Title}\". Write the chapter content now.";*/

                /*var generatedContent = _aiService.GenerateChapterContent(request.Title);

                return Json(new { success = true, generatedContent });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }*/


        /*public class GenerateChapterRequest
        {
            public string Title { get; set; }
        }*/

        public IActionResult Index()
        {
            List<Course> objCourseList = _unitOfWork.Course.GetAll(includeProperties: "Category").ToList();
            return View(objCourseList);
        }

        public IActionResult Upsert(int? id)
        {
            CourseVM courseVM = new()
            {
                CategoryList = _unitOfWork.CourseCategory.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.CourseCategoryId.ToString()
                }),
                Course = new Course()
            };

            if (id != null && id != 0)
            {
                courseVM.Course = _unitOfWork.Course.Get(u => u.CourseId == id, includeProperties: "CourseImages,Chapters");
            }

            // Ensure Chapters is never null and contains at least one chapter
            if (courseVM.Course.Chapters == null || courseVM.Course.Chapters.Count == 0)
            {
                courseVM.Course.Chapters = new List<Chapter> { new Chapter() }; // Add a default chapter
            }

            return View(courseVM);
        }




        [HttpPost]
        public IActionResult Upsert(CourseVM courseVM, List<IFormFile> files)
        {
            // Initialize Quizzes if null
            courseVM.Course.Quizzes ??= new List<Quiz>();
            if (!ModelState.IsValid)
            {
                // Log errors to understand what is failing
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0)
                                       .Select(x => new { x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage) })
                                       .ToList();

                foreach (var error in errors)
                {
                    Console.WriteLine($"Field: {error.Key}");
                    foreach (var errorMessage in error.Errors)
                    {
                        Console.WriteLine($"Error: {errorMessage}");
                    }
                }

                TempData["error"] = "Validation failed. Please check all required fields.";

                // Reload Category List for dropdown
                courseVM.CategoryList = _unitOfWork.CourseCategory.GetAll()
                    .Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.CourseCategoryId.ToString()
                    });

                return View(courseVM);
            }

            if (ModelState.IsValid)
            {
                if (courseVM.Course.CourseId == 0)
                {
                    _unitOfWork.Course.Add(courseVM.Course);  // Add new course
                }
                else
                {
                    _unitOfWork.Course.Update(courseVM.Course);  // Update existing course

                }

                _unitOfWork.Save();

                // Save images (course images)
                string wwwRootPath = _webHostEnvironment.WebRootPath;

                // Process course images
                if (files != null && files.Count > 0)
                {
                    foreach (IFormFile file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string coursePath = @"images\courses\course-" + courseVM.Course.CourseId;
                        string finalPath = Path.Combine(wwwRootPath, coursePath);

                        if (!Directory.Exists(finalPath))
                            Directory.CreateDirectory(finalPath);

                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        CourseImage courseImage = new()
                        {
                            ImageUrl = @"\" + coursePath + @"\" + fileName,
                            CourseId = courseVM.Course.CourseId,
                        };

                        if (courseVM.Course.CourseImages == null)
                            courseVM.Course.CourseImages = new List<CourseImage>();

                        courseVM.Course.CourseImages.Add(courseImage);
                    }

                    _unitOfWork.Course.Update(courseVM.Course);
                    _unitOfWork.Save();
                }

                // Handle Chapters (without image handling)
                if (courseVM.Course.Chapters != null)
                {
                    foreach (var chapter in courseVM.Course.Chapters)
                    {
                        if (chapter.ChapterId == 0)
                        {
                            // New chapter
                            chapter.CourseId = courseVM.Course.CourseId; // Associate with the course
                            _unitOfWork.Chapter.Add(chapter);
                        }
                        else
                        {
                            // Existing chapter
                            _unitOfWork.Chapter.Update(chapter);
                        }
                    }
                }

                _unitOfWork.Save();

                TempData["success"] = "Course created/updated successfully";
                return RedirectToAction("Index");
            }
            else
            {
                courseVM.CategoryList = _unitOfWork.CourseCategory.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.CourseCategoryId.ToString()
                });
                return View(courseVM);
            }
        }


        public IActionResult DeleteImage(int imageId)
        {
            var imageToBeDeleted = _unitOfWork.CourseImage.Get(u => u.Id == imageId);
            int courseId = imageToBeDeleted.CourseId;

            if (imageToBeDeleted != null)
            {
                if (!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, imageToBeDeleted.ImageUrl.Trim('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                _unitOfWork.CourseImage.Remove(imageToBeDeleted);
                _unitOfWork.Save();

                TempData["success"] = "Image deleted successfully";
            }

            return RedirectToAction(nameof(Upsert), new { id = courseId });
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Course> objCourseList = _unitOfWork.Course.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objCourseList });
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var courseToBeDeleted = _unitOfWork.Course.Get(u => u.CourseId == id);
            if (courseToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting course" });
            }

            // Delete course images
            string coursePath = Path.Combine(_webHostEnvironment.WebRootPath, "images/courses/course-" + id);

            if (Directory.Exists(coursePath))
            {
                foreach (string filePath in Directory.GetFiles(coursePath))
                {
                    System.IO.File.Delete(filePath);
                }
                Directory.Delete(coursePath);
            }

            // Remove the course from database
            _unitOfWork.Course.Remove(courseToBeDeleted);
            _unitOfWork.Save();

            return RedirectToAction("Index"); // Redirect after delete
        }


        #endregion
    }
}
