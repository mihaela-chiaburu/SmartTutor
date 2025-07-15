using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using SmartTuror.Models;

namespace SmartTutor.Models
{
    public class UserAnswer
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public int QuizId { get; set; }
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }
        public bool IsCorrect { get; set; }
        public DateTime AnsweredOn { get; set; }
        public double ResponseTime { get; set; }
        public int TabSwitches { get; set; }

        [ForeignKey("UserId")]
        public IdentityUser User { get; set; }

        [ForeignKey("QuizId")]
        public Quiz Quiz { get; set; }

        [ForeignKey("QuestionId")]
        public Question Question { get; set; }

        [ForeignKey("AnswerId")]
        public Answer Answer { get; set; }
    }
}
