using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SmartTuror.Models
{
    public class Question
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Text { get; set; }

        public int QuizId { get; set; }

        [ForeignKey("QuizId")]
        [ValidateNever] // Add this to skip validation for navigation property
        public Quiz Quiz { get; set; }

        public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Medium;

        [ValidateNever] // Add this to skip validation for the collection
        public List<Answer> Answers { get; set; } = new List<Answer>();
    }

    public enum DifficultyLevel
    {
        Easy = 1,
        Medium = 2,
        Hard = 3
    }

}
