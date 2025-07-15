using SmartTuror.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTutor.Models.ViewModels
{
    public class QuestionManagementVM
    {
        public int ChapterId { get; set; }
        public string ChapterTitle { get; set; }
        public List<Question> Questions { get; set; }
    }
}
