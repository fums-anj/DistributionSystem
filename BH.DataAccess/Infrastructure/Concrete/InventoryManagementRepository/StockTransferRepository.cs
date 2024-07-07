using BH.DataAccess.Data;
using BH.DataAccess.Infrastructure.Concrete.Repository;
using BH.DataAccess.Infrastructure.Interface.IInventoryManagementRepository;
using BH.Models.InventoryManagement;
using BH.Models.ViewModels;
using BH.Utility;
using Microsoft.EntityFrameworkCore;

namespace BH.DataAccess.Infrastructure.Concrete.InventoryManagementRepository
{
    public class StockTransferRepository : Repository<StockTransfer>, IStockTransferRepository
    {
        private readonly BHContext _db;
        public StockTransferRepository(BHContext db) : base(db)
        {
            _db = db;
        }
        public void Update(StockTransfer stockTransferobj)
        {
            _db.StockTransfers.Update(stockTransferobj);
        }
        public IEnumerable<StockTransfer> FilterStock(StockTransferVM stockTransferVM)
        {
            if (stockTransferVM.StockTransfer != null)
            {
                string key = "";
                key = stockTransferVM.StockTransfer.StockType;
                if (!string.IsNullOrEmpty(key))
                {
                    stockTransferVM.stockTransferList = stockTransferVM.stockTransferList.Where(x => (x.Variant.PackingSKU ?? "").Contains(key) ||
                                                                                        x.Variant.Name.Contains(key) ||
                                                                                        (x.Variant.VariantCode ?? "").Contains(key) ||
                                                                                        (x.ShopCustomer.CustomerCode ?? "").Contains(key) ||
                                                                                        (x.ShopCustomer.CustomerName ?? "").Contains(key) ||
                                                                                        (x.StockType ?? "").Contains(key) ||
                                                                                        (x.Variant.SKU ?? "").Contains(key));
                }
                if (stockTransferVM.StockTransfer.SupplierId != null)
                    stockTransferVM.stockTransferList = stockTransferVM.stockTransferList.Where(x => x.Variant.SupplierId == stockTransferVM.StockTransfer.SupplierId);
                if (stockTransferVM.StockTransfer.VariantId != null && stockTransferVM.StockTransfer.VariantId != 0)
                    stockTransferVM.stockTransferList = stockTransferVM.stockTransferList.Where(x => x.VariantId == stockTransferVM.StockTransfer.VariantId);
                if (stockTransferVM.StockTransfer.CreatedBy != null)
                    stockTransferVM.stockTransferList = stockTransferVM.stockTransferList.Where(x => x.CreatedBy == stockTransferVM.StockTransfer.CreatedBy);
            }
            if (stockTransferVM.RegDateFrom != null && stockTransferVM.RegDateTo != null)
            {
                DateTime toDate = stockTransferVM.RegDateTo.Value.Date.AddDays(1);
                stockTransferVM.stockTransferList = stockTransferVM.stockTransferList.Where(x =>
                    x.CreatedDate >= stockTransferVM.RegDateFrom.Value &&
                    x.CreatedDate < toDate
                );
            }
            return stockTransferVM.stockTransferList;
        }
        public double GetStockQuantity(int Id, int shopId)
        {
            ShopCartVM shopCartVM = new ShopCartVM();
            shopCartVM.ShopCart = new();
            shopCartVM.StockTransferList = _db.StockTransfers.Where(x => x.VariantId == Id && x.ShopId == shopId && x.IsDeleted != true && (x.StockType == SDStockType.StockType_Purchase || x.StockType == SDStockType.StockType_StockReturn) && x.ReceivedQty != x.StockSoldQty).Include(x => x.Variant).Include(x => x.Variant.ShopProduct);
            shopCartVM.AvailableQty = shopCartVM.StockTransferList.Sum(x => x.ReceivedQty - x.StockSoldQty);
            shopCartVM.ListCart = _db.ShopCarts.Where(u => u.VariantId == Id).Include(x => x.Variant);
            if (shopCartVM.ListCart.Count() > 0)
            {
                shopCartVM.AvailableQty -= shopCartVM.ListCart.Sum(s => s.Quantity);
            }
            return shopCartVM.AvailableQty;
        }
    }
}
