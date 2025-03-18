using Lib.DataAccess.Repository.IRepository;
using Lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.DataAccess.Repository.IRepository
{
    public interface IUserProgressRepository : IRepository<UserProgress>
    {
        void Update(UserProgress obj);
    }
}
