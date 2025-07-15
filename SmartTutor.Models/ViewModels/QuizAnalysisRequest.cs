using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTutor.Models.ViewModels
{
    public class QuizAnalysisRequest
    {
        public string UserId { get; set; }
        public int ChapterId { get; set; }
        public List<QuestionAnswer> Answers { get; set; }
        public int TimeTaken { get; set; } // in seconds
        public int TabSwitches { get; set; }
    }
}
