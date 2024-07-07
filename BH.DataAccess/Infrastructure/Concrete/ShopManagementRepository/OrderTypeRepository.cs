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
    public class OrderTypeRepository : Repository<OrderType>, IOrderTypeRepository
    {
        private readonly BHContext _db;
        public OrderTypeRepository(BHContext db) : base(db)
        {
            _db = db;
        }
        public void Update(OrderType obj)
        {
            _db.OrderTypes.Update(obj);
        }
    }
}
