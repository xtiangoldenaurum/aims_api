﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class MovementContainerModel
    {
        public string? Sku { get; set; }
        public string? ReceivingId { get; set; }
        public string? MovementTaskId { get; set; }
        public string? UserAccountId { get; set; }
        public InventoryHistoryModel? InvHistory { get; set; }
        public MovementTaskProcModel? WinData { get; set; }
        public LotAttributeDetailModel? LotAtt { get; set; }
    }
}