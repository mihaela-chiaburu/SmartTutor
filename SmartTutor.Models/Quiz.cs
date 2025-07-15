using SmartTuror.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTuror.Models
{
    public class Quiz
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public int ChapterId { get; set; }
        [ForeignKey("ChapterId")]
        public Chapter Chapter { get; set; }

        // Add these new properties
        public int? CourseId { get; set; } // Nullable since it's optional
        [ForeignKey("CourseId")]
        public Course? Course { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public List<Question> Questions { get; set; } = new List<Question>();
    }
}

