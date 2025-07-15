using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SmartTuror.Models
{
    [Table("CourseEnrollments")]
    public class CourseEnrollment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column("UserId")]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        [ValidateNever]
        public ApplicationUser User { get; set; }

        [Required]
        [Column("CourseId")]
        public int CourseId { get; set; }

        [ForeignKey("CourseId")]
        [ValidateNever]
        public Course Course { get; set; }

        [Column("EnrollmentDate")]
        public DateTime EnrollmentDate { get; set; } = DateTime.Now;

        [Column("Status")]
        public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;

        [Column("CompletionDate")]
        public DateTime? CompletionDate { get; set; }

        [Required]
        [Column("ProgressId")]
        public int ProgressId { get; set; }

        [ForeignKey("ProgressId")]
        [ValidateNever]
        public UserProgress Progress { get; set; }
    }

    public enum EnrollmentStatus
    {
        Active,
        Completed,
        Dropped
    }
} 