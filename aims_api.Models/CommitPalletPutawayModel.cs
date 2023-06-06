using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class CommitPalletPutawayModel
    {
        public IEnumerable<PalletContentModel>? Contents { get; set; }
        public string? PutawayLocation { get; set; }
        public string? UserAccountId { get; set; }
    }
}
