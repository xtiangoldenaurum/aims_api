using aims_api.Cores.Interface;
using aims_api.Enums;
using aims_api.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace aims_api.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]

    public class TransactionTypeController : ControllerBase
    {
        private ITransactionTypeCore TranTypeCore { get; set; }

        public TransactionTypeController(ITransactionTypeCore tranTypeCore)
        {
            TranTypeCore = tranTypeCore;
        }

        [HttpGet("gettrantypes")]
        public async Task<ActionResult> GetTranTypes()
        {
            try
            {
                return Ok(await TranTypeCore.GetAllTranType());
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
