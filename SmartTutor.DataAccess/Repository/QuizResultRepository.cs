using SmartTuror.DataAccess.Data;
using SmartTuror.DataAccess.Repository.IRepository;
using SmartTuror.DataAccess.Repository;
using SmartTuror.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartTutor.Models;

namespace SmartTuror.DataAccess.Repository
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
