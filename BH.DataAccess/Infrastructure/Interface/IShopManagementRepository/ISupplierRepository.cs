using BH.DataAccess.Infrastructure.Interface.IRepository;
using BH.Models.ShopManagement;

namespace BH.DataAccess.Infrastructure.Interface.IShopManagementRepository
{
    public interface ISupplierRepository : IRepository<Supplier>
    {
        void Update(Supplier obj);
    }
}
