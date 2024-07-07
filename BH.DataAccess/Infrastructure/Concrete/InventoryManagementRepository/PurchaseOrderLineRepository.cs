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
    public class PurchaseOrderLineRepository : Repository<PurchaseOrderLine>, IPurchaseOrderLineRepository
    {
        private readonly BHContext _db;
        public PurchaseOrderLineRepository(BHContext db) : base(db)
        {
            _db = db;
        }
        public void Update(PurchaseOrderLine obj)
        {
            _db.PurchaseOrderLines.Update(obj);
        }
    }
}
