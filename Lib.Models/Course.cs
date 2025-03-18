using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Lib.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }

        [Required]
        public string Title { get; set; }

        public string? Description { get; set; }

        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        [ValidateNever]
        public CourseCategory Category { get; set; }

        public string? CreatedByUserId { get; set; }  // Make this nullable

        [ForeignKey("CreatedByUserId")]
        public ApplicationUser? CreatedBy { get; set; }  // Make the navigation property nullable
        [ValidateNever]
        public List<CourseImage> CourseImages { get; set; }
        public List<Chapter> Chapters { get; set; } // Add this line
    }
}
