using Lib.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICourseCategoryRepository CourseCategory { get; }
        ICourseRepository Course { get; }
        IApplicationUserRepository ApplicationUser { get; }
        ICourseImageRepository CourseImage { get; }
        IQuizRepository Quiz { get; }
        IQuestionRespository Question { get; }
        IAnswerRepository Answer { get; }
        IChapterRepository Chapter { get; }
        IQuizResultRepository QuizResult { get; }
        void Save();
        Task SaveAsync();
    }
}
