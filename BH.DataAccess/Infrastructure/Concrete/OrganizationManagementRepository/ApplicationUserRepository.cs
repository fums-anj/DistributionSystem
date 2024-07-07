using BH.DataAccess.Data;
using BH.DataAccess.Infrastructure.Concrete.Repository;
using BH.DataAccess.Infrastructure.Interface.IOrganizationManagementRepository;
using BH.Models.OrganizationManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.DataAccess.Infrastructure.Concrete.OrganizationManagementRepository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private BHContext _db;

        public ApplicationUserRepository(BHContext db) : base(db)
        {
            _db = db;
        }

    }
}
