﻿using BH.DataAccess.Infrastructure.Interface.IRepository;
using BH.Models.ProductManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.DataAccess.Infrastructure.Interface.IProductManagementRepository
{
    public interface IShopProductRepository : IRepository<ShopProduct>
    {
        void Update(ShopProduct obj);
    }
}
