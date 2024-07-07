using BH.DataAccess.Infrastructure.Interface.IRepository;
using BH.Models.InventoryManagement;
using BH.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.DataAccess.Infrastructure.Interface.IInventoryManagementRepository
{
    public interface IStockTransferRepository : IRepository<StockTransfer>
    {
        void Update(StockTransfer obj);
        public IEnumerable<StockTransfer> FilterStock(StockTransferVM stockTransferVM);
        public double GetStockQuantity(int Id, int shopId);
    }
}
