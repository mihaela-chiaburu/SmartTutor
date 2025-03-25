using Lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTutor.Models.ViewModels
{
    public class QuizViewModel
    {
        public Quiz Quiz { get; set; }
        public int ChapterId { get; set; }
    }
}
