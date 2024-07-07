using BH.DataAccess.Data;
using BH.DataAccess.Infrastructure.Concrete.Repository;
using BH.DataAccess.Infrastructure.Interface.IProductManagementRepository;
using BH.Models.ProductManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.DataAccess.Infrastructure.Concrete.ProductManagementRepository
{
    public class UnitOfMeasureRepository : Repository<UnitOfMeasure>, IUnitOfMeasureRepository
    {
        private readonly BHContext _db;
        public UnitOfMeasureRepository(BHContext db) : base(db)
        {
            _db = db;
        }
        public void Update(UnitOfMeasure obj)
        {
            _db.UnitOfMeasures.Update(obj);
        }
    }
}
