using BH.DataAccess.Data;
using BH.DataAccess.Infrastructure.Concrete.Repository;
using BH.DataAccess.Infrastructure.Interface.IAccountManagementRepository;
using BH.DataAccess.Infrastructure.Interface.IRepository;
using BH.Models.AccountManagement;
using BH.Models.CustomerManagement;
using BH.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.DataAccess.Infrastructure.Concrete.AccountManagementRepository
{
    public class AccountReceivableRepository : Repository<AccountReceivable>, IAccountReceivableRepository
    {
        private readonly BHContext _db;
        public AccountReceivableRepository(BHContext db) : base(db)
        {
            _db = db;
        }
        public void Update(AccountReceivable obj)
        {
            _db.AccountReceivables.Update(obj);
        }

        //this method is used to update the account receivable when a sale order is created
        public void accountReceivableUpdate(double totalReceiveable, int saleorderid, int shopcustomerid, string userid, int? shopId, double Receamount, DateTime? orderDate)
        {
            AccountReceivable accountReceivable = new();
            AccountReceivable accountRecDB = _db.AccountReceivables.OrderBy(u => u.Id).LastOrDefault(u => u.ShopCustomerId == shopcustomerid);
            ShopCustomer shopCustomerDB = _db.ShopCustomers.OrderBy(u => u.Id).LastOrDefault(u => u.Id == shopcustomerid);
            accountReceivable.TotalReceivable = totalReceiveable;
            accountReceivable.ReceivedAmount = Receamount;
            if (saleorderid != 0)
            {
                accountReceivable.SaleOrderId = saleorderid;
            }
            accountReceivable.ShopCustomerId = shopcustomerid;
            if (Receamount > 0)
            {
                if (orderDate != null)
                {
                    accountReceivable.ReceivedDate = orderDate;
                }
                else
                {
                    accountReceivable.ReceivedDate = DateTime.Now;
                }
            }
            if (orderDate != null)
            {
                accountReceivable.CreatedDate = orderDate;
            }
            else
            {
                accountReceivable.CreatedDate = DateTime.Now;
            }
            accountReceivable.CreatedBy = userid;
            accountReceivable.ShopId = shopId;
            accountReceivable.IsDeleted = false;
            accountReceivable.IsDisable = false;
            if (shopCustomerDB.Balance != null)
            {
                shopCustomerDB.Balance = shopCustomerDB.Balance + totalReceiveable - Receamount;
            }
            else
            {
                shopCustomerDB.Balance = totalReceiveable - Receamount;
            }
            _db.AccountReceivables.Add(accountReceivable);
            _db.SaveChanges();
        }

        public AccountReceivable getLastAccountReceivable(int shopcustomerid)
        {
            return _db.AccountReceivables.OrderBy(u => u.Id).LastOrDefault(u => u.ShopCustomerId == shopcustomerid);
        }

        public IEnumerable<AccountReceivable> FilterAccountReceivable(AccountReceivableVM accountReceivableVM)
        {
            if (accountReceivableVM.User.ShopId == 0) return Enumerable.Empty<AccountReceivable>();  //this will return empty list of AccountReceivabe when ShopId is 0

            accountReceivableVM.RegDateTo ??= DateTime.Now;

            var query = _db.AccountReceivables
                           .Include(x => x.ShopCustomer)
                           .Include(x => x.ShopCustomer.CustomerRoute)
                           .Where(x => x.ShopId == accountReceivableVM.User.ShopId &&
                                       !x.IsDisable &&
                                       !x.IsDeleted &&
                                       x.ShopCustomerId != null);

            if (accountReceivableVM.routeId != 0)
            {
                query = query.Where(x => x.ShopCustomer.CustomerRouteId == accountReceivableVM.routeId);
            }

            if (accountReceivableVM.RegDateFrom != null)
            {
                var regDateTo = accountReceivableVM.RegDateTo.Value.AddDays(1);
                query = query.Where(x => x.CreatedDate >= accountReceivableVM.RegDateFrom && x.CreatedDate <= regDateTo);
            }

            accountReceivableVM.AccountReceivableList = query.ToList();
            return accountReceivableVM.AccountReceivableList;
        }


        //Above is optimized by chattgpt

        //public IEnumerable<AccountReceivable> FilterAccountReceivable(AccountReceivableVM accountReceivableVM)
        //{
        //    if (accountReceivableVM.User.ShopId != 0)
        //    {
        //        if (accountReceivableVM.RegDateTo == null)
        //        {
        //            accountReceivableVM.RegDateTo = DateTime.Now;
        //        }
        //        if (accountReceivableVM.RegDateFrom != null && accountReceivableVM.routeId != 0 && accountReceivableVM.routeId != null)
        //        {
        //            accountReceivableVM.AccountReceivableList = _db.AccountReceivables.Include(x => x.ShopCustomer).Include(x => x.ShopCustomer.CustomerRoute).Where(x =>
        //                                                                                          x.ShopId == accountReceivableVM.User.ShopId &&
        //                                                                                          x.IsDisable != true &&
        //                                                                                          x.IsDeleted != true &&
        //                                                                                          x.ShopCustomerId != null &&
        //                                                                                          x.ShopCustomer.CustomerRouteId == accountReceivableVM.routeId &&
        //                                                                                          x.CreatedDate >= accountReceivableVM.RegDateFrom &&
        //                                                                                          x.CreatedDate <= accountReceivableVM.RegDateTo + TimeSpan.FromDays(1)).ToList();
        //        }
        //        else if (accountReceivableVM.routeId != 0 && accountReceivableVM.routeId != null)
        //        {
        //            accountReceivableVM.AccountReceivableList = _db.AccountReceivables.Include(x => x.ShopCustomer).Include(x => x.ShopCustomer.CustomerRoute).Where(x =>
        //                                                                                          x.ShopId == accountReceivableVM.User.ShopId &&
        //                                                                                          x.IsDisable != true &&
        //                                                                                          x.IsDeleted != true &&
        //                                                                                          x.ShopCustomerId != null &&
        //                                                                                          x.ShopCustomer.CustomerRouteId == accountReceivableVM.routeId).ToList();
        //        }
        //        else if (accountReceivableVM.RegDateFrom != null)
        //        {

        //            accountReceivableVM.AccountReceivableList = _db.AccountReceivables.Include(x => x.ShopCustomer).Include(x => x.ShopCustomer.CustomerRoute).Where(x =>
        //                                                                                          x.ShopId == accountReceivableVM.User.ShopId &&
        //                                                                                          x.IsDisable != true &&
        //                                                                                          x.IsDeleted != true &&
        //                                                                                          x.ShopCustomerId != null &&
        //                                                                                          x.CreatedDate >= accountReceivableVM.RegDateFrom &&
        //                                                                                          x.CreatedDate <= accountReceivableVM.RegDateTo + TimeSpan.FromDays(1)).ToList();
        //        }
        //        else
        //        {
        //            accountReceivableVM.AccountReceivableList = _db.AccountReceivables.Include(x => x.ShopCustomer).Include(x => x.ShopCustomer.CustomerRoute).Where(x =>
        //                                                                                          x.ShopId == accountReceivableVM.User.ShopId &&
        //                                                                                          x.IsDisable != true &&
        //                                                                                          x.IsDeleted != true &&
        //                                                                                          x.ShopCustomerId != null).ToList();
        //        }
        //    }
        //    return accountReceivableVM.AccountReceivableList;
        //}
    }
}
