using BH.DataAccess.Infrastructure.Interface.IRepository;
using BH.Models.CustomerManagement;

namespace BH.DataAccess.Infrastructure.Interface.ICustomerManagementRepository
{
    public interface ICustomerRepository : IRepository<ShopCustomer>
    {
        void Update(ShopCustomer obj);
        string GenerateCustomerCode();
    }
}
