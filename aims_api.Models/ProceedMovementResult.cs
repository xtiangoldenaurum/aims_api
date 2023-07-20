﻿using aims_api.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class ProceedMovementResult
    {
        public ProceedMovementResultCode ResultCode { get; set; } = ProceedMovementResultCode.FAILED;
        public string? ConflictMsg { get; set; }
    }
}
