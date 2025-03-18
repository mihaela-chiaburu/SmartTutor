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
    public class QuestionRepository : Repository<Question>, IQuestionRespository
    {
        private ApplicationDbContext _db;
        public QuestionRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Question obj)
        {
            _db.Questions.Update(obj);
        }
    }
}
