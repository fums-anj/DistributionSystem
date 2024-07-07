using BH.DataAccess.Infrastructure.Interface.IRepository;
using BH.Models.ProductManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.DataAccess.Infrastructure.Interface.IShopManagementRepository
{
    public interface IReturnReasonRepository : IRepository<ReturnReason>
    {
        void Update(ReturnReason obj);
    }
}
