using Lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Lib.DataAccess.Repository.IRepository
{
    public interface IChapterProgressRepository : IRepository<ChapterProgress>
    {
        void Update(ChapterProgress obj);
    }
} 