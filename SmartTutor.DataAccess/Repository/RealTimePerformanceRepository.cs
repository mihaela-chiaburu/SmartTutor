using SmartTuror.DataAccess.Data;
using SmartTuror.DataAccess.Repository.IRepository;
using SmartTuror.Models;

namespace SmartTuror.DataAccess.Repository
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