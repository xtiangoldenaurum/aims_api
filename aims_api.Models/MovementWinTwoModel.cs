using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class MovementWinTwoModel
    {
        public int MovementQty { get; set; }
        public string? MovementLocation { get; set; }
        public string? MovementTrackId { get; set; }
        public string? MovementLPN { get; set; }
    }
}
