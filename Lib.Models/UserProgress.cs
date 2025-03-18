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
    public class UserProgress
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course Course { get; set; }

        public double ProgressPercentage { get; set; }
    }

}
