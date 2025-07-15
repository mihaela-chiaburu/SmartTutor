using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTuror.Utility
{
    public static class SD
    {
        // User roles 
        public const string Role_Student = "Student";
        public const string Role_Professor = "Professor";
        public const string Role_Admin = "Admin";

        // Course and quiz statuses
        public const string StatusEnrolled = "Enrolled";
        public const string StatusInProgress = "InProgress";
        public const string StatusCompleted = "Completed";
        public const string StatusFailed = "Failed";

        // Quiz statuses
        public const string QuizStatusNotStarted = "NotStarted";
        public const string QuizStatusOngoing = "Ongoing";
        public const string QuizStatusFinished = "Finished";

    }
}

