using Lib.DataAccess.Data;
using Lib.DataAccess.Repository.IRepository;
using Lib.Models;

namespace Lib.DataAccess.Repository
{
    public class RealTimePerformanceRepository : Repository<RealTimePerformance>, IRealTimePerformanceRepository
    {
        private ApplicationDbContext _db;
        public RealTimePerformanceRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(RealTimePerformance obj)
        {
            _db.RealTimePerformances.Update(obj);
        }
    }
} 