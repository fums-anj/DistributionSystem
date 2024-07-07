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
    public interface IAccountReceivableRepository : IRepository<AccountReceivable>
    {
        void Update(AccountReceivable obj);
        void accountReceivableUpdate(double totalReceiveable, int saleorderid, int shopcustomerid, string userid, int? shopId, double Receamount, DateTime? orderDate);
        AccountReceivable getLastAccountReceivable(int shopcustomerid);
        IEnumerable<AccountReceivable> FilterAccountReceivable(AccountReceivableVM accountReceivableVM);
    }
}
