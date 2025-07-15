using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTutor.Models.ViewModels
{
    public class QuizSubmission
    {
        public int QuizId { get; set; }
        public List<QuestionAnswer> Answers { get; set; }
        public int TimeTaken { get; set; } 
        public int TabSwitches { get; set; }
    }
}
