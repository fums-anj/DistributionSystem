using BH.DataAccess.Infrastructure.Interface.IRepository;
using BH.Models.ShopManagement;

namespace BH.DataAccess.Infrastructure.Interface.IShopManagementRepository
{
    public interface IStatusRepository : IRepository<Status>
    {
        void Update(Status obj);
    }
}
