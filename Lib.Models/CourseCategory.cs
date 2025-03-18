using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Lib.Models
{
    public class CourseCategory
    {
        [Key]
        public int CourseCategoryId { get; set; }

        [Required]
        [MaxLength(30)]
        [DisplayName("Category Name")]
        public string Name { get; set; }

        [Range(1, 100, ErrorMessage = "Display Order must be between 1-100")] // Fixed typo
        [DisplayName("Display Order")]
        public int DisplayOrder { get; set; }
    }
}
