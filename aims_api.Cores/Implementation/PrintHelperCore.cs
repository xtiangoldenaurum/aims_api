using aims_api.Cores.Interface;
using aims_api.Enums;
using aims_api.Models;
using aims_api.Repositories.Interface;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Implementation
{
    public class PrintHelperCore : IPrintHelperCore
    {
        private IPrintHelperRepository PrintHelperRepo { get; set; }
        public PrintHelperCore(IPrintHelperRepository printHelperRepo)
        {
            PrintHelperRepo = printHelperRepo;
        }

        public async Task<RequestResponse> BuildZplDetails(List<BCodeLabelToPrintModel> docsToPrint)
        {
            var data = await PrintHelperRepo.BuildZplDetails(docsToPrint);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }
    }
}
