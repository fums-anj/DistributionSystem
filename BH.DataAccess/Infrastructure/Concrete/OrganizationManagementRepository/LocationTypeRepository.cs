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
    public class LocationTypeRepository : Repository<LocationType>, ILocationTypeRepository
    {
        private readonly BHContext _db;
        public LocationTypeRepository(BHContext db) : base(db)
        {
            _db = db;
        }
        public void Update(LocationType obj)
        {
            _db.LocationTypes.Update(obj);
        }
    }
}
