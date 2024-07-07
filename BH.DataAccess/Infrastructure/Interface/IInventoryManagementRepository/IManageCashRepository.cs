using BH.DataAccess.Infrastructure.Interface.IRepository;
using BH.Models.InventoryManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.DataAccess.Infrastructure.Interface.IInventoryManagementRepository
{
    public interface IManageCashRepository : IRepository<ManageCash>
    {
        void Update(ManageCash obj);
    }
}
