﻿using SmartTuror.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartTutor.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SmartTuror.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<CourseCategory> Categories { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<CourseCategory> CourseCategories { get; set; }
        public DbSet<CourseImage> CourseImages { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<UserProgress> UserProgresses { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<QuizResult> QuizResults { get; set; }
        public DbSet<UserAnswer> UserAnswers { get; set; }
        public DbSet<CourseEnrollment> CourseEnrollments { get; set; }
        public DbSet<ChapterProgress> ChapterProgresses { get; set; }
        public DbSet<UserPreferences> UserPreferences { get; set; }
        public DbSet<RealTimePerformance> RealTimePerformances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CourseEnrollment>(entity =>
            {
                entity.ToTable("CourseEnrollments");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).UseIdentityColumn();
                
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.Course)
                    .WithMany()
                    .HasForeignKey(e => e.CourseId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.Progress)
                    .WithOne(p => p.Enrollment)
                    .HasForeignKey<CourseEnrollment>(e => e.ProgressId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserProgress>(entity =>
            {
                entity.ToTable("UserProgresses");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).UseIdentityColumn();

                entity.HasOne(up => up.User)
                    .WithMany()
                    .HasForeignKey(up => up.UserId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(up => up.Course)
                    .WithMany()
                    .HasForeignKey(up => up.CourseId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Quiz>()
                .HasMany(q => q.Questions)
                .WithOne(q => q.Quiz)
                .HasForeignKey(q => q.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Question>()
                .HasMany(q => q.Answers)
                .WithOne(a => a.Question)
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserProgress>()
                .HasOne(up => up.User)
                .WithMany()
                .HasForeignKey(up => up.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserProgress>()
                .HasOne(up => up.Course)
                .WithMany()
                .HasForeignKey(up => up.CourseId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<CourseCategory>().HasData(
                new CourseCategory { CourseCategoryId = 1, Name = "Programming", DisplayOrder = 1 },
                new CourseCategory { CourseCategoryId = 2, Name = "Business", DisplayOrder = 2 },
                new CourseCategory { CourseCategoryId = 3, Name = "Design", DisplayOrder = 3 }
            );

            modelBuilder.Entity<Course>().HasData(
                new Course
                {
                    CourseId = 1,
                    Title = "C# for Beginners",
                    Description = "Learn the basics of C# programming with this beginner-friendly course.",
                    CategoryId = 1,
                    CreatedByUserId = null
                },
                new Course
                {
                    CourseId = 2,
                    Title = "Startup Business Strategies",
                    Description = "Explore key strategies for building a successful startup.",
                    CategoryId = 2,
                    CreatedByUserId = null
                },
                new Course
                {
                    CourseId = 3,
                    Title = "Graphic Design Essentials",
                    Description = "Master the fundamentals of graphic design with this hands-on course.",
                    CategoryId = 3,
                    CreatedByUserId = null
                }
            );

            modelBuilder.Entity<Quiz>().HasData(
                new Quiz
                {
                    Id = 1,
                    Title = "Object-Oriented Programming (OOP) Quiz",
                    ChapterId = 2
                }
            );

            modelBuilder.Entity<Question>().HasData(
                new Question { Id = 1, Text = "What is object-oriented programming (OOP)?", QuizId = 1 },
                new Question { Id = 2, Text = "What does OOP focus on?", QuizId = 1 },
                new Question { Id = 3, Text = "What is the first step in OOP?", QuizId = 1 },
                new Question { Id = 4, Text = "What is the benefit of OOP for collaborative development?", QuizId = 1 },
                new Question { Id = 5, Text = "What is an example of an object in OOP?", QuizId = 1 }
            );

            modelBuilder.Entity<Answer>().HasData(
                new Answer { Id = 1, Text = "A programming language", IsCorrect = false, QuestionId = 1 },
                new Answer { Id = 2, Text = "A method of organizing software", IsCorrect = true, QuestionId = 1 },
                new Answer { Id = 3, Text = "A database model", IsCorrect = false, QuestionId = 1 },
                new Answer { Id = 4, Text = "A tool for debugging code", IsCorrect = false, QuestionId = 1 },

                new Answer { Id = 5, Text = "Objects", IsCorrect = true, QuestionId = 2 },
                new Answer { Id = 6, Text = "Methods", IsCorrect = false, QuestionId = 2 },
                new Answer { Id = 7, Text = "Procedures", IsCorrect = false, QuestionId = 2 },
                new Answer { Id = 8, Text = "Variables", IsCorrect = false, QuestionId = 2 },

                new Answer { Id = 9, Text = "Model the data", IsCorrect = true, QuestionId = 3 },
                new Answer { Id = 10, Text = "Write the methods", IsCorrect = false, QuestionId = 3 },
                new Answer { Id = 11, Text = "Define the classes", IsCorrect = false, QuestionId = 3 },
                new Answer { Id = 12, Text = "Test the code", IsCorrect = false, QuestionId = 3 },

                new Answer { Id = 13, Text = "Improved communication", IsCorrect = false, QuestionId = 4 },
                new Answer { Id = 14, Text = "Code reusability", IsCorrect = true, QuestionId = 4 },
                new Answer { Id = 15, Text = "Faster development", IsCorrect = false, QuestionId = 4 },
                new Answer { Id = 16, Text = "More bugs", IsCorrect = false, QuestionId = 4 },

                new Answer { Id = 17, Text = "A person", IsCorrect = false, QuestionId = 5 },
                new Answer { Id = 18, Text = "A car", IsCorrect = false, QuestionId = 5 },
                new Answer { Id = 19, Text = "A widget", IsCorrect = true, QuestionId = 5 },
                new Answer { Id = 20, Text = "A method", IsCorrect = false, QuestionId = 5 }
            );

            modelBuilder.Entity<UserPreferences>()
                .HasOne(p => p.User)
                .WithOne()
                .HasForeignKey<UserPreferences>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserPreferences>()
                .HasOne(p => p.PreferredCategory)
                .WithMany()
                .HasForeignKey(p => p.PreferredCategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
