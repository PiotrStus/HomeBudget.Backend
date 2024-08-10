﻿using HomeBudget.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Application.Interfaces
{
    public interface ICurrentAccountProvider
    {
        Task<Account> GetAuthenticatedAccount();

        Task<int?> GetAccountId();
    }
}