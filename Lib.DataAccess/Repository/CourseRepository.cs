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
    public class CourseRepository : Repository<Course>, ICourseRepository
    {
        private ApplicationDbContext _db;
        public CourseRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Course obj)
        {
            var objFromDb = _db.Courses.FirstOrDefault(u=>u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Title = obj.Title;
                objFromDb.Description = obj.Description;
                objFromDb.CategoryId = obj.CategoryId;
                objFromDb.CourseImages = obj.CourseImages;
                //if (obj.ImageUrl != null)
                //{
                //    objFromDb.ImageUrl = obj.ImageUrl;
                //}
            }
        }
    }
}
