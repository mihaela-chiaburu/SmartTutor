using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Models
{
    public class Question
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Text { get; set; }
        public int QuizId { get; set; }
        [ForeignKey("QuizId")]
        public Quiz Quiz { get; set; }
    }

}
