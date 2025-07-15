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

            if (courseVM.Course.Chapters == null || courseVM.Course.Chapters.Count == 0)
            {
                courseVM.Course.Chapters = new List<Chapter> { new Chapter() };
            }

            return View(courseVM);
        }




        [HttpPost]
        public IActionResult Upsert(CourseVM courseVM, List<IFormFile> files)
        {
            courseVM.Course.Quizzes ??= new List<Quiz>();
            if (!ModelState.IsValid)
            {
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
                    _unitOfWork.Course.Add(courseVM.Course);  
                }
                else
                {
                    _unitOfWork.Course.Update(courseVM.Course);  

                }

                _unitOfWork.Save();

                string wwwRootPath = _webHostEnvironment.WebRootPath;

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

                if (courseVM.Course.Chapters != null)
                {
                    foreach (var chapter in courseVM.Course.Chapters)
                    {
                        if (chapter.ChapterId == 0)
                        {
                            chapter.CourseId = courseVM.Course.CourseId; 
                            _unitOfWork.Chapter.Add(chapter);
                        }
                        else
                        {
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

            string coursePath = Path.Combine(_webHostEnvironment.WebRootPath, "images/courses/course-" + id);

            if (Directory.Exists(coursePath))
            {
                foreach (string filePath in Directory.GetFiles(coursePath))
                {
                    System.IO.File.Delete(filePath);
                }
                Directory.Delete(coursePath);
            }

            _unitOfWork.Course.Remove(courseToBeDeleted);
            _unitOfWork.Save();

            return RedirectToAction("Index"); 
        }


        #endregion
    }
}
