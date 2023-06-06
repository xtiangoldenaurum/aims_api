using aims_api.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class PutawayTaskProcModel
    {
        public PutawayWinOneModel? PutawayWinOne { get; set; }
        public PutawayWinTwoModel? PutawayWinTwo { get; set; }
        public string? UserAccountId { get; set; }
    }
}
