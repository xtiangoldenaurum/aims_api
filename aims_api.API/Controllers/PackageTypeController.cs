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

    public class PackageTypeController : ControllerBase
    {
        private IPackageTypeCore PackageTypeCore { get; set; }
        DataValidator DataValidator;

        public PackageTypeController(IPackageTypeCore packageTypeCore)
        {
            PackageTypeCore = packageTypeCore;
            DataValidator = new DataValidator();
        }

        [HttpGet("getpackagetypepg")]
        public async Task<ActionResult> GetPackageTypePg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await PackageTypeCore.GetPackageTypePg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getpackagetypepgsrch")]
        public async Task<ActionResult> GetPackageTypePgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await PackageTypeCore.GetPackageTypePgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getpackagetypebyid")]
        public async Task<ActionResult> GetPackageTypeById(string packageTypeId)
        {
            try
            {
                if (string.IsNullOrEmpty(packageTypeId))
                {
                    await DataValidator.AddErrorField("packageTypeId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await PackageTypeCore.GetPackageTypeById(packageTypeId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createpackagetype")]
        public async Task<ActionResult> CreatePackageType(PackageTypeModel packageType)
        {
            try
            {
                if (packageType == null)
                {
                    await DataValidator.AddErrorField("packageType");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await PackageTypeCore.CreatePackageType(packageType));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updatepackagetype")]
        public async Task<ActionResult> UpdatePackageType(PackageTypeModel packageType)
        {
            try
            {
                if (packageType == null)
                {
                    await DataValidator.AddErrorField("packageType");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await PackageTypeCore.UpdatePackageType(packageType));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deletepackagetype")]
        public async Task<ActionResult> DeletePackageType(string packageTypeId)
        {
            try
            {
                if (string.IsNullOrEmpty(packageTypeId))
                {
                    await DataValidator.AddErrorField("packageTypeId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await PackageTypeCore.DeletePackageType(packageTypeId));
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
