using BH.DataAccess.Data;
using BH.DataAccess.Infrastructure.Concrete.Repository;
using BH.DataAccess.Infrastructure.Interface.IShopManagementRepository;
using BH.Models.SaleManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.DataAccess.Infrastructure.Concrete.ShopManagementRepository
{
    public class ShopCartRepository : Repository<ShopCart>, IShopCartRepository
    {
        private BHContext _db;

        public ShopCartRepository(BHContext db) : base(db)
        {
            _db = db;
        }

        public double DecrementCount(ShopCart shopCart, double count)
        {
            shopCart.Quantity -= count;
            return shopCart.Quantity;
        }

        public double IncrementCount(ShopCart shopCart, double count)
        {
            shopCart.Quantity += count;
            return shopCart.Quantity;
        }
    }
}
