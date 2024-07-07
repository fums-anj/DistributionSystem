using BH.DataAccess.Data;
using BH.DataAccess.Infrastructure.Concrete.Repository;
using BH.DataAccess.Infrastructure.Interface.IProductManagementRepository;
using BH.Models.ProductManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.DataAccess.Infrastructure.Concrete.ProductManagementRepository
{
    public class ShopProductRepository : Repository<ShopProduct>, IShopProductRepository
    {
        private readonly BHContext _db;
        public ShopProductRepository(BHContext db) : base(db)
        {
            _db = db;
        }
        public void Update(ShopProduct obj)
        {
            var objFromDb = _db.ShopProducts.FirstOrDefault(w => w.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Name = obj.Name;
                objFromDb.ProductAcronym = obj.ProductAcronym;
                objFromDb.Brand = obj.Brand;
                objFromDb.CatalogId = obj.CatalogId;
                objFromDb.LocationId = obj.LocationId;
                objFromDb.IsDisable = obj.IsDisable;
                objFromDb.IsDeleted = obj.IsDeleted;
                objFromDb.ModifiedBy = obj.ModifiedBy;
                objFromDb.ModifiedDate = obj.ModifiedDate;

            }
        }
    }
}
