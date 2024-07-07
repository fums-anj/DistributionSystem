using BH.DataAccess.Infrastructure.Interface.IRepository;
using BH.Models.AccountManagement;
using BH.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.DataAccess.Infrastructure.Interface.IAccountManagementRepository
{
    public interface IExpenseRepository : IRepository<Expense>
    {
        void Update(Expense obj);
        public IEnumerable<Expense> FilterExpense(ExpenseVM expenseVM);
    }
}
