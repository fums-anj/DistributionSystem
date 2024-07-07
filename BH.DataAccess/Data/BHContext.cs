using BH.Models;
using BH.Models.AccountManagement;
using BH.Models.CustomerManagement;
using BH.Models.InventoryManagement;
using BH.Models.OrganizationManagement;
using BH.Models.ProductManagement;
using BH.Models.SaleManagement;
using BH.Models.ShopManagement;
using BH.Utility;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BH.DataAccess.Data
{
    public class BHContext : IdentityDbContext
    {
        public BHContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Variant> Variants { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; }
        public DbSet<UnitOfMeasure> UnitOfMeasures { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<StockTransfer> StockTransfers { get; set; }
        public DbSet<SaleOrderLine> SaleOrderLines { get; set; }
        public DbSet<SaleOrder> SaleOrders { get; set; }
        public DbSet<ReturnReason> ReturnReasons { get; set; }
        public DbSet<ReturnOrder> ReturnOrders { get; set; }
        public DbSet<ReturnLine> ReturnLines { get; set; }
        public DbSet<ReceiptVoucher> ReceiptVouchers { get; set; }
        public DbSet<PurchaseOrderLine> PurchaseOrderLines { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PaymentVoucher> PaymentVouchers { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<OrderType> OrderTypes { get; set; }
        public DbSet<ItemCondition> ItemConditions { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<ExpenseYear> ExpenseYears { get; set; }
        public DbSet<ExpenseType> ExpenseTypes { get; set; }
        public DbSet<ExpenseMonth> ExpenseMonths { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<ShopCustomer> ShopCustomers { get; set; }
        public DbSet<CreditNote> CreditNotes { get; set; }
        public DbSet<Catalog> Catalogs { get; set; }
        public DbSet<AccountReceivable> AccountReceivables { get; set; }
        public DbSet<AccountPayable> AccountPayables { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<LocationType> LocationTypes { get; set; }
        public DbSet<Shop> Shops { get; set; }
        public DbSet<ShopProduct> ShopProducts { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<CustomerRoute> CustomerRoutes { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<ShopCart> ShopCarts { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetail { get; set; }
        public DbSet<ManageCash> ManageCash { get; set; }
        public DbSet<GlobalNumber> GlobalNumbers { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<GlobalNumber>().HasData(new GlobalNumber
            {
                Id = 1,
                Name = "globalnumber",
                ProficitCenterNum = 01,
                CostCenterNum = 01,
                SKU = 1,
                VariantCode = 0000,
                CustomerCode = 0000000000,
                CreatedDateTime = DateTime.Now
            });
            builder.Entity<UnitOfMeasure>().HasData(
                new UnitOfMeasure{ Id = 1, Name = SDUnits.Unit_piece, ValueInBaseUnit = 1, IsWaight = false, IsDeleted = false, IsDisable = false, CreatedDate = DateTime.Now },
                new UnitOfMeasure{ Id = 2, Name = SDUnits.Unit_packing, ValueInBaseUnit = 1, IsWaight = false, IsDeleted = false, IsDisable = false, CreatedDate = DateTime.Now },
                new UnitOfMeasure{ Id = 3, Name = SDUnits.Unit_kg, ValueInBaseUnit = 1, IsWaight = true, IsDeleted = false, IsDisable = false, CreatedDate = DateTime.Now },
                new UnitOfMeasure{ Id = 4, Name = SDUnits.Unit_gram, ValueInBaseUnit = 0.001, IsWaight = true, IsDeleted = false, IsDisable = false, CreatedDate = DateTime.Now },
                new UnitOfMeasure{ Id = 5, Name = SDUnits.Unit_mg, ValueInBaseUnit = 0.000001, IsWaight = true, IsDeleted = false, IsDisable = false, CreatedDate = DateTime.Now }
            );
            builder.Entity<PaymentMethod>().HasData(
                new PaymentMethod{ Id = 1, Name = SDPaymentMethod.PaymentMethod_Cash, IsDeleted = false, IsDisable = false, CreatedDate = DateTime.Now },
                new PaymentMethod{ Id = 2, Name = SDPaymentMethod.PaymentMethod_BankTransfer, IsDeleted = false, IsDisable = false, CreatedDate = DateTime.Now },
                new PaymentMethod{ Id = 3, Name = SDPaymentMethod.PaymentMethod_OnlinPayment, IsDeleted = false, IsDisable = false, CreatedDate = DateTime.Now },
                new PaymentMethod{ Id = 4, Name = SDPaymentMethod.PaymentMethod_CreditCard, IsDeleted = false, IsDisable = false, CreatedDate = DateTime.Now },
                new PaymentMethod{ Id = 5, Name = SDPaymentMethod.PaymentMethod_Cheque, IsDeleted = false, IsDisable = false, CreatedDate = DateTime.Now }
            );
        }
    }
}
