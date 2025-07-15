using SmartTuror.DataAccess.Repository.IRepository;
using SmartTuror.Models;

namespace SmartTuror.DataAccess.Repository.IRepository
{
    public interface IRealTimePerformanceRepository : IRepository<RealTimePerformance>
    {
        void Update(RealTimePerformance obj);
    }
} 