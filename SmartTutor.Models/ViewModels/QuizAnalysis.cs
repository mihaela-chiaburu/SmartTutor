using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTutor.Models.ViewModels
{
    public class QuizAnalysis
    {
        public double ConfidenceLevel { get; set; } 
        public List<LearningResource> SuggestedResources { get; set; }
        public string Recommendations { get; set; }
    }
}
