using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class CommitMovementTaskModel
    {
        public IEnumerable<MovementContentModel>? Contents { get; set; }
        public int FinalQty { get; set; }
        public string? FinalLocation { get; set; }
        public string? FinalLpn { get; set; }
        public string? UserAccountId { get; set; }
    }
}
