using aims_printsvc.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class BCodeLabelToPrintModel
    {
        public PrinterDocType DocType { get; set; }
        public string? DocTypeId { get; set; }
        public string? Description { get; set; }
        public string? BarcodeContent { get; set; }
        public string? EPC { get; set; }
    }
}
