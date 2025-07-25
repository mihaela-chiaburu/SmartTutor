﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTuror.Models.ViewModels
{
    public class CourseVM
    {
        public Course Course { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> CategoryList { get; set; }
        public List<Chapter> Chapters { get; set; } = new List<Chapter>(); 

        public CourseVM()
        {
            Course = new Course
            {
                Chapters = new List<Chapter> { new Chapter() } 
            };
        }
    }
}
