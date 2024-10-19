using BH.DataAccess.Data;
using BH.DataAccess.Infrastructure.Concrete.Repository;
using BH.DataAccess.Infrastructure.Interface.IProductManagementRepository;
using BH.Models.ProductManagement;

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

			if (objFromDb == null) return;

			// Map the simple properties
			objFromDb.Name = obj.Name;
			objFromDb.VendorSKU = obj.VendorSKU;
			objFromDb.Packing = obj.Packing;
			objFromDb.SKU = obj.SKU;
			objFromDb.PackingSKU = obj.PackingSKU;
			objFromDb.Size = obj.Size;
			objFromDb.ImageUrl = obj.ImageUrl ?? objFromDb.ImageUrl; // Only update if ImageUrl is not null
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
		public string GenerateSKU(string shopName)
        {
			Random random = new Random();
			int randomNumber = random.Next(1000, 10000);
			string SKU = DateTime.UtcNow.Year.ToString("d4") + DateTime.UtcNow.Month.ToString("d2") + DateTime.UtcNow.Day.ToString("d2");
			return SKU + randomNumber.ToString().Trim();
		}

		public string GenerateCode()
		 {
			using (var transaction = _db.Database.BeginTransaction()) // Use a transaction for data consistency
			{
				var variable = _db.GlobalNumbers.Where(x => x.Name == "globalnumber").FirstOrDefault();

				if (variable == null)
				{
					throw new InvalidOperationException("Global number record not found."); // Handle missing record
				}

				variable.VariantCode++;
				_db.SaveChanges();

				string code = variable.VariantCode.ToString().PadLeft(4, '0'); // Use string interpolation and PadLeft

				while (_db.Variants.Any(x => x.VariantCode == code)) // More efficient Any() for duplicate check
				{
					code = GenerateCode(); // Recursive call to generate a new code
				}

				transaction.Commit(); // Commit changes after successful generation
				return code;
			}
		}
	}
}
