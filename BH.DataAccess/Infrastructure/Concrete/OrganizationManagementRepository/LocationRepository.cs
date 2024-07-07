using BH.DataAccess.Data;
using BH.DataAccess.Infrastructure.Concrete.Repository;
using BH.DataAccess.Infrastructure.Interface.IOrganizationManagementRepository;
using BH.Models.OrganizationManagement;
using BH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.DataAccess.Infrastructure.Concrete.OrganizationManagementRepository
{
    public class LocationRepository : Repository<Location>, ILocationRepository
    {
        private readonly BHContext _db;
        public LocationRepository(BHContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Location obj)
        {
            var objFromDb = _db.Locations.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Name = obj.Name;
                objFromDb.Incharge = obj.Incharge;
                objFromDb.Address = obj.Address;
                objFromDb.ModifiedBy = obj.ModifiedBy;
                objFromDb.ModifiedDate = obj.ModifiedDate;
                objFromDb.IsDeleted = obj.IsDeleted;
                objFromDb.IsDisable = obj.IsDisable;
                objFromDb.LocationTypeId = obj.LocationTypeId;
            }
        }
        public string GenerateProfitCenter()
        {
            GlobalNumber variable = _db.GlobalNumbers.Where(x => x.Name == "globalnumber").FirstOrDefault();
            variable.ProficitCenterNum++;
            _db.SaveChanges();
            string ProficitCenter = DateTime.UtcNow.Year % 100 + DateTime.UtcNow.Month.ToString("d2") + DateTime.UtcNow.Day.ToString("d2") + variable.CostCenterNum;
            return ProficitCenter + "PC";
        }
        public string GenerateCostCenter()
        {
            GlobalNumber variable = _db.GlobalNumbers.Where(x => x.Name == "globalnumber").FirstOrDefault();
            variable.CostCenterNum++;
            _db.SaveChanges();
            string CostCenter = DateTime.UtcNow.Year % 100 + DateTime.UtcNow.Month.ToString("d2") + DateTime.UtcNow.Day.ToString("d2") + variable.CostCenterNum;
            return CostCenter + "CC";
        }
    }
}
