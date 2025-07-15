using SmartTuror.DataAccess.Data;
using SmartTuror.DataAccess.Repository.IRepository;
using SmartTuror.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTuror.DataAccess.Repository
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
        public IChapterRepository Chapter { get; private set; }
        public IQuizResultRepository QuizResult { get; private set; }
        public IUserAnswerRepository UserAnswer { get; private set; }
        public ICourseEnrollmentRepository CourseEnrollment { get; private set; }
        public IChapterProgressRepository ChapterProgress { get; private set; }
        public IUserPreferencesRepository UserPreferences { get; private set; }
        public IRealTimePerformanceRepository RealTimePerformances { get; private set; }


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
            Chapter = new ChapterRepository(_db);
            QuizResult = new QuizResultRepository(_db);
            UserAnswer = new UserAnswerRepository(_db);
            CourseEnrollment = new CourseEnrollmentRepository(_db);
            ChapterProgress = new ChapterProgressRepository(_db);
            UserPreferences = new UserPreferencesRepository(_db);
            RealTimePerformances = new RealTimePerformanceRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
