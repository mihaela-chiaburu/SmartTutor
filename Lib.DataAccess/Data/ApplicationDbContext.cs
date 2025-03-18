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

            // Adding categories
            modelBuilder.Entity<CourseCategory>().HasData(
                new CourseCategory { CourseCategoryId = 1, Name = "Programming", DisplayOrder = 1 },
                new CourseCategory { CourseCategoryId = 2, Name = "Business", DisplayOrder = 2 },
                new CourseCategory { CourseCategoryId = 3, Name = "Design", DisplayOrder = 3 }
            );

            // Adding dummy courses
            modelBuilder.Entity<Course>().HasData(
                new Course
                {
                    CourseId = 1,
                    Title = "C# for Beginners",
                    Description = "Learn the basics of C# programming with this beginner-friendly course.",
                    CategoryId = 1,
                    CreatedByUserId = "6cd7510b-1cae-49c8-bb4f-9922ca24b874"
                },
                new Course
                {
                    CourseId = 2,
                    Title = "Startup Business Strategies",
                    Description = "Explore key strategies for building a successful startup.",
                    CategoryId = 2,
                    CreatedByUserId = "6cd7510b-1cae-49c8-bb4f-9922ca24b874"
                },
                new Course
                {
                    CourseId = 3,
                    Title = "Graphic Design Essentials",
                    Description = "Master the fundamentals of graphic design with this hands-on course.",
                    CategoryId = 3,
                    CreatedByUserId = "6cd7510b-1cae-49c8-bb4f-9922ca24b874"
                }
            );
        }
    }
}
