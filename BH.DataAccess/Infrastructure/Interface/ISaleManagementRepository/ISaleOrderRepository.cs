using BH.DataAccess.Infrastructure.Interface.IRepository;
using BH.Models.SaleManagement;
using BH.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.DataAccess.Infrastructure.Interface.ISaleManagementRepository
{
    public interface ISaleOrderRepository : IRepository<SaleOrder>
    {
        void Update(SaleOrder obj);
        IEnumerable<SaleOrder> FilterSaleOrder(SaleOrderVM saleOrderVM);
    }
}
