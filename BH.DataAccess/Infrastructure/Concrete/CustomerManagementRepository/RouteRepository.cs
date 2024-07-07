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
using BH.DataAccess.Infrastructure.Interface.ICustomerManagementRepository;

namespace BH.DataAccess.Infrastructure.Concrete.CustomerManagementRepository
{
    public class RouteRepository : Repository<CustomerRoute>, IRouteRepository
    {
        private readonly BHContext _db;
        public RouteRepository(BHContext db) : base(db)
        {
            _db = db;
        }
        public void Update(CustomerRoute obj)
        {
            var objFromDb = _db.CustomerRoutes.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.RouteName = obj.RouteName;
                objFromDb.RouteDay = obj.RouteDay;
                objFromDb.RouteDetail = obj.RouteDetail;
                objFromDb.ModifiedBy = obj.ModifiedBy;
                objFromDb.ModifiedDate = obj.ModifiedDate;
                objFromDb.IsDeleted = obj.IsDeleted;
                objFromDb.IsDisable = obj.IsDisable;
                objFromDb.SalemanId = obj.SalemanId;
            }
        }
    }
}
