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

    public class ConfigController : ControllerBase
    {
        private IConfigCore ConfigCore { get; set; }
        DataValidator DataValidator;

        public ConfigController(IConfigCore configCore)
        {
            ConfigCore = configCore;
            DataValidator = new DataValidator();
        }

        [HttpGet("getconfig")]
        public async Task<ActionResult> GetConfig()
        {
            try
            {
                return Ok(await ConfigCore.GetAllConfig());
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getconfigpg")]
        public async Task<ActionResult> GetConfigPg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await ConfigCore.GetConfigPg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getconfigpgsrch")]
        public async Task<ActionResult> GetConfigPgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await ConfigCore.GetConfigPgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getconfiguoms")]
        public async Task<ActionResult> GetConfigUOMs()
        {
            try
            {
                return Ok(await ConfigCore.GetAllConfig());
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getconfigbyid")]
        public async Task<ActionResult> GetConfigById(string configType, string configName)
        {
            try
            {
                if (string.IsNullOrEmpty(configType))
                {
                    await DataValidator.AddErrorField("configType");
                }
                if (string.IsNullOrEmpty(configName))
                {
                    await DataValidator.AddErrorField("configName");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await ConfigCore.GetConfigById(configType, configName));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createconfig")]
        public async Task<ActionResult> CreateConfig(ConfigModel config)
        {
            try
            {
                if (config == null)
                {
                    await DataValidator.AddErrorField("config");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await ConfigCore.CreateConfig(config));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updateconfig")]
        public async Task<ActionResult> UpdateConfig(ConfigModel config)
        {
            try
            {
                if (config == null)
                {
                    await DataValidator.AddErrorField("config");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await ConfigCore.UpdateConfig(config));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deleteconfig")]
        public async Task<ActionResult> DeleteConfig(string configType, string configName, string userAccountId)
        {
            try
            {
                if (string.IsNullOrEmpty(configType))
                {
                    await DataValidator.AddErrorField("configType");
                }
                if (string.IsNullOrEmpty(configName))
                {
                    await DataValidator.AddErrorField("configName");
                }
                if (string.IsNullOrEmpty(userAccountId))
                {
                    await DataValidator.AddErrorField("userAccountId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await ConfigCore.DeleteConfig(configType, configName, userAccountId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deleteuomconfig")]
        public async Task<ActionResult> DeleteUOMConfig(string configType, string configName, string userAccountId)
        {
            try
            {
                if (string.IsNullOrEmpty(configType))
                {
                    await DataValidator.AddErrorField("configType");
                }
                if (string.IsNullOrEmpty(configName))
                {
                    await DataValidator.AddErrorField("configName");
                }
                if (string.IsNullOrEmpty(userAccountId))
                {
                    await DataValidator.AddErrorField("userAccountId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await ConfigCore.DeleteUOMConfig(configType, configName, userAccountId));
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
