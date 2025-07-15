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
    public class CourseCategoryRepository : Repository<CourseCategory>, ICourseCategoryRepository
    {
        private ApplicationDbContext _db;
        public CourseCategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(CourseCategory obj)
        {
            _db.CourseCategories.Update(obj);
        }
    }
}
