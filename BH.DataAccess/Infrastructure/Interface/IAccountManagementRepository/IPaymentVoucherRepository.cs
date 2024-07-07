﻿using BH.DataAccess.Infrastructure.Interface.IRepository;
using BH.Models.AccountManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.DataAccess.Infrastructure.Interface.IAccountManagementRepository
{
    public interface IPaymentVoucherRepository : IRepository<PaymentVoucher>
    {
        void Update(PaymentVoucher obj);
    }
}
