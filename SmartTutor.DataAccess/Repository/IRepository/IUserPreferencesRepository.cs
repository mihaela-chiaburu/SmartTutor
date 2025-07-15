using SmartTuror.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SmartTuror.DataAccess.Repository.IRepository
{
    public interface IUserPreferencesRepository : IRepository<UserPreferences>
    {
        void Update(UserPreferences obj);
        Task<UserPreferences> GetFirstOrDefaultAsync(Expression<Func<UserPreferences, bool>> filter);
    }
} 