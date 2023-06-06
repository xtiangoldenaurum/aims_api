using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class ProductPricingModel
    {
        public string? SKU { get; set; }
        public double Cost { get; set; }
        public double RetailPrice { get; set; }
        public double WholeSalePrice { get; set; }
        public double DiscountedPrice { get; set; }
    }
}
