using Lib.DataAccess.Repository.IRepository;
using Lib.Models;
using SmartTutor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.DataAccess.Repository.IRepository
{
    public interface IQuizResultRepository : IRepository<QuizResult>
    {
        void Update(QuizResult obj);
    }
}
