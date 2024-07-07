using BH.DataAccess.Infrastructure.Interface.IAccountManagementRepository;
using BH.DataAccess.Infrastructure.Interface.ICustomerManagementRepository;
using BH.DataAccess.Infrastructure.Interface.IInventoryManagementRepository;
using BH.DataAccess.Infrastructure.Interface.IOrganizationManagementRepository;
using BH.DataAccess.Infrastructure.Interface.IProductManagementRepository;
using BH.DataAccess.Infrastructure.Interface.ISaleManagementRepository;
using BH.DataAccess.Infrastructure.Interface.IShopManagementRepository;

namespace BH.DataAccess.Infrastructure.Interface.IRepository
{
    public interface IUnitOfWork
    {
        IVariantRepository Variant { get; }
        IUserActivityRepository UserActivity { get; }
        IUnitOfMeasureRepository UnitOfMeasure { get; }
        ISupplierRepository Supplier { get; }
        IStockTransferRepository StockTransfer { get; }
        ISaleOrderLineRepository SaleOrderLine { get; }
        ISaleOrderRepository SaleOrder { get; }
        IReturnReasonRepository ReturnReason { get; }
        IReturnOrderRepository ReturnOrder { get; }
        IReturnLineRepository ReturnLine { get; }
        IReceiptVoucherRepository ReceiptVoucher { get; }
        IPurchaseOrderLineRepository PurchaseOrderLine { get; }
        IPurchaseOrderRepository PurchaseOrder { get; }
        IPaymentVoucherRepository PaymentVoucher { get; }
        IPaymentMethodRepository PaymentMethod { get; }
        IOrderTypeRepository OrderType { get; }
        IItemConditionRepository ItemCondition { get; }
        IInvoiceRepository Invoice { get; }
        IExpenseYearRepository ExpenseYear { get; }
        IExpenseTypeRepository ExpenseType { get; }
        IExpenseMonthRepository ExpenseMonth { get; }
        IExpenseRepository Expense { get; }
        ICustomerRepository ShopCustomer { get; }
        ICreditNoteRepository CreditNote { get; }
        ICatalogRepository Catalog { get; }
        IAccountReceivableRepository AccountReceivable { get; }
        IAccountPayableRepository AccountPayable { get; }
        IStatusRepository Status { get; }
        ILocationRepository Location { get; }
        ILocationTypeRepository LocationType { get; }
        IShopRepository Shop { get; }
        IShopProductRepository ShopProduct { get; }
        ICompanyRepository Company { get; }
        IShopCartRepository ShopCart { get; }
        IApplicationUserRepository ApplicationUser { get; }
        IOrderHeaderRepository OrderHeader { get; }
        IOrderDetailRepository OrderDetail { get; }
        IManageCashRepository ManageCash { get; }
        IRouteRepository CustomerRoute { get; }

        void Save();
    }
}
