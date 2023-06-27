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
    public class InvMoveCore : IInvMoveCore
    {
        private IInvMoveRepository InvMoveRepo { get; set; }
        public EnumHelper EnumHelper { get; set; }
        public InvMoveCore(IInvMoveRepository invMoveRepo, EnumHelper enumHelper)
        {
            InvMoveRepo = invMoveRepo;
            EnumHelper = enumHelper;
        }
        public async Task<RequestResponse> CreateInvMoveMod(InvMoveModelMod invMove)
        {
            var res = await InvMoveRepo.CreateInvMoveMod(invMove);
            string resMsg = await EnumHelper.GetDescription(res.ResultCode);

            if (res.ResultCode == InvMoveTranResultCode.SUCCESS)
            {
                return new RequestResponse(ResponseCode.SUCCESS, resMsg, res.InvMoveId);
            }

            return new RequestResponse(ResponseCode.FAILED, resMsg, (res.ResultCode).ToString());

        }
    }
}
