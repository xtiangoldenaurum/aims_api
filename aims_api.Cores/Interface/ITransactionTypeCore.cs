﻿using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface ITransactionTypeCore
    {
        Task<RequestResponse> GetAllTranType();
    }
}
