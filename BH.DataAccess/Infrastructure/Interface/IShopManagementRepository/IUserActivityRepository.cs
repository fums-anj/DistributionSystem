using BH.DataAccess.Infrastructure.Interface.IRepository;
using BH.Models.ShopManagement;

namespace BH.DataAccess.Infrastructure.Interface.IShopManagementRepository
{
    public interface IUserActivityRepository : IRepository<UserActivity>
    {
        void Update(UserActivity obj);
    }
}
