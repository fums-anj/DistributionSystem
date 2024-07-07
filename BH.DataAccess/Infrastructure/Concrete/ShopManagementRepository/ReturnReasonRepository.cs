using BH.DataAccess.Data;
using BH.DataAccess.Infrastructure.Concrete.Repository;
using BH.DataAccess.Infrastructure.Interface.IShopManagementRepository;
using BH.Models.ProductManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.DataAccess.Infrastructure.Concrete.ShopManagementRepository
{
    public class ReturnReasonRepository : Repository<ReturnReason>, IReturnReasonRepository
    {
        private readonly BHContext _db;
        public ReturnReasonRepository(BHContext db) : base(db)
        {
            _db = db;
        }
        public void Update(ReturnReason obj)
        {
            _db.ReturnReasons.Update(obj);
        }
    }
}
