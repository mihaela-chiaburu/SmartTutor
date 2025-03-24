﻿using Lib.Models;
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

        public int ChapterId { get; set; } // Link quiz to a chapter
        [ForeignKey("ChapterId")]
        public Chapter Chapter { get; set; }

        public List<Question> Questions { get; set; } = new List<Question>();
    }
}

