using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class MovementWinOneModel
    {
        public string? TargetTrackId { get; set; }
        public string? Sku { get; set; }
        public string? Barcode { get; set; }
        public string? Barcode2 { get; set; }
        public string? Barcode3 { get; set; }
        public string? Barcode4 { get; set; }
        public string? ProductName { get; set; }
        public int CurrentQty { get; set; }
        public string? CurrentLocation { get; set; }
        public string? CurrentLPN { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? WarehousingDate { get; set; }
        public string? ProductConditionId { get; set; }
        public string? UomDisplay { get; set; }

        // override method for property values comparison
        public override bool Equals(object? obj)
        {
            var other = obj as MovementWinOneModel;

            if (other == null) return false;

            if (TargetTrackId != other.TargetTrackId ||
                Sku != other.Sku ||
                Barcode != other.Barcode ||
                Barcode2 != other.Barcode2 ||
                Barcode3 != other.Barcode3 ||
                Barcode4 != other.Barcode4 ||
                ProductName != other.ProductName ||
                CurrentQty != other.CurrentQty ||
                CurrentLocation != other.CurrentLocation ||
                CurrentLPN != other.CurrentLPN ||
                ManufactureDate != other.ManufactureDate ||
                ExpiryDate != other.ExpiryDate ||
                WarehousingDate != other.WarehousingDate ||
                ProductConditionId != other.ProductConditionId ||
                UomDisplay != other.UomDisplay)
                return false;

            return true;
        }
    }
}
