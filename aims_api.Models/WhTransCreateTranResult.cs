﻿using aims_api.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class WhTransCreateTranResult
    {
        public WhTransferTranResultCode ResultCode { get; set; }
        public string? WhTransferId { get; set; }
    }
}
