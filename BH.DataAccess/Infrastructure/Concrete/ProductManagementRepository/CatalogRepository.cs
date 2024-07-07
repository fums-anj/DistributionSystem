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
    public class CatalogRepository : Repository<Catalog>, ICatalogRepository
    {
        private readonly BHContext _db;
        public CatalogRepository(BHContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Catalog obj)
        {
            var objFromDb = _db.Catalogs.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Name = obj.Name;
                objFromDb.LocationId = obj.LocationId;
                objFromDb.ModifiedBy = obj.ModifiedBy;
                objFromDb.ModifiedDate = obj.ModifiedDate;
                objFromDb.IsDisable = obj.IsDisable;
                objFromDb.IsDeleted = obj.IsDeleted;
            }
        }
    }
}
