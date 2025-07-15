using SmartTuror.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using SmartTutor.Models;

namespace SmartTuror.Models
{
    [Table("UserProgresses")]
    public class UserProgress
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column("UserId")]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        [ValidateNever]
        public ApplicationUser User { get; set; }

        [Required]
        [Column("CourseId")]
        public int CourseId { get; set; }

        [ForeignKey("CourseId")]
        [ValidateNever]
        public Course Course { get; set; }

        [Column("ProgressPercentage")]
        public double ProgressPercentage { get; set; }

        [Column("LastAccessed")]
        public DateTime LastAccessed { get; set; }

        [ValidateNever]
        public ICollection<ChapterProgress> ChapterProgresses { get; set; } = new List<ChapterProgress>();

        [ValidateNever]
        public ICollection<QuizResult> QuizResults { get; set; } = new List<QuizResult>();

        [ValidateNever]
        public CourseEnrollment Enrollment { get; set; }

        public bool IsCompleted => ProgressPercentage >= 100;
    }
}
