using SmartTuror.DataAccess.Data;
using SmartTuror.DataAccess.Repository.IRepository;
using SmartTuror.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SmartTuror.DataAccess.Repository
{
    public class UserProgressRepository : Repository<UserProgress>, IUserProgressRepository
    {
        private ApplicationDbContext _db;
        public UserProgressRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(UserProgress obj)
        {
            _db.UserProgresses.Update(obj);
        }
    }
}
