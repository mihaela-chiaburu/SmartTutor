using Lib.DataAccess.Data;
using Lib.DataAccess.Repository.IRepository;
using Lib.DataAccess.Repository;
using Lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartTutor.Models;

namespace Lib.DataAccess.Repository
{
    public class QuizResultRepository : Repository<QuizResult>, IQuizResultRepository
    {
        private ApplicationDbContext _db;
        public QuizResultRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(QuizResult obj)
        {
            _db.QuizResults.Update(obj);
        }
    }
}
