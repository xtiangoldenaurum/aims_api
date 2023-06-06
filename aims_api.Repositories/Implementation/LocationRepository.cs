using aims_api.Enums;
using aims_api.Models;
using aims_api.Repositories.AuditBuilder;
using aims_api.Repositories.Interface;
using aims_api.Utilities.Interface;
using Dapper;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;

namespace aims_api.Repositories.Implementation
{
    public class LocationRepository : ILocationRepository
    {
        private string ConnString;
        LocationAudit AuditBuilder;
        IAuditTrailRepository AuditTrailRepo;
        IPagingRepository PagingRepo;
        IIdNumberRepository IdNumberRepo;
        ILocationTypeRepository LocationTypRepo;

        public LocationRepository(ITenantProvider tenantProvider,
                                    IAuditTrailRepository auditTrailRepo,
                                    IIdNumberRepository idNumberRepo,
                                    ILocationTypeRepository locationTypRepo)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            AuditTrailRepo = auditTrailRepo;
            IdNumberRepo = idNumberRepo;
            LocationTypRepo = locationTypRepo;
            AuditBuilder = new LocationAudit();
            PagingRepo = new PagingRepository();
        }

        public async Task<IEnumerable<LocationModel>> GetLocationPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from Location limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<LocationModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<LocationPagedMdl?> GetLocationPaged(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from Location order by loactionName limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);

                var ret = await db.QueryAsync<LocationModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    string strPgQry = "select count(locationId) from location";
                    var pageDetail = await PagingRepo.GetPageDetail(db, strPgQry, pageNum, pageItem, ret.Count());

