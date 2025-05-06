using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lib.Models
{
    public class RealTimePerformance
    {
        [Key]
        public int Id { get; set; }
        
        public string UserId { get; set; }
        public int QuizId { get; set; }
        public int QuestionId { get; set; }
        
        public double ResponseTime { get; set; } // Time taken to answer in seconds
        public int TabSwitches { get; set; }
        public bool IsCorrect { get; set; }
        public double ConfidenceLevel { get; set; } // 0-1 scale
        public DateTime Timestamp { get; set; }
        
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        
        [ForeignKey("QuizId")]
        public Quiz Quiz { get; set; }
        
        [ForeignKey("QuestionId")]
        public Question Question { get; set; }
    }
} 