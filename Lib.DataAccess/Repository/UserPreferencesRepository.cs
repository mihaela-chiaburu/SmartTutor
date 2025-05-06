using Lib.DataAccess.Data;
using Lib.DataAccess.Repository.IRepository;
using Lib.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Lib.DataAccess.Repository
{
    public class UserPreferencesRepository : Repository<UserPreferences>, IUserPreferencesRepository
    {
        private ApplicationDbContext _db;
        public UserPreferencesRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(UserPreferences obj)
        {
            _db.UserPreferences.Update(obj);
        }

        public async Task<UserPreferences> GetFirstOrDefaultAsync(Expression<Func<UserPreferences, bool>> filter)
        {
            return await _db.UserPreferences.FirstOrDefaultAsync(filter);
        }
    }
} 