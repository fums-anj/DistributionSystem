using BH.DataAccess.Data;
using BH.DataAccess.Infrastructure.Concrete.Repository;
using BH.DataAccess.Infrastructure.Interface.IAccountManagementRepository;
using BH.Models.AccountManagement;
using BH.Models.OrganizationManagement;
using BH.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace BH.DataAccess.Infrastructure.Concrete.AccountManagementRepository
{
    public class AccountPayableRepository : Repository<AccountPayable>, IAccountPayableRepository
    {
        private readonly BHContext _db;
        public AccountPayableRepository(BHContext db) : base(db)
        {
            _db = db;
        }
        public void Update(AccountPayable obj)
        {
            _db.AccountPayables.Update(obj);
        }

        
    }
}

