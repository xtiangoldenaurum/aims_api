using aims_api.Enums;
using aims_api.Models;
using aims_api.Repositories.AuditBuilder;
using aims_api.Repositories.Interface;
using aims_api.Utilities;
using aims_api.Utilities.Interface;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Implementation
{
    public class UserAccountRepository : IUserAccountRepository
    {
        private string ConnString;
        IAuditTrailRepository AuditTrailRepo;
        UserAccountAudit AuditBuilder;
        IPagingRepository PagingRepo;
        PasswordHash PassHash;
        IAccessRightDetailRepository AccessRightDtlRepo;
        IUsrAccUserFieldRepository UsrAccUFieldRepo;

        public UserAccountRepository(ITenantProvider tenantProvider,
                                        IAuditTrailRepository auditTrailRepo,
                                        PasswordHash passHash,
                                        IAccessRightDetailRepository accessRightDtlRepo,
                                        IUsrAccUserFieldRepository usrAccUFieldRepo)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            AuditTrailRepo = auditTrailRepo;
            PagingRepo = new PagingRepository();
            AuditBuilder = new UserAccountAudit();
            PassHash = passHash;
            AccessRightDtlRepo = accessRightDtlRepo;
            UsrAccUFieldRepo = usrAccUFieldRepo;
        }

        public async Task<IEnumerable<UserAccountModel>> GetUserAccountPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select userAccountId, 
                                            accessRightId, 
                                            firstName, 
                                            middleInitial, 
                                            lastName, 
                                            image, 
                                            email, 
                                            phone, 
                                            inactive, 
                                            accountExpiry, 
                                            dateCreated, 
                                            dateModified, 
                                            createdBy, 
                                            modifiedBy 
                                    from UserAccount limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<UserAccountModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<UserAccountPagedMdl?> GetUserAccPaged(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select userAccountId, 
                                            accessRightId, 
                                            firstName, 
                                            middleInitial, 
                                            lastName, 
                                            image, 
                                            email, 
                                            phone, 
                                            inactive, 
                                            accountExpiry, 
                                            dateCreated, 
                                            dateModified, 
                                            createdBy, 
                                            modifiedBy 
                                    from UserAccount order by lastName 
                                    limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);

                var ret = await db.QueryAsync<UserAccountModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    string strPgQry = "select count(userAccountId) from useraccount;";
                    var pageDetail = await PagingRepo.GetPageDetail(db, strPgQry, pageNum, pageItem, ret.Count());

                    return new UserAccountPagedMdl()
                    {
                        Pagination = pageDetail,
                        UserAccount = ret
                    };
                }
            }

            return null;
        }

        public async Task<IEnumerable<UserAccountModel>> GetActiveUserAccountPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select userAccountId, 
                                            accessRightId, 
                                            firstName, 
                                            middleInitial, 
                                            lastName, 
                                            image, 
                                            email, 
                                            phone, 
                                            inactive, 
                                            accountExpiry, 
                                            dateCreated, 
                                            dateModified, 
                                            createdBy, 
                                            modifiedBy 
                                     from UserAccount where inactive = false limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<UserAccountModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<UserAccountModel>> GetInActiveUserAccountPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select userAccountId, 
                                            accessRightId, 
                                            firstName, 
                                            middleInitial, 
                                            lastName, 
                                            image, 
                                            email, 
                                            phone, 
                                            inactive, 
                                            accountExpiry, 
                                            dateCreated, 
                                            dateModified, 
                                            createdBy, 
                                            modifiedBy 
                                     from UserAccount where inactive = true limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<UserAccountModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<UserAccountModel>> GetUserAccountPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select userAccountId, 
                                            accessRightId, 
                                            firstName, 
                                            middleInitial, 
                                            lastName, 
                                            image, 
                                            email, 
                                            phone, 
                                            inactive, 
                                            accountExpiry, 
                                            dateCreated, 
                                            dateModified, 
                                            createdBy, 
                                            modifiedBy 
                                     from UserAccount 
                                        where userAccountId like @searchKey or 
                                                accessRightId like @searchKey or 
                                                firstName like @searchKey or 
                                                middleInitial like @searchKey or 
                                                lastName like @searchKey or 
                                                image like @searchKey or 
                                                email like @searchKey or 
                                                phone like @searchKey or 
                                                inactive like @searchKey or 
                                                accountExpiry like @searchKey or 
                                                dateCreated like @searchKey or 
                                                dateModified like @searchKey or 
                                                createdBy like @searchKey or 
                                                modifiedBy like @searchKey  
                                                limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<UserAccountModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<UserAccountPagedMdl?> GetUserAccSrchPaged(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select userAccountId, 
                                            accessRightId, 
                                            firstName, 
                                            middleInitial, 
                                            lastName, 
                                            image, 
                                            email, 
                                            phone, 
                                            inactive, 
                                            accountExpiry, 
                                            dateCreated, 
                                            dateModified, 
                                            createdBy, 
                                            modifiedBy 
                                     from UserAccount ";

                string strFltr = @"where userAccountId like @searchKey or 
                                                accessRightId like @searchKey or 
                                                firstName like @searchKey or 
                                                middleInitial like @searchKey or 
                                                lastName like @searchKey or 
                                                image like @searchKey or 
                                                email like @searchKey or 
                                                phone like @searchKey or 
                                                inactive like @searchKey or 
                                                accountExpiry like @searchKey or 
                                                dateCreated like @searchKey or 
                                                dateModified like @searchKey or 
                                                createdBy like @searchKey or 
                                                modifiedBy like @searchKey ";

                strQry += strFltr + "order by lastName limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);

                var ret = await db.QueryAsync<UserAccountModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    string strPgQry = $"select count(userAccountId) from useraccount {strFltr}";
                    var pgParam = new DynamicParameters();
                    pgParam.Add("@searchKey", $"%{searchKey}%");

                    var pageDetail = await PagingRepo.GetPageDetail(db, strPgQry, pgParam, pageNum, pageItem, ret.Count());

                    return new UserAccountPagedMdl()
                    {
                        Pagination = pageDetail,
                        UserAccount = ret
                    };
                }
            }

            return null;
        }

        public async Task<UserAccountModel> GetUserAccountById(string userAccountId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select userAccountId, 
                                            accessRightId, 
                                            firstName, 
                                            middleInitial, 
                                            lastName, 
                                            image, 
                                            email, 
                                            phone, 
                                            inactive, 
                                            accountExpiry, 
                                            dateCreated, 
                                            dateModified, 
                                            createdBy, 
                                            modifiedBy 
                                     from UserAccount 
                                        where userAccountId = @userAccountId";

                var param = new DynamicParameters();
                param.Add("@userAccountId", userAccountId);
                return await db.QuerySingleOrDefaultAsync<UserAccountModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<UserAccountModel> GetUserAccWithPassById(string userAccountId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select userAccountId, 
                                            password, 
                                            accessRightId, 
                                            firstName, 
                                            middleInitial, 
                                            lastName, 
                                            image, 
                                            email, 
                                            phone, 
                                            inactive, 
                                            accountExpiry, 
                                            dateCreated, 
                                            dateModified, 
                                            createdBy, 
                                            modifiedBy 
                                     from UserAccount 
                                        where userAccountId = @userAccountId";

                var param = new DynamicParameters();
                param.Add("@userAccountId", userAccountId);
                return await db.QuerySingleOrDefaultAsync<UserAccountModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<UserAccountModel>> GetUserAccountPgFiltered(UserAccountFilterMdl filter, int pageNum, int pageItem)
        {
            string strQry = "select * from useraccount";
            string strFltr = " where ";
            var param = new DynamicParameters();

            if (!string.IsNullOrEmpty(filter.AccessRightId))
            {
                strFltr += $"accessRightId = @accessRightId ";
                param.Add("@accessRightId", filter.AccessRightId);
            }

            // set inactive param
            if (filter.Inactive != null)
            {
                if (strFltr != " where ")
                {
                    strFltr += " and ";
                }

                strFltr += $"inactive = @inactive ";
                param.Add("@inactive", filter.Inactive);
            }

            if (filter.AccountExpiry != null)
            {
                if (strFltr != " where ")
                {
                    strFltr += " and ";
                }

                strFltr += $"date(accountExpiry) = @accountExpiry ";
                param.Add("@accountExpiry", filter.AccountExpiry);
            }

            // build order by and paging
            strQry += strFltr + $" order by lastName limit @pageItem offset @offset";

            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            param.Add("@pageItem", pageItem);
            param.Add("@offset", offset);

            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                return await db.QueryAsync<UserAccountModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<UserAccountPagedMdl?> GetUserAccFltrPaged(UserAccountFilterMdl filter, int pageNum, int pageItem)
        {
            string strQry = "select * from useraccount";
            string strFltr = " where ";
            var param = new DynamicParameters();
            var pgParam = new DynamicParameters();

            if (!string.IsNullOrEmpty(filter.AccessRightId))
            {
                strFltr += $"accessRightId = @accessRightId ";
                param.Add("@accessRightId", filter.AccessRightId);
            }

            // set inactive param
            if (filter.Inactive != null)
            {
                if (strFltr != " where ")
                {
                    strFltr += " and ";
                }

                strFltr += $"inactive = @inactive ";
                param.Add("@inactive", filter.Inactive);
            }

            if (filter.AccountExpiry != null)
            {
                if (strFltr != " where ")
                {
                    strFltr += " and ";
                }

                strFltr += $"date(accountExpiry) = @accountExpiry ";
                param.Add("@accountExpiry", filter.AccountExpiry);
            }

            // build order by and paging
            strQry += strFltr + $" order by lastName limit @pageItem offset @offset";

            // set paging param
            pgParam = param;

            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            param.Add("@pageItem", pageItem);
            param.Add("@offset", offset);

            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                var ret = await db.QueryAsync<UserAccountModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    string strPgQry = $"select count(userAccountId) from useraccount {strFltr}";
                    var pageDetail = await PagingRepo.GetPageDetail(db, strPgQry, pgParam, pageNum, pageItem, ret.Count());

                    return new UserAccountPagedMdl()
                    {
                        Pagination = pageDetail,
                        UserAccount = ret
                    };
                }
            }

            return null;
        }

        public async Task<bool> UserAccountExists(string userAccountId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select count(userAccountId) from UserAccount where 
														userAccountId = @userAccountId";

                var param = new DynamicParameters();
                param.Add("@userAccountId", userAccountId);

                var res = await db.ExecuteScalarAsync<int>(strQry, param);
                if (res == 0)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<IEnumerable<UserAccountModel>> GetUsrAccByAccessRightIdPg(string accessRightId, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select userAccountId, 
                                            accessRightId, 
                                            firstName, 
                                            middleInitial, 
                                            lastName, 
                                            image, 
                                            email, 
                                            phone, 
                                            inactive, 
                                            accountExpiry, 
                                            dateCreated, 
                                            dateModified, 
                                            createdBy, 
                                            modifiedBy 
                                     from UserAccount 
                                        where accessRightId = @accessRightId limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@accessRightId", accessRightId);
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);

                return await db.QueryAsync<UserAccountModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<CreateUserAccResultCode> CreateUserAccount(UserAccountModelMod userAccount)
        {
            // check if user acc data is valid
            if (userAccount.UserAccData == null)
            {
                return CreateUserAccResultCode.INVALIDUSERACCDATA;
            }

            // check if similar userAccountId exists
            var userExists = await UserAccountExists(userAccount.UserAccData.UserAccountId);
            if (userExists)
            {
                return CreateUserAccResultCode.SIMILARUSERACCID;
            }

            // set user acc
            var usrAccData = userAccount.UserAccData;

            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                // encrypt password
                usrAccData.Password = PassHash.CreateHash(usrAccData.Password);

                string strQry = @"insert into UserAccount(userAccountId, 
														password, 
														accessRightId, 
														firstName, 
														middleInitial, 
														lastName, 
														image, 
														email, 
														phone, 
														inactive, 
														accountExpiry, 
														createdBy, 
														modifiedBy)
 												values(@userAccountId, 
														@password, 
														@accessRightId, 
														@firstName, 
														@middleInitial, 
														@lastName, 
														@image, 
														@email, 
														@phone, 
														@inactive, 
														@accountExpiry, 
														@createdBy, 
														@modifiedBy)";

                int res = await db.ExecuteAsync(strQry, usrAccData);
                if (res > 0)
                {
                    // init useraccount user fields default data
                    var initPOUFld = await UsrAccUFieldRepo.InitUserAccUField(db, usrAccData.UserAccountId);
                    if (!initPOUFld)
                    {
                        return CreateUserAccResultCode.USRFIELDSAVEFAILED;
                    }

                    if (userAccount.UserAccUfields != null)
                    {
                        // insert useraccount user fields values
                        var uFieldsCreated = await UsrAccUFieldRepo.UpdateUserAccUField(db, usrAccData.UserAccountId, usrAccData.CreatedBy, userAccount.UserAccUfields);
                        if (!uFieldsCreated)
                        {
                            return CreateUserAccResultCode.USRFIELDSAVEFAILED;
                        }
                    }

                    // hide password before audit
                    usrAccData.Password = "password_removed";

                    // log audit
                    var audit = await AuditBuilder.BuildTranAuditADD(usrAccData);

                    if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                    {
                        return CreateUserAccResultCode.SUCCESS;
                    }
                }
            }

            return CreateUserAccResultCode.FAILED;
        }

        public async Task<bool> UpdateUserAccount(UserAccountModelMod userAccount)
        {
            // check if user acc data is valid
            if (userAccount.UserAccData == null)
            {
                return false;
            }

            // set user acc
            var usrAccData = userAccount.UserAccData;

            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                string strQry = @"update UserAccount set accessRightId = @accessRightId, 
														firstName = @firstName, 
														middleInitial = @middleInitial, 
														lastName = @lastName, 
														image = @image, 
														email = @email, 
														phone = @phone, 
														inactive = @inactive, 
														accountExpiry = @accountExpiry, 
														modifiedBy = @modifiedBy ";

                string strFLtr = @"where userAccountId = @userAccountId";

                // encrypt password
                if (!string.IsNullOrEmpty(usrAccData.Password))
                {
                    usrAccData.Password = PassHash.CreateHash(usrAccData.Password);

                    // include password update on query
                    strQry += "password = @password, ";
                }

                // finalize query
                strQry += strFLtr;

                int res = await db.ExecuteAsync(strQry, usrAccData);
                if (res > 0)
                {
                    // update useraccouont user fields values
                    if (userAccount.UserAccUfields != null)
                    {
                        var uFieldsCreated = await UsrAccUFieldRepo.UpdateUserAccUFieldMOD(db, usrAccData.UserAccountId, usrAccData.ModifiedBy, userAccount.UserAccUfields);
                        if (!uFieldsCreated)
                        {
                            return false;
                        }
                    }

                    // remove password before audit
                    usrAccData.Password = "password_removed";

                    // log audit
                    var audit = await AuditBuilder.BuildTranAuditMOD(usrAccData);

                    if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        // place here InUse checker function

        public async Task<bool> DeleteUserAccount(string delUserAccountId, string userAccountId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from UserAccount where 
														userAccountId = @delUserAccountId";
                var param = new DynamicParameters();
                param.Add("@delUserAccountId", delUserAccountId);
                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> DeActUserAccount(string delUserAccountId, string userAccountId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                string strQry = @"update UserAccount set inactive = true where 
														userAccountId = @delUserAccountId";
                var param = new DynamicParameters();
                param.Add("@delUserAccountId", delUserAccountId);
                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    var audit = await AuditBuilder.BuildTranAuditDeAct(delUserAccountId, userAccountId);

                    if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<bool> ReActUserAccount(string delUserAccountId, string userAccountId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                string strQry = @"update UserAccount set inactive = false where 
														userAccountId = @delUserAccountId";
                var param = new DynamicParameters();
                param.Add("@delUserAccountId", delUserAccountId);
                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    var audit = await AuditBuilder.BuildTranAuditReAct(delUserAccountId, userAccountId);

                    if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<AccValidationResult> UserAccountValidationWeb(LoginCredentailsModel account)
        {
            var ret = new AccValidationResult();

            // get user account by id
            var userDetail = await GetUserAccWithPassById(account.Username);
            if (userDetail == null)
            {
                ret.ResultCode = LoginResultCode.INVALIDUSER;
                return ret;
            }

            // validate hashed password
            var validPass = PassHash.ValidatePassword(account.Password, userDetail.Password);
            if (!validPass)
            {
                ret.ResultCode = LoginResultCode.INVALIDUSER;
                return ret;
            }

            // validate account if still active
            if (userDetail.Inactive > 0)
            {
                ret.ResultCode = LoginResultCode.INACTIVEACCOUNT;
                return ret;
            }

            // check account expiration
            if (userDetail.AccountExpiry <= DateTime.Now)
            {
                ret.ResultCode = LoginResultCode.EXPIREDACCOUNT;
                return ret;
            }

            // get user access rights
            var accessRights = await AccessRightDtlRepo.GetUserAllowedActionsWeb(userDetail.AccessRightId);
            if (accessRights == null || !accessRights.Any())
            {
                ret.ResultCode = LoginResultCode.FAILEDTOGETACCESSRIGHTS;
                return ret;
            }

            // get parent modules
            var parents = accessRights.Where(x => x.ParentId == null).ToList();

            // group modules by parent
            var groupedActions = parents.Select(x => new GroupedUserAllowedActions()
            {
                ModuleId = x.ModuleId,
                ModuleName = x.ModuleName,
                Actions = x.Actions,
                SeqNum = x.SeqNum,
                ParentId = x.ParentId,
                Url = x.Url,
                Icon = x.Icon,
                Image = x.Image,
                EnvTypeId = x.EnvTypeId,
                ChildModules = accessRights.Where(y => y.ParentId == x.ModuleId).ToList()
            });

            // success return
            if (userDetail != null &&
                accessRights != null &&
                accessRights.Any())
            {
                // remove password before return
                userDetail.Password = "password_removed";

                ret.ResultCode = LoginResultCode.SUCCESS;
                ret.UserAccount = userDetail;
                ret.WebUserAccessRights = groupedActions;
            }

            // simply return failed result
            return ret;
        }

        public async Task<AccValidationResult> UserAccountValidationMob(LoginCredentailsModel account)
        {
            var ret = new AccValidationResult();

            // get user account by id
            var userDetail = await GetUserAccWithPassById(account.Username);
            if (userDetail == null)
            {
                ret.ResultCode = LoginResultCode.INVALIDUSER;
                return ret;
            }

            // validate hashed password
            var validPass = PassHash.ValidatePassword(account.Password, userDetail.Password);
            if (!validPass)
            {
                ret.ResultCode = LoginResultCode.INVALIDUSER;
                return ret;
            }

            // validate account if still active
            if (userDetail.Inactive > 0)
            {
                ret.ResultCode = LoginResultCode.INACTIVEACCOUNT;
                return ret;
            }

            // check account expiration
            if (userDetail.AccountExpiry <= DateTime.Now)
            {
                ret.ResultCode = LoginResultCode.EXPIREDACCOUNT;
                return ret;
            }

            // get user access rights
            var accessRights = await AccessRightDtlRepo.GetUserAllowedActionsMob(userDetail.AccessRightId);
            if (accessRights == null || !accessRights.Any())
            {
                ret.ResultCode = LoginResultCode.FAILEDTOGETACCESSRIGHTS;
                return ret;
            }

            // success return
            if (userDetail != null &&
                accessRights != null &&
                accessRights.Any())
            {
                // remove password before return
                userDetail.Password = "password_removed";

                ret.ResultCode = LoginResultCode.SUCCESS;
                ret.UserAccount = userDetail;
                ret.MobUserAccessRights = accessRights;
            }

            // simply return failed result
            return ret;
        }

    }
}
