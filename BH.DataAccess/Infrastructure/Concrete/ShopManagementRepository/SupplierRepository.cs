using BH.DataAccess.Data;
using BH.DataAccess.Infrastructure.Concrete.Repository;
using BH.DataAccess.Infrastructure.Interface.IShopManagementRepository;
using BH.Models.ShopManagement;

namespace BH.DataAccess.Infrastructure.Concrete.ShopManagementRepository
{
    public class SupplierRepository : Repository<Supplier>, ISupplierRepository
    {
        private readonly BHContext _db;
        public SupplierRepository(BHContext db) : base(db) //here base class is Repository which aspects db parameter so : base(db) provoid db to base class
        {
            _db = db;
        }
        public void Update(Supplier obj)
        {
            _db.Suppliers.Update(obj);
        }
    }
}
