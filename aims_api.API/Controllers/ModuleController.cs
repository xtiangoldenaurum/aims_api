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
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]

    public class ModuleController : ControllerBase
    {
        private IModuleCore ModuleCore { get; set; }
        DataValidator DataValidator;

        public ModuleController(IModuleCore moduleCore)
        {
            ModuleCore = moduleCore;
            DataValidator = new DataValidator();
        }

        [HttpGet("getmodulepg")]
        public async Task<ActionResult> GetModulePg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await ModuleCore.GetModulePg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getmodulepgsrch")]
        public async Task<ActionResult> GetModulePgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
        {
            try
            {
                if (string.IsNullOrEmpty(searchKey))
                {
                    await DataValidator.AddErrorField("searchKey");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await ModuleCore.GetModulePgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getmodulebyid")]
        public async Task<ActionResult> GetModuleById(string moduleId)
        {
            try
            {
                if (string.IsNullOrEmpty(moduleId))
                {
                    await DataValidator.AddErrorField("moduleId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await ModuleCore.GetModuleById(moduleId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createmodule")]
        public async Task<ActionResult> CreateModule(ModuleModel module)
        {
            try
            {
                if (module == null)
                {
                    await DataValidator.AddErrorField("module");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await ModuleCore.CreateModule(module));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updatemodule")]
        public async Task<ActionResult> UpdateModule(ModuleModel module)
        {
            try
            {
                if (module == null)
                {
                    await DataValidator.AddErrorField("module");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await ModuleCore.UpdateModule(module));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deletemodule")]
        public async Task<ActionResult> DeleteModule(string moduleId)
        {
            try
            {
                if (string.IsNullOrEmpty(moduleId))
                {
                    await DataValidator.AddErrorField("moduleId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await ModuleCore.DeleteModule(moduleId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getusermodules")]
        public async Task<ActionResult> GetUserModules(string accessRightId)
        {
            try
            {
                if (string.IsNullOrEmpty(accessRightId))
                {
                    await DataValidator.AddErrorField("moduleId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await ModuleCore.GetUserModules(accessRightId));
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
