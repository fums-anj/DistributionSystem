using BH.DataAccess.Data;
using BH.DataAccess.Infrastructure.Concrete.Repository;
using BH.DataAccess.Infrastructure.Interface.IInventoryManagementRepository;
using BH.Models.InventoryManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.DataAccess.Infrastructure.Concrete.InventoryManagementRepository
{
    public class ManageCashRepository : Repository<ManageCash>, IManageCashRepository
    {
        private readonly BHContext _db;
        public ManageCashRepository(BHContext db) : base(db)
        {
            _db = db;
        }
        public void Update(ManageCash obj)
        {
            _db.ManageCash.Update(obj);
        }
    }
}
