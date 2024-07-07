using BH.DataAccess.Infrastructure.Interface.IRepository;
using BH.Models.OrganizationManagement;

namespace BH.DataAccess.Infrastructure.Interface.ICustomerManagementRepository
{
    public interface IRouteRepository : IRepository<CustomerRoute>
    {
        void Update(CustomerRoute obj);
    }
}
