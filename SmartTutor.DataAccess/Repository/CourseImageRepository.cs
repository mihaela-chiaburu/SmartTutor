using SmartTuror.DataAccess.Data;
using SmartTuror.DataAccess.Repository.IRepository;
using SmartTuror.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTuror.DataAccess.Repository
{
    public class CourseImageRepository : Repository<CourseImage>, ICourseImageRepository
    {
        private ApplicationDbContext _db;
        public CourseImageRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(CourseImage obj)
        {
            _db.CourseImages.Update(obj);
        }
    }
}
