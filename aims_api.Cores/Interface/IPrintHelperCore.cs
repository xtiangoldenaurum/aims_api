using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IPrintHelperCore
    {
        Task<RequestResponse> BuildZplDetails(List<BCodeLabelToPrintModel> docsToPrint);
    }
}
