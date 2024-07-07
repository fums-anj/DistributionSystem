using BH.DataAccess.Data;
using BH.DataAccess.Infrastructure.Concrete.Repository;
using BH.DataAccess.Infrastructure.Interface.IShopManagementRepository;
using BH.Models.ShopManagement;

namespace BH.DataAccess.Infrastructure.Concrete.ShopManagementRepository
{
    public class StatusRepository : Repository<Status>, IStatusRepository
    {
        private readonly BHContext _db;
        public StatusRepository(BHContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Status obj)
        {
            _db.Statuses.Update(obj);
        }
    }
}
