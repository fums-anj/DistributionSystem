using BH.DataAccess.Infrastructure.Interface.IRepository;
using BH.Models.OrganizationManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.DataAccess.Infrastructure.Interface.IOrganizationManagementRepository
{
    public interface ILocationTypeRepository : IRepository<LocationType>
    {
        void Update(LocationType obj);
    }
}
