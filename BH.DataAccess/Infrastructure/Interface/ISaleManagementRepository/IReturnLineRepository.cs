using BH.DataAccess.Infrastructure.Interface.IRepository;
using BH.Models.SaleManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.DataAccess.Infrastructure.Interface.ISaleManagementRepository
{
    public interface IReturnLineRepository : IRepository<ReturnLine>
    {
        void Update(ReturnLine obj);
    }
}
