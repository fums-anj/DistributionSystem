using BH.DataAccess.Data;
using BH.DataAccess.Infrastructure.Concrete.AccountManagementRepository;
using BH.DataAccess.Infrastructure.Concrete.CustomerManagementRepository;
using BH.DataAccess.Infrastructure.Concrete.InventoryManagementRepository;
using BH.DataAccess.Infrastructure.Concrete.OrganizationManagementRepository;
using BH.DataAccess.Infrastructure.Concrete.ProductManagementRepository;
using BH.DataAccess.Infrastructure.Concrete.SaleManagementRepository;
using BH.DataAccess.Infrastructure.Concrete.ShopManagementRepository;
using BH.DataAccess.Infrastructure.Interface.IAccountManagementRepository;
using BH.DataAccess.Infrastructure.Interface.ICustomerManagementRepository;
using BH.DataAccess.Infrastructure.Interface.IInventoryManagementRepository;
using BH.DataAccess.Infrastructure.Interface.IOrganizationManagementRepository;
using BH.DataAccess.Infrastructure.Interface.IProductManagementRepository;
using BH.DataAccess.Infrastructure.Interface.IRepository;
using BH.DataAccess.Infrastructure.Interface.ISaleManagementRepository;
using BH.DataAccess.Infrastructure.Interface.IShopManagementRepository;
using BH.Models.CustomerManagement;

namespace BH.DataAccess.Infrastructure.Concrete.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BHContext _db;
        public UnitOfWork(BHContext db)
        {
            _db = db;
            Variant = new VariantRepository(_db);
            UserActivity = new UserActivityRepository(_db);
            UnitOfMeasure = new UnitOfMeasureRepository(_db);
            Supplier = new SupplierRepository(_db);
            StockTransfer = new StockTransferRepository(_db);
            SaleOrderLine = new SaleOrderLineRepository(_db);
            SaleOrder = new SaleOrderRepository(_db);
            ReturnReason = new ReturnReasonRepository(_db);
            ReturnOrder = new ReturnOrderRepository(_db);
            ReturnLine = new ReturnLineRepository(_db);
            ReceiptVoucher = new ReceiptVoucherRepository(_db);
            PurchaseOrderLine = new PurchaseOrderLineRepository(_db);
            PurchaseOrder = new PurchaseOrderRepository(_db);
            PaymentVoucher = new PaymentVoucherRepository(_db);
            PaymentMethod = new PaymentMethodRepository(_db);
            OrderType = new OrderTypeRepository(_db);
            ItemCondition = new ItemConditionRepository(_db);
            Invoice = new InvoiceRepository(_db);
            ExpenseYear = new ExpenseYearRepository(_db);
            ExpenseType = new ExpenseTypeRepository(_db);
            ExpenseMonth = new ExpenseMonthRepository(_db);
            Expense = new ExpenseRepository(_db);
            ShopCustomer = new CustomerRepository(_db);
            CreditNote = new CreditNoteRepository(_db);
            Catalog = new CatalogRepository(_db);
            AccountReceivable = new AccountReceivableRepository(_db);
            AccountPayable = new AccountPayableRepository(_db);
            Status = new StatusRepository(_db);
            LocationType = new LocationTypeRepository(_db);
            Shop = new ShopRepository(_db);
            Location = new LocationRepository(_db);
            ShopProduct = new ShopProductRepository(_db);
            Company = new CompanyRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
            ShopCart = new ShopCartRepository(_db);
            OrderHeader = new OrderHeaderRepository(_db);
            OrderDetail = new OrderDetailRepository(_db);
            ManageCash = new ManageCashRepository(_db);
            CustomerRoute = new RouteRepository(_db);
        }
        public IVariantRepository Variant { get; private set; }
        public IUserActivityRepository UserActivity { get; private set; }
        public IUnitOfMeasureRepository UnitOfMeasure { get; private set; }
        public ISupplierRepository Supplier { get; private set; }
        public IStockTransferRepository StockTransfer { get; private set; }
        public ISaleOrderLineRepository SaleOrderLine { get; private set; }
        public ISaleOrderRepository SaleOrder { get; private set; }
        public IReturnReasonRepository ReturnReason { get; private set; }
        public IReturnOrderRepository ReturnOrder { get; private set; }
        public IReturnLineRepository ReturnLine { get; private set; }
        public IReceiptVoucherRepository ReceiptVoucher { get; private set; }
        public IPurchaseOrderLineRepository PurchaseOrderLine { get; private set; }
        public IPurchaseOrderRepository PurchaseOrder { get; private set; }
        public IPaymentVoucherRepository PaymentVoucher { get; private set; }
        public IPaymentMethodRepository PaymentMethod { get; private set; }
        public IOrderTypeRepository OrderType { get; private set; }
        public IItemConditionRepository ItemCondition { get; private set; }
        public IInvoiceRepository Invoice { get; private set; }
        public IExpenseYearRepository ExpenseYear { get; private set; }
        public IExpenseTypeRepository ExpenseType { get; private set; }
        public IExpenseMonthRepository ExpenseMonth { get; private set; }
        public IExpenseRepository Expense { get; private set; }
        public ICustomerRepository ShopCustomer { get; private set; }
        public ICreditNoteRepository CreditNote { get; private set; }
        public ICatalogRepository Catalog { get; private set; }
        public IAccountReceivableRepository AccountReceivable { get; private set; }
        public IAccountPayableRepository AccountPayable { get; private set; }
        public IStatusRepository Status { get; private set; }
        public ILocationTypeRepository LocationType { get; private set; }
        public ILocationRepository Location { get; private set; }
        public IShopRepository Shop { get; private set; }
        public IShopProductRepository ShopProduct { get; private set; }
        public ICompanyRepository Company { get; private set; }
        public IShopCartRepository ShopCart { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public IOrderHeaderRepository OrderHeader { get; private set; }
        public IOrderDetailRepository OrderDetail { get; private set; }
        public IManageCashRepository ManageCash { get; private set; }
        public IRouteRepository CustomerRoute { get; set; }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
