using BH.DataAccess.Data;
using BH.DataAccess.Infrastructure.Concrete.Repository;
using BH.DataAccess.Infrastructure.Interface.IAccountManagementRepository;
using BH.Models.AccountManagement;
using BH.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.DataAccess.Infrastructure.Concrete.AccountManagementRepository
{
    public class ExpenseRepository : Repository<Expense>, IExpenseRepository
    {
        private readonly BHContext _db;
        public ExpenseRepository(BHContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Expense obj)
        {
            _db.Expenses.Update(obj);
        }
        public IEnumerable<Expense> FilterExpense(ExpenseVM expenseVM)
        {
            if (expenseVM.Expense != null || expenseVM.KeyWord != null)
            {
                string key = "";
                key = expenseVM.KeyWord;
                if (!string.IsNullOrEmpty(key))
                {
                    expenseVM.ExpenseList = expenseVM.ExpenseList.Where(x => (x.Name ?? "").Contains(key) ||
                                                                            x.ApprovedBy.Contains(key) ||
                                                                            x.CostCenter.Contains(key) ||
                                                                            x.Description.Contains(key));
                }
                if (expenseVM.Expense != null)
                {
                    if (expenseVM.Expense.ExpenseTypeId != 0)
                        expenseVM.ExpenseList = expenseVM.ExpenseList.Where(x => x.ExpenseTypeId == expenseVM.Expense.ExpenseTypeId);
                    if (expenseVM.Expense.CreatedBy != null)
                        expenseVM.ExpenseList = expenseVM.ExpenseList.Where(x => x.CreatedBy == expenseVM.Expense.CreatedBy);
                }
            }
            if (expenseVM.RegDateFrom != null && expenseVM.RegDateTo != null)
            {
                DateTime toDate = (DateTime)expenseVM.RegDateTo;
                expenseVM.RegDateTo = toDate.AddDays(1);
                expenseVM.ExpenseList = expenseVM.ExpenseList.Where(x =>
                    x.CreatedDate >= expenseVM.RegDateFrom &&
                    x.CreatedDate <= expenseVM.RegDateTo
                );
            }
            if (expenseVM.ApprovedDateFrom != null && expenseVM.ApprovedDateTo != null)
            {
                //DateTime toDate = (DateTime)expenseVM.ApprovedDateTo;
                //expenseVM.ApprovedDateTo = toDate.AddDays(1);
                expenseVM.ExpenseList = expenseVM.ExpenseList.Where(x =>
                    x.ApprovedDate >= expenseVM.ApprovedDateFrom &&
                    x.ApprovedDate <= expenseVM.ApprovedDateTo
                );
            }
            return expenseVM.ExpenseList;
        }
    }
}
