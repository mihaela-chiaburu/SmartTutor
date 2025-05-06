using Lib.DataAccess.Data;
using Lib.DataAccess.Repository.IRepository;
using Lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Lib.DataAccess.Repository
{
    public class ChapterProgressRepository : Repository<ChapterProgress>, IChapterProgressRepository
    {
        private ApplicationDbContext _db;
        public ChapterProgressRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(ChapterProgress obj)
        {
            _db.ChapterProgresses.Update(obj);
        }
    }
} 