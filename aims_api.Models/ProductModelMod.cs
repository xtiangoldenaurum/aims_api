using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class ProductModelMod
    {
        public ProductModel? Product { get; set; }
        public ProductPricingModel? ProductPricing { get; set; }
        public dynamic? ProdUfields { get; set; }
    }
}
