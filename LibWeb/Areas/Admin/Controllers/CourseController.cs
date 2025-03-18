using Lib.DataAccess.Repository.IRepository;
using Lib.Models;
using Lib.Models.ViewModels;
using Lib.Utility;
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

        public CourseController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
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

            if (id == null || id == 0)
            {
                return View(courseVM);
            }
            else
            {
                courseVM.Course = _unitOfWork.Course.Get(u => u.CourseId == id, includeProperties: "CourseImages");
                return View(courseVM);
            }
        }

        [HttpPost]
        public IActionResult Upsert(CourseVM courseVM, List<IFormFile> files)
        {
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

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var courseToBeDeleted = _unitOfWork.Course.Get(u => u.CourseId == id);
            if (courseToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting course" });
            }

            // Delete course images
            string coursePath = @"images\courses\course-" + id;
            string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, coursePath);

            if (Directory.Exists(finalPath))
            {
                string[] filePaths = Directory.GetFiles(finalPath);
                foreach (string filePath in filePaths)
                {
                    System.IO.File.Delete(filePath);
                }
                Directory.Delete(finalPath);
            }

            // Remove the course from database
            _unitOfWork.Course.Remove(courseToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Course deleted successfully" });
        }

        #endregion
    }
}
