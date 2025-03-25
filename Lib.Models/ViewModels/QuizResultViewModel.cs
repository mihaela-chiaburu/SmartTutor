using Lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTutor.Models.ViewModels
{
    public class QuizResultViewModel
    {
        public double Score { get; set; }
        public QuizAnalysis Analysis { get; set; }
        public Chapter Chapter { get; set; }
    }
}