                    return new LocationPagedMdl()
                    {
                        Pagination = pageDetail,
                        Location = ret
                    };
                }
            }

            return null;
        }

        public async Task<LocationPagedMdl?> GetInbStatingLocationPaged(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * 
                                    from Location 
                                    where locationTypeId = 'INSTAGING' and 
                                            inactive = 0 
                                    order by loactionName limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);

                var ret = await db.QueryAsync<LocationModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    string strPgQry = "select count(locationId) from location where locationTypeId = 'INSTAGING' and inactive = 0";
                    var pageDetail = await PagingRepo.GetPageDetail(db, strPgQry, pageNum, pageItem, ret.Count());

                    return new LocationPagedMdl()
                    {
                        Pagination = pageDetail,
                        Location = ret
                    };
                }
            }

            return null;
        }

        public async Task<LocationPagedMdl?> GetOutStatingLocationPaged(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * 
                                    from Location 
                                    where locationTypeId = 'OUTSTAGING' and 
                                            inactive = 0 
                                    order by loactionName limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);

                var ret = await db.QueryAsync<LocationModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    string strPgQry = "select count(locationId) from location where locationTypeId = 'OUTSTAGING' and inactive = 0";
                    var pageDetail = await PagingRepo.GetPageDetail(db, strPgQry, pageNum, pageItem, ret.Count());

                    return new LocationPagedMdl()
                    {
                        Pagination = pageDetail,
                        Location = ret
                    };
                }
            }

            return null;
        }

        public async Task<IEnumerable<LocationModel>> GetLocationPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from Location where 
														locationId like @searchKey or 
														locationName like @searchKey or 
														description like @searchKey or 
														locationTypeId like @searchKey or 
														locationGroupId like @searchKey or 
														areaId like @searchKey or 
														validationCode like @searchKey or 
														aisleCode like @searchKey or 
														bayCode like @searchKey or 
														inactive like @searchKey or 
														dateCreated like @searchKey or 
														dateModified like @searchKey or 
														createdBy like @searchKey or 
														modifiedBy like @searchKey 
														limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<LocationModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<LocationPagedMdl?> GetLocationSrchPaged(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from Location ";
                string strFltr = @"where locationId like @searchKey or 
									        locationName like @searchKey or 
									        description like @searchKey or 
									        locationTypeId like @searchKey or 
									        locationGroupId like @searchKey or 
									        areaId like @searchKey or 
									        validationCode like @searchKey or 
									        aisleCode like @searchKey or 
									        bayCode like @searchKey or 
									        inactive like @searchKey or 
									        dateCreated like @searchKey or 
									        dateModified like @searchKey or 
									        createdBy like @searchKey or 
									        modifiedBy like @searchKey ";

                strQry += strFltr + "order by loactionName limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);

                var ret = await db.QueryAsync<LocationModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    string strPgQry = $"select count(locationId) from location {strFltr}";
                    var pgParam = new DynamicParameters();
                    pgParam.Add("@searchKey", $"%{searchKey}%");

                    var pageDetail = await PagingRepo.GetPageDetail(db, strPgQry, pgParam, pageNum, pageItem, ret.Count());

                    return new LocationPagedMdl()
                    {
                        Pagination = pageDetail,
                        Location = ret
                    };
                }
            }

            return null;
        }

        public async Task<LocationModel> GetLocationById(string locationId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from Location where 
														locationId = @locationId";

                var param = new DynamicParameters();
                param.Add("@locationId", locationId);
                return await db.QuerySingleOrDefaultAsync<LocationModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<LocationModel> GetLocationByVCode(string locVCode)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from Location where 
														validationCode = @validationCode";

                var param = new DynamicParameters();
                param.Add("@validationCode", locVCode);
                return await db.QuerySingleOrDefaultAsync<LocationModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<bool> LocationExists(string locationId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select locationId from Location where 
														locationId = @locationId";

                var param = new DynamicParameters();
                param.Add("@locationId", locationId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<IEnumerable<string>> GetDIstinctAisle()
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "SELECT DISTINCT aisleCode asile FROM location where aisleCode <> null or aisleCode <> ''";
                return await db.QueryAsync<string>(strQry, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<string>> GetDIstinctBay()
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "SELECT DISTINCT bayCode bay FROM location where bayCode <> null or bayCode <> '';";
                return await db.QueryAsync<string>(strQry, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<LocationModel>> GetLocationPgFiltered(LocationFilterMdl filter, int pageNum, int pageItem)
        {
            string strQry = "select * from location";
            string strFltr = " where ";
            var param = new DynamicParameters();

            if (!string.IsNullOrEmpty(filter.LocationTypeId))
            {
                strFltr += $"locationTypeId = @locationTypeId ";
                param.Add("@locationTypeId", filter.LocationTypeId);
            }

            if (!string.IsNullOrEmpty(filter.LocationGroupId))
            {
                if (strFltr != " where ")
                {
                    strFltr += " and ";
                }

                strFltr += $"locationGroupId = @locationGroupId ";
                param.Add("@locationGroupId", filter.LocationGroupId);
            }

            if (!string.IsNullOrEmpty(filter.AreaId))
            {
                if (strFltr != " where ")
                {
                    strFltr += " and ";
                }

                strFltr += $"areaId = @areaId ";
                param.Add("@areaId", filter.AreaId);
            }

            if (filter.Inactive != null)
            {
                if (strFltr != " where ")
                {
                    strFltr += " and ";
                }

                strFltr += $"inactive = @inactive ";
                param.Add("@inactive", filter.Inactive);
            }

            if (!string.IsNullOrEmpty(filter.AisleCode))
            {
                if (strFltr != " where ")
                {
                    strFltr += " and ";
                }

                strFltr += $"aisleCode = @aisleCode ";
                param.Add("@aisleCode", filter.AisleCode);
            }

            if (!string.IsNullOrEmpty(filter.BayCode))
            {
                if (strFltr != " where ")
                {
                    strFltr += " and ";
                }

                strFltr += $"bayCode = @bayCode ";
                param.Add("@bayCode", filter.BayCode);
            }

            // build order by and paging
            strQry += strFltr + $" order by locationName limit @pageItem offset @offset";

            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            param.Add("@pageItem", pageItem);
            param.Add("@offset", offset);

            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                return await db.QueryAsync<LocationModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<LocationPagedMdl?> GetLocationFltrPaged(LocationFilterMdl filter, int pageNum, int pageItem)
        {
            string strQry = "select * from location";
            string strFltr = " where ";
            var param = new DynamicParameters();
            var pgParam = new DynamicParameters();

            if (!string.IsNullOrEmpty(filter.LocationTypeId))
            {
                strFltr += $"locationTypeId = @locationTypeId ";
                param.Add("@locationTypeId", filter.LocationTypeId);
            }

            if (!string.IsNullOrEmpty(filter.LocationGroupId))
            {
                if (strFltr != " where ")
                {
                    strFltr += " and ";
                }

                strFltr += $"locationGroupId = @locationGroupId ";
                param.Add("@locationGroupId", filter.LocationGroupId);
            }

            if (!string.IsNullOrEmpty(filter.AreaId))
            {
                if (strFltr != " where ")
                {
                    strFltr += " and ";
                }

                strFltr += $"areaId = @areaId ";
                param.Add("@areaId", filter.AreaId);
            }

            if (filter.Inactive != null)
            {
                if (strFltr != " where ")
                {
                    strFltr += " and ";
                }

                strFltr += $"inactive = @inactive ";
                param.Add("@inactive", filter.Inactive);
            }

            if (!string.IsNullOrEmpty(filter.AisleCode))
            {
                if (strFltr != " where ")
                {
                    strFltr += " and ";
                }

                strFltr += $"aisleCode = @aisleCode ";
                param.Add("@aisleCode", filter.AisleCode);
            }

            if (!string.IsNullOrEmpty(filter.BayCode))
            {
                if (strFltr != " where ")
                {
                    strFltr += " and ";
                }

                strFltr += $"bayCode = @bayCode ";
                param.Add("@bayCode", filter.BayCode);
            }

            // build order by and paging
            strQry += strFltr + $" order by locationName limit @pageItem offset @offset";

            // set paging param
            pgParam = param;

            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            param.Add("@pageItem", pageItem);
            param.Add("@offset", offset);

            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                var ret = await db.QueryAsync<LocationModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    string strPgQry = $"select count(locationId) from location {strFltr}";
                    var pageDetail = await PagingRepo.GetPageDetail(db, strPgQry, pgParam, pageNum, pageItem, ret.Count());

                    return new LocationPagedMdl()
                    {
                        Pagination = pageDetail,
                        Location = ret
                    };
                }
            }

            return null;
        }

        public async Task<bool> CreateLocation(LocationModel location)
        {
            // generate location validation code
            string? locVCode = await IdNumberRepo.GetNxtDocNum("LOCVCODE", location.CreatedBy);

            if (!string.IsNullOrEmpty(locVCode))
            {
                // set location validation code
                location.ValidationCode = locVCode;

                using (IDbConnection db = new MySqlConnection(ConnString))
                {
                    db.Open();
                    var tran = db.BeginTransaction();

                    string strQry = @"insert into Location(locationId, 
														locationName, 
														description, 
														locationTypeId, 
														locationGroupId, 
														areaId, 
														validationCode, 
														aisleCode, 
														bayCode, 
														inactive, 
														createdBy, 
														modifiedBy)
 												values(@locationId, 
														@locationName, 
														@description, 
														@locationTypeId, 
														@locationGroupId, 
														@areaId, 
														@validationCode, 
														@aisleCode, 
														@bayCode, 
														@inactive, 
														@createdBy, 
														@modifiedBy)";

                    int res = await db.ExecuteAsync(strQry, location);

                    if (res > 0)
                    {
                        // log audit
                        var audit = await AuditBuilder.BuildTranAuditADD(location);

                        if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                        {
                            tran.Commit();
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public async Task<bool> UpdateLocation(LocationModel location)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                var tran = db.BeginTransaction();

                // get previous locationTypeId
                var locTypeId = await GetLocationTypeId(db, tran, location.LocationId);
                if (string.IsNullOrEmpty(locTypeId))
                {
                    return false;
                }

                // validate new locationTypeId
                if (locTypeId != location.LocationTypeId)
                {
                    var skipAlter = await ChkIsLocationInUsed(db, tran, location.LocationId);
                    if (skipAlter)
                    {
                        return false;
                    }
                }

                string strQry = @"update Location set 
														locationName = @locationName, 
														description = @description, 
														locationTypeId = @locationTypeId, 
														locationGroupId = @locationGroupId, 
														areaId = @areaId, 
														validationCode = @validationCode, 
														aisleCode = @aisleCode, 
														bayCode = @bayCode, 
														inactive = @inactive, 
														modifiedBy = @modifiedBy where 
														locationId = @locationId";

                int res = await db.ExecuteAsync(strQry, location);

                if (res > 0)
                {
                    // log audit
                    var audit = await AuditBuilder.BuildTranAuditMOD(location);

                    if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                    {
                        tran.Commit();
                        return true;
                    }

                }
            }

            return false;
        }

        // place here InUse checker function
        public async Task<bool> LocationInUse(string locationId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                string strQry = @"select count(inventoryId) from inventoryhistory 
                                    where locationFrom = @locationId or 
                                            locationTo = @locationId";

                var param = new DynamicParameters();
                param.Add("@locationId", locationId);
                var res = await db.ExecuteScalarAsync<int>(strQry, param);

                if (res == 0)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> DeleteLocation(string locationId, string userAccountId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                var tran = db.BeginTransaction();

                // get location detail first
                var location = await GetLocationById(locationId);

                if (location != null)
                {
                    string strQry = @"delete from Location where 
														locationId = @locationId";
                    var param = new DynamicParameters();
                    param.Add("@locationId", locationId);
                    int res = await db.ExecuteAsync(strQry, param);

                    if (res > 0)
                    {
                        // log audit
                        var audit = await AuditBuilder.BuildTranAuditDEL(location, userAccountId);

                        if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                        {
                            tran.Commit();
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public async Task<TargetLocModelMod> DefineTargetLocation(string locVCode)
        {
            var ret = new TargetLocModelMod();
            var targetLoc = new TargetLocModel();

            // get location detail binded on scanned validation code
            var location = await GetLocationByVCode(locVCode);
            if (location == null)
            {
                ret.ResultCode = PutawayResultCode.INVALIDLOCVCODE;
                return ret;
            }

            // get location type pallet restrictions
            var locType = await LocationTypRepo.GetLocationTypeById(location.LocationTypeId);
            if (locType == null)
            {
                ret.ResultCode = PutawayResultCode.FAILEDTOGETLOCTYPE;
                return ret;
            }

            // check if location is occupied or empty
            var res = await GetLocUsedLPNs(location.LocationId);

            if (res.Any())
            {
                // count non-empty pallets (has qty greater than 0)
                var lpnCount = res.Where(x => x.OccupantQty > 0).Count();

                // exit process if pallet found on non lpn location
                if (!locType.AllowPallet)
                {
                    ret.ResultCode = PutawayResultCode.PALLETFOUNDONNONPALLETLOC;
                    return ret;
                }

                // exit process if multi-pallet found on single pallet location
                if (locType.SinglePallet && lpnCount > 1)
                {
                    ret.ResultCode = PutawayResultCode.MULTIPALLETONSINGLEPALLETLOC;
                    return ret;
                }

                // allowed pallet and single pallet restricted conditions
                if (locType.AllowPallet && locType.SinglePallet && lpnCount < 2)
                {
                    // insist putaway to use this as LPNTo (re-use LPNTo)
                    if (lpnCount == 1)
                    {
                        // get location used lpnTo
                        var lpn = res.Where(x => x.OccupantQty > 0).FirstOrDefault();

                        if (lpn != null)
                        {
                            return await BuildFixedTargetLoc(locVCode, location, lpn.LPNTo);
                        }
                        else
                        {
                            ret.ResultCode = PutawayResultCode.FAILEDTOGETLPNTO;
                            return ret;
                        }
                    }

                    // allow manual LPNTo field entry
                    return await BuildAnyLPNTargetLoc(locVCode, location);
                }

                // allowed multi-pallet conditions
                if (locType.AllowPallet && !locType.SinglePallet)
                {
                    // allow manual LPNTo field entry
                    return await BuildAnyLPNTargetLoc(locVCode, location);
                }
            }
            else
            {
                // enable LPNTo field if pallet is allowed on locaiton
                if (locType.AllowPallet)
                {
                    return await BuildAnyLPNTargetLoc(locVCode, location);
                }

                // disable if pallet not allow on target location
                return await BuildNoLPNTargetLoc(locVCode, location);
            }

            ret.ResultCode = PutawayResultCode.FAILED;
            return ret;
        }

        public async Task<TargetLocModelMod> DefineTargetLocByLocId(string locationId)
        {
            var ret = new TargetLocModelMod();
            var targetLoc = new TargetLocModel();

            // get location detail
            var location = await GetLocationById(locationId);
            if (location == null)
            {
                ret.ResultCode = PutawayResultCode.INVALIDLOCVCODE;
                return ret;
            }

            // get location type pallet restrictions
            var locType = await LocationTypRepo.GetLocationTypeById(location.LocationTypeId);
            if (locType == null)
            {
                ret.ResultCode = PutawayResultCode.FAILEDTOGETLOCTYPE;
                return ret;
            }

            // check if location is occupied or empty
            var res = await GetLocUsedLPNs(location.LocationId);

            if (res.Any())
            {
                // count non-empty pallets (has qty greater than 0)
                var lpnCount = res.Where(x => x.OccupantQty > 0).Count();

                // exit process if pallet found on non lpn location
                if (!locType.AllowPallet)
                {
                    ret.ResultCode = PutawayResultCode.PALLETFOUNDONNONPALLETLOC;
                    return ret;
                }

                // exit process if multi-pallet found on single pallet location
                if (locType.SinglePallet && lpnCount > 1)
                {
                    ret.ResultCode = PutawayResultCode.MULTIPALLETONSINGLEPALLETLOC;
                    return ret;
                }

                // allowed pallet and single pallet restricted conditions
                if (locType.AllowPallet && locType.SinglePallet && lpnCount < 2)
                {
                    // insist putaway to use this as LPNTo (re-use LPNTo)
                    if (lpnCount == 1)
                    {
                        // get location used lpnTo
                        var lpn = res.Where(x => x.OccupantQty > 0).FirstOrDefault();

                        if (lpn != null)
                        {
                            return await BuildFixedTargetLoc(location.ValidationCode, location, lpn.LPNTo);
                        }
                        else
                        {
                            ret.ResultCode = PutawayResultCode.FAILEDTOGETLPNTO;
                            return ret;
                        }
                    }

                    // allow manual LPNTo field entry
                    return await BuildAnyLPNTargetLoc(location.ValidationCode, location);
                }

                // allowed multi-pallet conditions
                if (locType.AllowPallet && !locType.SinglePallet)
                {
                    // allow manual LPNTo field entry
                    return await BuildAnyLPNTargetLoc(location.ValidationCode, location);
                }
            }
            else
            {
                // enable LPNTo field if pallet is allowed on locaiton
                if (locType.AllowPallet)
                {
                    return await BuildAnyLPNTargetLoc(location.ValidationCode, location);
                }

                // disable if pallet not allow on target location
                return await BuildNoLPNTargetLoc(location.ValidationCode, location);
            }

            ret.ResultCode = PutawayResultCode.FAILED;
            return ret;
        }

        public async Task<IEnumerable<TargetLocModel>?> GetLocUsedLPNs(string locationId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"CALL `spGetLocationUsedLPNs`(@targetLoc)";

                var param = new DynamicParameters();
                param.Add("@targetLoc", locationId);

                return await db.QueryAsync<TargetLocModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<TargetLocModelMod> BuildFixedTargetLoc(string locVCode, LocationModel location, string lpnTo)
        {
            return await Task.Run(() =>
            {
                var ret = new TargetLocModelMod();
                var targetLoc = new TargetLocModel();

                // set success result code
                ret.ResultCode = PutawayResultCode.SUCCESS;

                // build target loc details
                targetLoc.ValidationCode = locVCode;
                targetLoc.LocationId = location.LocationId;
                targetLoc.LocationName = location.LocationName;
                targetLoc.LPNTo = lpnTo;
                targetLoc.EnableLPNField = false;

                ret.TargetLoc = targetLoc;
                return ret;
            });
        }

        public async Task<TargetLocModelMod> BuildAnyLPNTargetLoc(string locVCode, LocationModel location)
        {
            return await Task.Run(() =>
            {
                var ret = new TargetLocModelMod();
                var targetLoc = new TargetLocModel();

                // set success result code
                ret.ResultCode = PutawayResultCode.SUCCESS;

                // build target loc details
                targetLoc.ValidationCode = locVCode;
                targetLoc.LocationId = location.LocationId;
                targetLoc.LocationName = location.LocationName;
                targetLoc.LPNTo = "";
                targetLoc.EnableLPNField = true;

                ret.TargetLoc = targetLoc;
                return ret;
            });
        }

        public async Task<TargetLocModelMod> BuildNoLPNTargetLoc(string locVCode, LocationModel location)
        {
            return await Task.Run(() =>
            {
                var ret = new TargetLocModelMod();
                var targetLoc = new TargetLocModel();

                // set success result code
                ret.ResultCode = PutawayResultCode.SUCCESS;

                // build target loc details
                targetLoc.ValidationCode = locVCode;
                targetLoc.LocationId = location.LocationId;
                targetLoc.LocationName = location.LocationName;
                targetLoc.LPNTo = "";
                targetLoc.EnableLPNField = false;

                ret.TargetLoc = targetLoc;
                return ret;
            });
        }

        public async Task<TargetLocModelMod> DefineLPNPutawayLoc(string lpnId, string locVCode)
        {
            var res = await DefineTargetLocation(locVCode);

            if (res == null)
            {
                var ret = new TargetLocModelMod();
                ret.ResultCode = PutawayResultCode.FAILED;
                return ret;
            }

            if (res.ResultCode != PutawayResultCode.SUCCESS)
            {
                return res;
            }

            if (res.TargetLoc == null)
            {
                var ret = new TargetLocModelMod();
                ret.ResultCode = PutawayResultCode.FAILED;
                return ret;
            }

            if (!res.TargetLoc.EnableLPNField)
            {
                res.ResultCode = PutawayResultCode.INVALIDLPNLOCAION;
                return res;
            }

            res.TargetLoc.LPNTo = lpnId;
            return res;
        }

        private async Task<string> GetLocationTypeId(IDbConnection db, IDbTransaction tran, string locationId)
        {
            string strQry = @"select locationTypeId 
                                from Location 
                                where locationId = @locationId";

            var param = new DynamicParameters();
            param.Add("@locationId", locationId);

            return await db.ExecuteScalarAsync<string>(strQry, param, tran);
        }

        private async Task<bool> ChkIsLocationInUsed(IDbConnection db, IDbTransaction tran, string locationId)
        {
            string strQry = @"CALL `spChkLocationIsUsed`(@locationId)";

            var param = new DynamicParameters();
            param.Add("@locationId", locationId);

            var res = await db.ExecuteScalarAsync<int>(strQry, param, tran);

            if (res < 1)
            {
                return false;
            }

            return true;
        }

    }
}
