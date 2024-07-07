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
    public class ExpenseMonthRepository : Repository<ExpenseMonth>, IExpenseMonthRepository
    {
        private readonly BHContext _db;
        public ExpenseMonthRepository(BHContext db) : base(db)
        {
            _db = db;
        }
        public void Update(ExpenseMonth obj)
        {
            _db.ExpenseMonths.Update(obj);
        }
    }
}
