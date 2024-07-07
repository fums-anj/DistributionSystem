using BH.DataAccess.Data;
using BH.DataAccess.Infrastructure.Concrete.Repository;
using BH.DataAccess.Infrastructure.Interface.ICustomerManagementRepository;
using BH.Models;
using BH.Models.CustomerManagement;

namespace BH.DataAccess.Infrastructure.Concrete.CustomerManagementRepository
{
    public class CustomerRepository : Repository<ShopCustomer>, ICustomerRepository
    {
        private readonly BHContext _db;
        public CustomerRepository(BHContext db) : base(db)
        {
            _db = db;
        }
        public void Update(ShopCustomer obj)
        {
            _db.ShopCustomers.Update(obj);
        }
        public string GenerateCustomerCode()
        {
            GlobalNumber variable = _db.GlobalNumbers.Where(x => x.Name == "globalnumber").FirstOrDefault();
            variable.CustomerCode++;
            _db.SaveChanges();
            string Code = string.Empty;
            if (variable.CustomerCode < 10)
            {
                Code = "000" + variable.CustomerCode;
            }
            else
            {
                if (variable.CustomerCode < 100)
                {
                    Code = "00" + variable.CustomerCode;
                }
                else
                {
                    if (variable.CustomerCode < 1000)
                    {
                        Code = "0" + variable.CustomerCode;
                    }
                    else
                    {
                        Code = variable.CustomerCode.ToString();
                    }
                }
            }
            while (_db.ShopCustomers.FirstOrDefault(x => x.CustomerCode == Code) != null)
            {
                Code = GenerateCustomerCode();
            }
            return Code;
        }
    }
}
