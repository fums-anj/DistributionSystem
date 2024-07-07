using BH.DataAccess.Data;
using BH.DataAccess.Infrastructure.Concrete.Repository;
using BH.DataAccess.Infrastructure.Interface.IShopManagementRepository;
using BH.Models.ShopManagement;

namespace BH.DataAccess.Infrastructure.Concrete.ShopManagementRepository
{
    public class UserActivityRepository : Repository<UserActivity>, IUserActivityRepository
    {
        private readonly BHContext _db;
        public UserActivityRepository(BHContext db) : base(db)
        {
            _db = db;
        }
        public void Update(UserActivity obj)
        {
            _db.UserActivities.Update(obj);
        }
    }
}
