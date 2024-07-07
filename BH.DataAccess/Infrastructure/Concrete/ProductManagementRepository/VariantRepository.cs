using BH.DataAccess.Data;
using BH.DataAccess.Infrastructure.Concrete.Repository;
using BH.DataAccess.Infrastructure.Interface.IProductManagementRepository;
using BH.Models.ProductManagement;
using BH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.DataAccess.Infrastructure.Concrete.ProductManagementRepository
{
    public class VariantRepository : Repository<Variant>, IVariantRepository
    {
        private readonly BHContext _db;
        public VariantRepository(BHContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Variant obj)
        {
            var objFromDb = _db.Variants.FirstOrDefault(w => w.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Name = obj.Name;
                objFromDb.VendorSKU = obj.VendorSKU;
                objFromDb.Packing = obj.Packing;
                objFromDb.SKU = obj.SKU;
                objFromDb.PackingSKU = obj.PackingSKU;
                objFromDb.Size = obj.Size;
                if (obj.ImageUrl != null)
                {
                    objFromDb.ImageUrl = obj.ImageUrl;
                }
                objFromDb.UnitOfMeasureId = obj.UnitOfMeasureId;
                objFromDb.PurchasePrice = obj.PurchasePrice;
                objFromDb.ListPrice = obj.ListPrice;
                objFromDb.WholesalePrice = obj.WholesalePrice;
                objFromDb.StorageLocation = obj.StorageLocation;
                objFromDb.ShopProductId = obj.ShopProductId;
                objFromDb.TypeColor = obj.TypeColor;
                objFromDb.LocationId = obj.LocationId;
                objFromDb.SupplierId = obj.SupplierId;
                objFromDb.IsDisable = obj.IsDisable;
                objFromDb.IsDeleted = obj.IsDeleted;
                objFromDb.ModifiedBy = obj.ModifiedBy;
                objFromDb.ModifiedDate = obj.ModifiedDate;
                objFromDb.IsWeight = obj.IsWeight;
                objFromDb.IsWithoutStock = obj.IsWithoutStock;
                objFromDb.LowStockWarningQuantity = obj.LowStockWarningQuantity;

            }
        }
        public string GenerateSKU(string shopName)
        {
            GlobalNumber variable = _db.GlobalNumbers.Where(x => x.Name == "globalnumber").FirstOrDefault();
            variable.SKU++;
            _db.SaveChanges();
            string SKU = DateTime.UtcNow.Year % 100 + DateTime.UtcNow.Month.ToString("d2") + DateTime.UtcNow.Day.ToString("d2") + variable.SKU;
            return SKU + shopName.Split(" ")[0].Trim();
        }
        public string GenerateCode()
        {
            GlobalNumber variable = _db.GlobalNumbers.Where(x => x.Name == "globalnumber").FirstOrDefault();
            variable.VariantCode++;
            _db.SaveChanges();
            string Code = string.Empty;
            if (variable.VariantCode < 10)
            {
                Code = "000" + variable.VariantCode;
            }
            else
            {
                if (variable.VariantCode < 100)
                {
                    Code = "00" + variable.VariantCode;
                }
                else
                {
                    if (variable.VariantCode < 1000)
                    {
                        Code = "0" + variable.VariantCode;
                    }
                    else
                    {
                        Code = variable.VariantCode.ToString();
                    }
                }
            }
            while (_db.Variants.FirstOrDefault(x => x.VariantCode == Code) != null)
            {
                Code = GenerateCode();
            }
            return Code;
        }
    }
}
