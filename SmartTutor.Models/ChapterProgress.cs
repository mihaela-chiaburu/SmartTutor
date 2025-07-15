using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using SmartTutor.Models;

namespace SmartTuror.Models
{
    public class ChapterProgress
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        [ValidateNever]
        public ApplicationUser User { get; set; }

        [Required]
        public int ChapterId { get; set; }

        [ForeignKey("ChapterId")]
        [ValidateNever]
        public Chapter Chapter { get; set; }

        [Required]
        public int UserProgressId { get; set; }

        [ForeignKey("UserProgressId")]
        [ValidateNever]
        public UserProgress UserProgress { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime? CompletedDate { get; set; }

        public double TimeSpent { get; set; } 

        public int LastAccessedPage { get; set; }

        public DateTime LastAccessed { get; set; } = DateTime.Now;

        [ValidateNever]
        public List<QuizResult> QuizResults { get; set; } = new List<QuizResult>();

        public double ChapterScore => QuizResults.Count > 0 
            ? QuizResults.Average(q => q.Score) 
            : 0;
    }
} 