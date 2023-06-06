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
    public class UtilitiesCore : IUtilitiesCore
    {
        private IUtilitiesRepository UtilitiesRepo { get; set; }
        public UtilitiesCore(IUtilitiesRepository utilitiesRepo)
        {
            UtilitiesRepo = utilitiesRepo;
        }

        public async Task<RequestResponse> DefineTranTypeByDocId(string documentId)
        {
            var data = await UtilitiesRepo.DefineTranTypeByDocId(documentId);

            if (!string.IsNullOrEmpty(data))
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to get document transaction origin.");
        }

        public async Task<RequestResponse> DefineTranTypeByDocLineId(string docLineId)
        {
            var data = await UtilitiesRepo.DefineTranTypeByDocLineId(docLineId);

            if (!string.IsNullOrEmpty(data))
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to get document transaction origin.");
        }

        public async Task<RequestResponse> GetTenantSettings()
        {
            var data = await UtilitiesRepo.GetTenantSettings();

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to get document transaction origin.");
        }
    }
}
