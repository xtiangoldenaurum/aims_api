using aims_api.Cores.Interface;
using aims_api.Enums;
using aims_api.Repositories.Interface;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Implementation
{
    public class AccessRightDetailCore : IAccessRightDetailCore
    {
        private IAccessRightDetailRepository AccessRightDetailRepo { get; set; }
        public AccessRightDetailCore(IAccessRightDetailRepository accessRightDetailRepo)
        {
            AccessRightDetailRepo = accessRightDetailRepo;
        }

        public async Task<RequestResponse> GetAllAccessRightDetail()
        {
            var data = await AccessRightDetailRepo.GetAllAccessRightDetail();

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetAccessRightDetailById(string accessRightId)
        {
            var data = await AccessRightDetailRepo.GetAccessRightDetailById(accessRightId);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetUserAccessDetails(string accessRightId)
        {
            var data = await AccessRightDetailRepo.GetUserAccessDetails(accessRightId);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }
    }
}
