using aims_api.Cores.Interface;
using aims_api.Enums;
using aims_api.Models;
using aims_api.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace aims_api.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvMoveController : ControllerBase
    {
        private IInvMoveCore InvMoveCore { get; set; }
        DataValidator DataValidator;
        public InvMoveController(IInvMoveCore invMoveCore)
        {
            InvMoveCore = invMoveCore;
            DataValidator = new DataValidator();
        }

        [HttpPost("createinvmovemod")]
        public async Task<ActionResult> CreateInvMoveMod(InvMoveModelMod invMove)
        {
            try
            {
                if (invMove == null)
                {
                    await DataValidator.AddErrorField("invMove");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvMoveCore.CreateInvMoveMod(invMove));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }
    }
}
