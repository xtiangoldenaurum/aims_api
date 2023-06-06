using aims_api.Cores.Implementation;
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

    public class UserAccountController : ControllerBase
    {
        private IUserAccountCore UserAccountCore { get; set; }
        DataValidator DataValidator;

        public UserAccountController(IUserAccountCore userAccountCore)
        {
            UserAccountCore = userAccountCore;
            DataValidator = new DataValidator();
        }

        [HttpPost("getuseraccountsspecial")]
        public async Task<ActionResult> GetUserAccountsSpecial(UserAccountFilterMdl filter, string? searchKey, int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await UserAccountCore.GetUserAccountsSpecial(filter, searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getuseraccountpg")]
        public async Task<ActionResult> GetUserAccountPg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await UserAccountCore.GetUserAccountPg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getuseraccpaged")]
        public async Task<ActionResult> GetUserAccPaged(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await UserAccountCore.GetUserAccPaged(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getactiveuseraccountpg")]
        public async Task<ActionResult> GetActiveUserAccountPg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await UserAccountCore.GetActiveUserAccountPg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getinactiveuseraccountpg")]
        public async Task<ActionResult> GetInActiveUserAccountPg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await UserAccountCore.GetInActiveUserAccountPg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getuseraccountpgsrch")]
        public async Task<ActionResult> GetUserAccountPgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await UserAccountCore.GetUserAccountPgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getuseraccsrchpaged")]
        public async Task<ActionResult> GetUserAccSrchPaged(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await UserAccountCore.GetUserAccSrchPaged(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getuseraccountbyid")]
        public async Task<ActionResult> GetUserAccountById(string userAccountId)
        {
            try
            {
                if (string.IsNullOrEmpty(userAccountId))
                {
                    await DataValidator.AddErrorField("userAccountId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await UserAccountCore.GetUserAccountById(userAccountId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("getuseraccountpgfiltered")]
        public async Task<ActionResult> GetUserAccountPgFiltered(UserAccountFilterMdl filter, int pageNum, int pageItem)
        {
            try
            {
                if (filter == null)
                {
                    await DataValidator.AddErrorField("filter");
                }
                if (filter != null)
                {
                    if (filter.AccessRightId == null && 
                        filter.Inactive == null && 
                        filter.AccountExpiry == null)
                    {
                        return Ok(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data"));
                    }
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await UserAccountCore.GetUserAccountPgFiltered(filter, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("getuseraccfltrpaged")]
        public async Task<ActionResult> GetUserAccFltrPaged(UserAccountFilterMdl filter, int pageNum, int pageItem)
        {
            try
            {
                if (filter == null)
                {
                    await DataValidator.AddErrorField("filter");
                }
                if (filter != null)
                {
                    if (filter.AccessRightId == null &&
                        filter.Inactive == null &&
                        filter.AccountExpiry == null)
                    {
                        return Ok(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data"));
                    }
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await UserAccountCore.GetUserAccFltrPaged(filter, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getusraccbyaccessrightidpg")]
        public async Task<ActionResult> GetUsrAccByAccessRightIdPg(string accessrightId, int pageNum = 1, int pageItem = 100)
        {
            try
            {
                if (string.IsNullOrEmpty(accessrightId))
                {
                    await DataValidator.AddErrorField("accessrightId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await UserAccountCore.GetUsrAccByAccessRightIdPg(accessrightId, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createuseraccount")]
        public async Task<ActionResult> CreateUserAccount(UserAccountModelMod userAccount)
        {
            try
            {
                if (userAccount == null)
                {
                    await DataValidator.AddErrorField("userAccount");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await UserAccountCore.CreateUserAccount(userAccount));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updateuseraccount")]
        public async Task<ActionResult> UpdateUserAccount(UserAccountModelMod userAccount)
        {
            try
            {
                if (userAccount == null)
                {
                    await DataValidator.AddErrorField("userAccount");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await UserAccountCore.UpdateUserAccount(userAccount));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deleteuseraccount")]
        public async Task<ActionResult> DeleteUserAccount(string delUserAccountId, string userAccountId)
        {
            try
            {
                if (string.IsNullOrEmpty(delUserAccountId))
                {
                    await DataValidator.AddErrorField("delUserAccountId");
                }
                if (string.IsNullOrEmpty(userAccountId))
                {
                    await DataValidator.AddErrorField("userAccountId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await UserAccountCore.DeleteUserAccount(delUserAccountId, userAccountId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deactuseraccount")]
        public async Task<ActionResult> DeActUserAccount(string delUserAccountId, string userAccountId)
        {
            try
            {
                if (string.IsNullOrEmpty(delUserAccountId))
                {
                    await DataValidator.AddErrorField("delUserAccountId");
                }
                if (string.IsNullOrEmpty(userAccountId))
                {
                    await DataValidator.AddErrorField("userAccountId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await UserAccountCore.DeActUserAccount(delUserAccountId, userAccountId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("ReActuseraccount")]
        public async Task<ActionResult> ReActUserAccount(string delUserAccountId, string userAccountId)
        {
            try
            {
                if (string.IsNullOrEmpty(delUserAccountId))
                {
                    await DataValidator.AddErrorField("delUserAccountId");
                }
                if (string.IsNullOrEmpty(userAccountId))
                {
                    await DataValidator.AddErrorField("userAccountId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await UserAccountCore.ReActUserAccount(delUserAccountId, userAccountId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("useraccountvalidationweb")]
        public async Task<ActionResult> UserAccountValidationWeb(LoginCredentailsModel account)
        {
            try
            {
                if (account == null)
                {
                    await DataValidator.AddErrorField("account");
                }
                if (account != null && string.IsNullOrEmpty(account.Username))
                {
                    await DataValidator.AddErrorField("Username");
                }
                if (account != null && string.IsNullOrEmpty(account.Password))
                {
                    await DataValidator.AddErrorField("Password");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await UserAccountCore.UserAccountValidationWeb(account));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("useraccountvalidationmob")]
        public async Task<ActionResult> UserAccountValidationMob(LoginCredentailsModel account)
        {
            try
            {
                if (account == null)
                {
                    await DataValidator.AddErrorField("account");
                }
                if (account != null && string.IsNullOrEmpty(account.Username))
                {
                    await DataValidator.AddErrorField("Username");
                }
                if (account != null && string.IsNullOrEmpty(account.Password))
                {
                    await DataValidator.AddErrorField("Password");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await UserAccountCore.UserAccountValidationMob(account));
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
