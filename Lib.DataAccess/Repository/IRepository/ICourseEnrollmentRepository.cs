using Lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.DataAccess.Repository.IRepository
{
    public interface ICourseEnrollmentRepository : IRepository<CourseEnrollment>
    {
        void Update(CourseEnrollment obj);
    }
} 