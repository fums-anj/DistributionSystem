using BH.DataAccess.Data;
using BH.DataAccess.Infrastructure.Concrete.Repository;
using BH.DataAccess.Infrastructure.Interface.IAccountManagementRepository;
using BH.Models.AccountManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.DataAccess.Infrastructure.Concrete.AccountManagementRepository
{
    public class ReceiptVoucherRepository : Repository<ReceiptVoucher>, IReceiptVoucherRepository
    {
        private readonly BHContext _db;
        public ReceiptVoucherRepository(BHContext db) : base(db)
        {
            _db = db;
        }
        public void Update(ReceiptVoucher obj)
        {
            _db.ReceiptVouchers.Update(obj);
        }
    }
}
