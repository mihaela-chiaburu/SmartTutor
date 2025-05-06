using Lib.DataAccess.Repository.IRepository;
using Lib.Models;

namespace Lib.DataAccess.Repository.IRepository
{
    public interface IRealTimePerformanceRepository : IRepository<RealTimePerformance>
    {
        void Update(RealTimePerformance obj);
    }
} 