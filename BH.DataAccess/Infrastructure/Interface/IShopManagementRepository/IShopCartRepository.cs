using BH.DataAccess.Infrastructure.Interface.IRepository;
using BH.Models.SaleManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.DataAccess.Infrastructure.Interface.IShopManagementRepository
{
    public interface IShopCartRepository : IRepository<ShopCart>
    {
        double IncrementCount(ShopCart shopCart, double count);
        double DecrementCount(ShopCart shopCart, double count);
    }
}
