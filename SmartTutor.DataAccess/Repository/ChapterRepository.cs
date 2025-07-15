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
    public class ChapterRepository : Repository<Chapter>, IChapterRepository
    {
        private ApplicationDbContext _db;
        public ChapterRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Chapter obj)
        {
            _db.Chapters.Update(obj);
        }
    }
}
