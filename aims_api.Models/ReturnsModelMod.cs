using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class ReturnsModelMod
    {
        public ReturnsModel? ReturnsHeader { get; set; }
        public IEnumerable<ReturnsDetailModel>? ReturnsDetails { get; set; }
        public dynamic? ReturnsUfields { get; set; }
    }
}
