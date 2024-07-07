using BH.DataAccess.Infrastructure.Interface.IRepository;
using BH.Models.ProductManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.DataAccess.Infrastructure.Interface.IProductManagementRepository
{
    public interface IVariantRepository : IRepository<Variant>
    {
        void Update(Variant obj);
        public string GenerateSKU(string shopName);
        public string GenerateCode();
    }
}
