using aims_api.Cores.Interface;
using aims_api.Enums;
using aims_api.Models;
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

    public class PrintHelperController : ControllerBase
    {
        private IPrintHelperCore PrintHelperCore { get; set; }

        public PrintHelperController(IPrintHelperCore printHelperCore)
        {
            PrintHelperCore = printHelperCore;
        }

        [HttpGet("builzpldetails")]
        public async Task<ActionResult> BuildZplDetails(List<BCodeLabelToPrintModel> labelData)
        {
            try
            {
                return Ok(await PrintHelperCore.BuildZplDetails(labelData));
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
