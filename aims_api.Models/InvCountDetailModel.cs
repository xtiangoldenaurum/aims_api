﻿using aims_api.Models.Heredities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class InvCountDetailModel : InvCountDetailModelHeredity
    {
        public string? InvCountLineId { get; set; }
        public string? InvCountId { get; set; }
        public string? InvCountLineStatusId { get; set; }
        public string? InventoryId { get; set; }
        public int? QtyCount { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public string? Remarks { get; set; }
    }
}
