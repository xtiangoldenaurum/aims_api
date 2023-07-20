using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class MovementTaskProcModel
    {
        public MovementWinOneModel? MovementWinOne { get; set; }
        public MovementWinTwoModel? MovementWinTwo { get; set; }
        public string? UserAccountId { get; set; }
    }
}
