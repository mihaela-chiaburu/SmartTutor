using SmartTuror.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTuror.DataAccess.Repository.IRepository
{
    public interface ICourseCategoryRepository: IRepository<CourseCategory>
    {
        void Update(CourseCategory obj);
    }
}
