using BH.DataAccess.Data;
using BH.DataAccess.Infrastructure.Concrete.Repository;
using BH.DataAccess.Infrastructure.Interface.ISaleManagementRepository;
using BH.Models.SaleManagement;
using BH.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.DataAccess.Infrastructure.Concrete.SaleManagementRepository
{
    public class SaleOrderRepository : Repository<SaleOrder>, ISaleOrderRepository
    {
        private readonly BHContext _db;
        public SaleOrderRepository(BHContext db) : base(db)
        {
            _db = db;
        }
        public void Update(SaleOrder obj)
        {
            _db.SaleOrders.Update(obj);
        }
        public IEnumerable<SaleOrder> FilterSaleOrder(SaleOrderVM saleOrderVM)
        {
            if (saleOrderVM.SaleOrder != null)
            {
                if (saleOrderVM.SaleOrder.CreatedBy != null)
                    saleOrderVM.SaleOrderList = saleOrderVM.SaleOrderList.Where(x => x.CreatedBy == saleOrderVM.SaleOrder.CreatedBy);
                if (saleOrderVM.SaleOrder.Id != 0)
                    saleOrderVM.SaleOrderList = saleOrderVM.SaleOrderList.Where(x => x.Id == saleOrderVM.SaleOrder.Id);
            }
            if (saleOrderVM.RegDateFrom != null && saleOrderVM.RegDateTo != null)
            {
                DateTime toDate = (DateTime)saleOrderVM.RegDateTo;
                saleOrderVM.RegDateTo = toDate.AddDays(1);
                saleOrderVM.SaleOrderList = saleOrderVM.SaleOrderList.Where(x =>
                    x.CreatedDate >= saleOrderVM.RegDateFrom &&
                    x.CreatedDate <= saleOrderVM.RegDateTo
                );
            }
            return saleOrderVM.SaleOrderList;
        }
    }
}
