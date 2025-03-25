using Lib.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTutor.Models
{
    public class QuizResult
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public int QuizId { get; set; }
        public double Score { get; set; }
        public int TimeTaken { get; set; } // in seconds
        public int TabSwitches { get; set; }
        public double ConfidenceLevel { get; set; }
        public string? SuggestedResources { get; set; } // JSON serialized
        public DateTime TakenOn { get; set; }

        [ForeignKey("UserId")]
        public IdentityUser User { get; set; }

        [ForeignKey("QuizId")]
        public Quiz Quiz { get; set; }
    }
}
