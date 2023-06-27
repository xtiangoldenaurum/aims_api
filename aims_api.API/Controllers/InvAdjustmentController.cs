using aims_api.Cores.Interface;
using aims_api.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace aims_api.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvAdjustmentController : ControllerBase
    {
        private IInvAdjustmentCore InvAdjustmentCore { get; set; }
        DataValidator DataValidator;
        public InvAdjustmentController(IInvAdjustmentCore invAdjustmentCore)
        {
            InvAdjustmentCore = invAdjustmentCore;
            DataValidator = new DataValidator();
        }


    }
}
