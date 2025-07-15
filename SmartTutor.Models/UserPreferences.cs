using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartTuror.Models
{
    public class UserPreferences
    {
        [Key]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        public int? PreferredCategoryId { get; set; }

        [ForeignKey("PreferredCategoryId")]
        public CourseCategory PreferredCategory { get; set; }

        public bool EmailNotifications { get; set; } = true;
        public bool CourseUpdates { get; set; } = true;
        public bool QuizResults { get; set; } = true;
        public bool ProgressUpdates { get; set; } = true;
    }
} 