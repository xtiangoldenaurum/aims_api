using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class ProductModel
    {
        [Required(ErrorMessage = "Provide valid ID")]
        public string? Sku { get; set; }
		public string? ProductName { get; set; }
		public string? Description { get; set; }
		public string? Barcode { get; set; }
		public string? Barcode2 { get; set; }
		public string? Barcode3 { get; set; }
		public string? Barcode4 { get; set; }
		public string? UniqueRfid { get; set; }
		public string? QrCode { get; set; }
		public string? UomRef { get; set; }
		public string? Length { get; set; }
		public string? Width { get; set; }
		public string? Height { get; set; }
		public string? Cubic { get; set; }
		public string? GrossWeight { get; set; }
		public string? NetWeight { get; set; }
		public string? ProductCategoryId { get; set; }
		public string? ProductCategoryId2 { get; set; }
		public string? ProductCategoryId3 { get; set; }
		public byte[]? Image { get; set; }
		public int CaptureTag { get; set; }
		public DateTime DateCreated { get; set; }
		public DateTime DateModified { get; set; }
		public string? CreatedBy { get; set; }
		public string? ModifiedBy { get; set; }
    }
}
