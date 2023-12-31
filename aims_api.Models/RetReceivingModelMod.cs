﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class RetReceivingModelMod
    {
        public RetInventoryModelMod? InvHead { get; set; }
        public InventoryHistoryModel? InvDetail { get; set; }
        public LotAttributeDetailModel? LotAtt { get; set; }
        public IEnumerable<UniqueTagsModel>? UniqTags { get; set; }
    }
}
