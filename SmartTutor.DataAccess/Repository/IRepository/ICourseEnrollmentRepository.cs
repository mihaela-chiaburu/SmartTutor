using SmartTuror.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTuror.DataAccess.Repository.IRepository
{
    public interface ICourseEnrollmentRepository : IRepository<CourseEnrollment>
    {
        void Update(CourseEnrollment obj);
    }
} 