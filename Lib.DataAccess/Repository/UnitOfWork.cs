using Lib.DataAccess.Data;
using Lib.DataAccess.Repository.IRepository;
using Lib.Models;
using System;

namespace Lib.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;

        public ICourseRepository Course { get; private set; }
        public IQuizRepository Quiz { get; private set; }
        public IQuestionRespository Question { get; private set; }
        public IAnswerRepository Answer { get; private set; }
        public IUserProgressRepository UserProgress { get; private set; }
        public ICourseImageRepository CourseImage { get; private set; } 
        public ICourseCategoryRepository CourseCategory { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Course = new CourseRepository(_db);
            Quiz = new QuizRepository(_db);
            Question = new QuestionRepository(_db);
            Answer = new AnswerRepository(_db);
            UserProgress = new UserProgressRepository(_db);
            CourseImage = new CourseImageRepository(_db);
            CourseCategory = new CourseCategoryRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
