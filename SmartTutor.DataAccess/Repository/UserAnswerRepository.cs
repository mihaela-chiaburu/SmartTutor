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
    public class UserAnswerRepository : Repository<UserAnswer>, IUserAnswerRepository
    {
        private ApplicationDbContext _db;
        public UserAnswerRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(UserAnswer obj)
        {
            _db.UserAnswers.Update(obj);
        }
    }
}
