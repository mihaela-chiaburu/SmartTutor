using Lib.DataAccess.Data;
using Lib.DataAccess.Repository.IRepository;
using Lib.DataAccess.Repository;
using Lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.DataAccess.Repository
{
    public class QuizRepository : Repository<Quiz>, IQuizRepository
    {
        private ApplicationDbContext _db;
        public QuizRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Quiz obj)
        {
            _db.Quizzes.Update(obj);
        }
    }
}
