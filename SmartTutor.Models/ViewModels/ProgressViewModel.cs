using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTutor.Models.ViewModels
{
    public class ProgressViewModel
    {
        public List<QuizResult> QuizResults { get; set; }
        public double AverageScore { get; set; }
        public double BestScore { get; set; }
        public int AttemptsCount { get; set; }

        public string ScoreTrendData => string.Join(",",
            QuizResults.OrderBy(r => r.TakenOn).Select(r => r.Score));

        public string TimeTrendData => string.Join(",",
            QuizResults.OrderBy(r => r.TakenOn).Select(r => r.TimeTaken));

        public string DateLabels => string.Join(",",
            QuizResults.OrderBy(r => r.TakenOn).Select(r => r.TakenOn.ToString("MMM dd")));
    }
}
