using BH.DataAccess.Data;
using BH.DataAccess.Infrastructure.Concrete.Repository;
using BH.DataAccess.Infrastructure.Interface.IShopManagementRepository;
using BH.Models.ProductManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.DataAccess.Infrastructure.Concrete.ShopManagementRepository
{
    public class ItemConditionRepository : Repository<ItemCondition>, IItemConditionRepository
    {
        private readonly BHContext _db;
        public ItemConditionRepository(BHContext db) : base(db)
        {
            _db = db;
        }
        public void Update(ItemCondition obj)
        {
            _db.ItemConditions.Update(obj);
        }
    }
}
