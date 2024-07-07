using BH.DataAccess.Data;
using BH.DataAccess.Infrastructure.Concrete.Repository;
using BH.DataAccess.Infrastructure.Interface.ISaleManagementRepository;
using BH.Models.SaleManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.DataAccess.Infrastructure.Concrete.SaleManagementRepository
{
    public class CreditNoteRepository : Repository<CreditNote>, ICreditNoteRepository
    {
        private readonly BHContext _db;
        public CreditNoteRepository(BHContext db) : base(db)
        {
            _db = db;
        }
        public void Update(CreditNote obj)
        {
            _db.CreditNotes.Update(obj);
        }
    }
}
