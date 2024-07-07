using BH.DataAccess.Data;
using BH.DataAccess.Infrastructure.Concrete.Repository;
using BH.DataAccess.Infrastructure.Interface.IAccountManagementRepository;
using BH.Models.AccountManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.DataAccess.Infrastructure.Concrete.AccountManagementRepository
{
    public class ExpenseYearRepository : Repository<ExpenseYear>, IExpenseYearRepository
    {
        private readonly BHContext _db;
        public ExpenseYearRepository(BHContext db) : base(db)
        {
            _db = db;
        }
        public void Update(ExpenseYear obj)
        {
            _db.ExpenseYears.Update(obj);
        }
    }
}
