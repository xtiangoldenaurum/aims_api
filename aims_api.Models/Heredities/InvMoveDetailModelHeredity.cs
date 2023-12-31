﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models.Heredities
{
    public class InvMoveDetailModelHeredity
    {
        public string? Sku { get; set; }
        public string? UomRef{ get; set; }
        public string? ProductName { get; set; }
        public string? InvMoveLineStatus { get; set; }
        public string? LocationFrom { get; set; } // original name from database: 'LocationTo; 
        public string? LpnFrom { get; set; } // original name from database: 'LpnTo; 
        public string? TrackIdFrom { get; set; } //original name from db : 'TrackIdTo'
        //public int? QtyTo { get; set; } // original name from database: 'QtyTo'
    }
}
