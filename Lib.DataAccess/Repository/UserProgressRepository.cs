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
