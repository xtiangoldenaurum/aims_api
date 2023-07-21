using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class MovementTaskModelMod
    {
        public InventoryMovementModelMod? InvHead { get; set; }
        public InvMoveDetailModel? InvMoveDetail { get; set; }
    }
}
