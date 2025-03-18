using Lib.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Models
{
    public class Quiz
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course Course { get; set; }
    }

}
