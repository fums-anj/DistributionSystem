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
    public class ExpenseTypeRepository : Repository<ExpenseType>, IExpenseTypeRepository
    {
        private readonly BHContext _db;
        public ExpenseTypeRepository(BHContext db) : base(db)
        {
            _db = db;
        }
        public void Update(ExpenseType obj)
        {
            _db.ExpenseTypes.Update(obj);
        }
    }
}
