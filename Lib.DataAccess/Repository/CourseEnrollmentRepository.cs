using Lib.DataAccess.Data;
using Lib.DataAccess.Repository.IRepository;
using Lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.DataAccess.Repository
{
    public class CourseEnrollmentRepository : Repository<CourseEnrollment>, ICourseEnrollmentRepository
    {
        private ApplicationDbContext _db;
        public CourseEnrollmentRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(CourseEnrollment obj)
        {
            _db.CourseEnrollments.Update(obj);
        }
    }
} 