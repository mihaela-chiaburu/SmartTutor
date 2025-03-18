using Lib.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Lib.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
                
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


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CourseCategory>().HasData(
                new CourseCategory { CourseCategoryId = 1, Name = "Action", DisplayOrder = 1 },
                new CourseCategory { CourseCategoryId = 2, Name = "SciFi", DisplayOrder = 2 },
                new CourseCategory { CourseCategoryId = 3, Name = "History", DisplayOrder = 3 }
                );

            modelBuilder.Entity<Course>().HasData(
                new Course
                {
                    CourseId = 1,
                    Title = "Fortune of Time",
                    CategoryId = 1
                },
                new Course
                {
                    CourseId = 2,
                    Title = "Fortune of Time",
                    CategoryId = 1
                },
                new Course
                {
                    CourseId = 3,
                    Title = "Fortune of Time",
                    CategoryId = 1
                }
                );
        }
    }
}
