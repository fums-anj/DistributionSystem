﻿using BH.DataAccess.Data;
using BH.DataAccess.Infrastructure.Concrete.Repository;
using BH.DataAccess.Infrastructure.Interface.ISaleManagementRepository;
using BH.Models.SaleManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.DataAccess.Infrastructure.Concrete.SaleManagementRepository
{
    public class InvoiceRepository : Repository<Invoice>, IInvoiceRepository
    {
        private readonly BHContext _db;
        public InvoiceRepository(BHContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Invoice obj)
        {
            _db.Invoices.Update(obj);
        }
    }
}