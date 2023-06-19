using aims_api.Enums;
using aims_api.Models;
using aims_api.Repositories.AuditBuilder;
using aims_api.Repositories.Interface;
using aims_api.Utilities.Interface;
using Dapper;
using ExcelDataReader;
using LumenWorks.Framework.IO.Csv;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Implementation
{
    public class SORepository : ISORepository
    {
        private string ConnString;
        IIdNumberRepository IdNumberRepo;
        ISODetailRepository SODetailRepo;
        ISOUserFieldRepository SOUFieldRepo;
        IAuditTrailRepository AuditTrailRepo;
        SOAudit AuditBuilder;

        public SORepository(ITenantProvider tenantProvider,
                            IAuditTrailRepository auditTrailRepo,
                            IIdNumberRepository idNumberRepo,
                            ISODetailRepository soDetailsRepo,
                            ISOUserFieldRepository soUFieldRepo
                            )
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            AuditTrailRepo = auditTrailRepo;
            IdNumberRepo = idNumberRepo;
            SODetailRepo = soDetailsRepo;
            SOUFieldRepo = soUFieldRepo;
            AuditBuilder = new SOAudit();
        }

        public async Task<IEnumerable<SOModel>> GetSOPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from SO limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<SOModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<SOModel>> GetSOPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from SO where 
														soId like @searchKey or 
														soTypeId like @searchKey or 
														refNumber like @searchKey or 
														refNumber2 like @searchKey or 
														consigneeId like @searchKey or 
														consigneeName like @searchKey or 
														consigneeAddress like @searchKey or 
														consigneeContact like @searchKey or 
														consigneeEmail like @searchKey or 
														supplierId like @searchKey or 
														supplierName like @searchKey or 
														supplierAddress like @searchKey or 
														supplierContact like @searchKey or 
														supplierEmail like @searchKey or 
														carrierId like @searchKey or 
														carrierName like @searchKey or 
														carrierAddress like @searchKey or 
														carrierContact like @searchKey or 
														carrierEmail like @searchKey or 
														orderCreateDate like @searchKey or 
														arrivalDate like @searchKey or 
														arrivalDate2 like @searchKey or 
														soGrossWeight like @searchKey or 
														itemTotalQty like @searchKey or 
														soStatusId like @searchKey or 
														dateCreated like @searchKey or 
														dateModified like @searchKey or 
														createdBy like @searchKey or 
														modifiedBy like @searchKey or 
														remarks like @searchKey or 
														limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<SOModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<SOModel> GetSOById(string soId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from SO where 
														soId = @soId";

                var param = new DynamicParameters();
                param.Add("@soId", soId);
                return await db.QuerySingleOrDefaultAsync<SOModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<bool> SOExists(string soId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select soId from SO where 
														soId = @soId";

                var param = new DynamicParameters();
                param.Add("@soId", soId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }
        public async Task<bool> SKUExistsInSO(IDbConnection db, string sku, string soId)
        {
            string strQry = @"select count(sku) from soDetail 
                                where sku = @sku and 
                                        soId = @soId";

            var param = new DynamicParameters();
            param.Add("@sku", sku);
            param.Add("@soId", soId);

            var res = await db.ExecuteScalarAsync<int>(strQry, param);
            if (res == 0)
            {
                return false;
            }

            // default true to ensure no conflict will occur on error
            return true;
        }

        public async Task<bool> CreateSO(IDbConnection db, SOModel so)
        {
            // define po status
            so.SoStatusId = (SOStatus.CREATED).ToString();

            string strQry = @"insert into SO(soId, 
														soTypeId, 
														refNumber, 
														refNumber2, 
														consigneeId, 
														consigneeName, 
														consigneeAddress, 
														consigneeContact, 
														consigneeEmail, 
														supplierId, 
														supplierName, 
														supplierAddress, 
														supplierContact, 
														supplierEmail, 
														carrierId, 
														carrierName, 
														carrierAddress, 
														carrierContact, 
														carrierEmail, 
														orderCreateDate, 
														arrivalDate, 
														arrivalDate2, 
														soGrossWeight, 
														itemTotalQty, 
														soStatusId, 
														createdBy, 
														modifiedBy, 
														remarks)
 												values(@soId, 
														@soTypeId, 
														@refNumber, 
														@refNumber2, 
														@consigneeId, 
														@consigneeName, 
														@consigneeAddress, 
														@consigneeContact, 
														@consigneeEmail, 
														@supplierId, 
														@supplierName, 
														@supplierAddress, 
														@supplierContact, 
														@supplierEmail, 
														@carrierId, 
														@carrierName, 
														@carrierAddress, 
														@carrierContact, 
														@carrierEmail, 
														@orderCreateDate, 
														@arrivalDate, 
														@arrivalDate2, 
														@soGrossWeight, 
														@itemTotalQty, 
														@soStatusId, 
														@createdBy, 
														@modifiedBy, 
														@remarks)";

            int res = await db.ExecuteAsync(strQry, so);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildTranAuditADD(so, TranType.PO);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<SOCreateTranResult> CreateSOMod(SOModelMod so)
        {
            // get SO id number
            var soId = await IdNumberRepo.GetNextIdNum("SO");

            if (!string.IsNullOrEmpty(soId) &&
                so.SOHeader != null &&
                so.SODetails != null)
            {
                using (IDbConnection db = new MySqlConnection(ConnString))
                {
                    db.Open();

                    // check if expected arrival date is valid
                    if (so.SOHeader.ArrivalDate != null)
                    {
                        if (so.SOHeader.OrderCreateDate > so.SOHeader.ArrivalDate)
                        {
                            return new SOCreateTranResult()
                            {
                                ResultCode = SOTranResultCode.INVALIDARRIVALDATE
                            };
                        }
                    }

                    // check if expected arrival2 date is valid
                    if (so.SOHeader.ArrivalDate2 != null)
                    {
                        if (so.SOHeader.OrderCreateDate > so.SOHeader.ArrivalDate2)
                        {
                            return new SOCreateTranResult()
                            {
                                ResultCode = SOTranResultCode.INVALIDARRIVALDATE
                            };
                        }
                    }

                    // check if SO primary reference number are unique
                    if (!string.IsNullOrEmpty(so.SOHeader.RefNumber))
                    {
                        var soCount = await ReferenceNumExists(db, so.SOHeader.RefNumber);
                        if (soCount > 0)
                        {
                            return new SOCreateTranResult()
                            {
                                ResultCode = SOTranResultCode.INVALIDREFNUMONE
                            };
                        }
                    }

                    // check if SO secondary reference number are unique
                    if (!string.IsNullOrEmpty(so.SOHeader.RefNumber2))
                    {
                        var poCount = await ReferenceNumExists(db, so.SOHeader.RefNumber2);
                        if (poCount > 0)
                        {
                            return new SOCreateTranResult()
                            {
                                ResultCode = SOTranResultCode.INVALIDREFNUMTWO
                            };
                        }
                    }

                    // create header
                    so.SOHeader.SoId = soId;
                    var headCreated = await CreateSO(db, so.SOHeader);

                    if (headCreated)
                    {
                        // init po user fields default data
                        var initPOUFld = await SOUFieldRepo.InitSOUField(db, soId);
                        if (!initPOUFld)
                        {
                            return new SOCreateTranResult()
                            {
                                ResultCode = SOTranResultCode.USRFIELDSAVEFAILED
                            };
                        }

                        // insert so user fields values
                        if (so.SOUfields != null)
                        {
                            var uFieldsCreated = await SOUFieldRepo.UpdateSOUField(db, soId, so.SOHeader.CreatedBy, so.SOUfields);
                            if (!uFieldsCreated)
                            {
                                return new SOCreateTranResult()
                                {
                                    ResultCode = SOTranResultCode.USRFIELDSAVEFAILED
                                };
                            }
                        }

                        // create detail
                        if (so.SODetails.Any())
                        {
                            var details = so.SODetails.ToList();

                            for (int i = 0; i < details.Count(); i++)
                            {
                                var detail = details[i];

                                // check if similar SKU exists under this SO
                                var skuExists = await SKUExistsInSO(db, detail.Sku, soId);
                                if (skuExists)
                                {
                                    return new SOCreateTranResult()
                                    {
                                        ResultCode = SOTranResultCode.SKUCONFLICT
                                    };
                                }

                                // set detail id, status and header so id
                                detail.SoLineId = $"{soId}-{i + 1}";
                                detail.SoLineStatusId = (SOLneStatus.CREATED).ToString();
                                detail.SoId = soId;

                                // create detail
                                bool dtlSaved = await SODetailRepo.CreateSODetailMod(db, detail);

                                // return false if either of detail failed to save
                                if (!dtlSaved)
                                {
                                    return new SOCreateTranResult()
                                    {
                                        ResultCode = SOTranResultCode.SOLINESAVEFAILED
                                    };
                                }
                            }
                        }

                        return new SOCreateTranResult()
                        {
                            ResultCode = SOTranResultCode.SUCCESS,
                            SOId = soId
                        };
                    }
                }
            }

            return new SOCreateTranResult()
            {
                ResultCode = SOTranResultCode.FAILED
            };
        }

        public async Task<SOTranResultCode> UpdateSOMod(SOModelMod so)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                // check if SO primary reference number are unique
                if (!string.IsNullOrEmpty(so.SOHeader.RefNumber))
                {
                    var soCount = await ReferenceNumExists(db, so.SOHeader.RefNumber, so.SOHeader.SoId);
                    if (soCount > 0)
                    {
                        return SOTranResultCode.INVALIDREFNUMONE;
                    }
                }

                // check if SO secondary reference number are unique
                if (!string.IsNullOrEmpty(so.SOHeader.RefNumber2))
                {
                    var poCount = await ReferenceNumExists(db, so.SOHeader.RefNumber2, so.SOHeader.SoId);
                    if (poCount > 0)
                    {
                        return SOTranResultCode.INVALIDREFNUMTWO;
                    }
                }

                // update header
                var modHeader = await UpdateSO(db, so.SOHeader, TranType.SO);

                if (modHeader)
                {
                    // update po user fields values
                    if (so.SOUfields != null)
                    {
                        var uFieldsCreated = await SOUFieldRepo.UpdateSOUFieldMOD(db, so.SOHeader.SoId, so.SOHeader.ModifiedBy, so.SOUfields);
                        if (!uFieldsCreated)
                        {
                            return SOTranResultCode.USRFIELDSAVEFAILED;
                        }
                    }

                    // update detail
                    if (so.SODetails != null && so.SODetails.Any())
                    {
                        var details = so.SODetails.ToList();

                        // get last po detail line number
                        var poDetailsFromDb = await SODetailRepo.LockSODetails(db, so.SOHeader.SoId);
                        var lastPOLneId = poDetailsFromDb.OrderByDescending(x => x.SoLineId).Select(y => y.SoLineId).FirstOrDefault();
                        int lastLneNum = 0;

                        if (!string.IsNullOrEmpty(lastPOLneId))
                        {
                            lastLneNum = Convert.ToInt32(lastPOLneId.Substring(lastPOLneId.LastIndexOf('-') + 1));
                        }
                        else
                        {
                            lastLneNum = 0;
                        }

                        for (int i = 0; i < details.Count(); i++)
                        {
                            var detail = details[i];
                            bool dtlSaved = false;

                            if (detail.SoLineId == null)
                            {
                                // check if similar SKU exists under this SO
                                var skuExists = await SKUExistsInSO(db, detail.Sku, so.SOHeader.SoId);
                                if (skuExists)
                                {
                                    return SOTranResultCode.SKUCONFLICT;
                                }

                                // detail concidered as new
                                // set detail id, status and header po id
                                lastLneNum += 1;
                                detail.SoLineId = $"{so.SOHeader.SoId}-{lastLneNum}";
                                detail.SoLineStatusId = (SOLneStatus.CREATED).ToString();
                                detail.SoId = so.SOHeader.SoId;

                                // create detail
                                dtlSaved = await SODetailRepo.CreateSODetailMod(db, detail);
                            }
                            else
                            {
                                // update existing details
                                var prevDetail = await SODetailRepo.GetSODetailByIdMod(db, detail.SoLineId);

                                if (prevDetail.SoLineStatusId == (SOLneStatus.CREATED).ToString())
                                {
                                    if (prevDetail != detail)
                                    {
                                        dtlSaved = await SODetailRepo.UpdateSODetailMod(db, detail, TranType.PO);
                                    }
                                }
                            }

                            // return false if either of detail failed to save
                            if (!dtlSaved)
                            {
                                return SOTranResultCode.SOLINESAVEFAILED;
                            }
                        }
                    }
                    return SOTranResultCode.SUCCESS;
                }
            }

            return SOTranResultCode.FAILED;
        }

        public async Task<bool> UpdateSO(IDbConnection db, SOModel so, TranType tranTyp)
        {
            string strQry = @"update SO set 
														soTypeId = @soTypeId, 
														refNumber = @refNumber, 
														refNumber2 = @refNumber2, 
														consigneeId = @consigneeId, 
														consigneeName = @consigneeName, 
														consigneeAddress = @consigneeAddress, 
														consigneeContact = @consigneeContact, 
														consigneeEmail = @consigneeEmail, 
														supplierId = @supplierId, 
														supplierName = @supplierName, 
														supplierAddress = @supplierAddress, 
														supplierContact = @supplierContact, 
														supplierEmail = @supplierEmail, 
														carrierId = @carrierId, 
														carrierName = @carrierName, 
														carrierAddress = @carrierAddress, 
														carrierContact = @carrierContact, 
														carrierEmail = @carrierEmail, 
														orderCreateDate = @orderCreateDate, 
														arrivalDate = @arrivalDate, 
														arrivalDate2 = @arrivalDate2, 
														soGrossWeight = @soGrossWeight, 
														itemTotalQty = @itemTotalQty, 
														soStatusId = @soStatusId, 
														modifiedBy = @modifiedBy, 
														remarks = @remarks where 
														soId = @soId";

            int res = await db.ExecuteAsync(strQry, so);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildTranAuditMOD(so, tranTyp);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }

        // place here InUse checker function

        public async Task<bool> DeleteSO(string soId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from SO where 
														soId = @soId";
                var param = new DynamicParameters();
                param.Add("@soId", soId);
                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }
        private async Task<int> ReferenceNumExists(IDbConnection db, string refNum)
        {
            string strQry = @"select count(soId) from so where refNumber = @refNum or refNumber2 = @refNum";

            var param = new DynamicParameters();
            param.Add("@refNum", refNum);

            return await db.ExecuteScalarAsync<int>(strQry, param);
        }

        private async Task<int> ReferenceNumExists(IDbConnection db, string refNum, string soId)
        {
            string strQry = @"select count(soId) from so 
                                where (refNumber = @refNum 
                                or refNumber2 = @refNum) 
                                and soId <> @soId";

            var param = new DynamicParameters();
            param.Add("@refNum", refNum);
            param.Add("@soId", soId);

            return await db.ExecuteScalarAsync<int>(strQry, param);
        }

        public async Task<IEnumerable<SOModel>> GetExportSO()
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "SELECT * FROM so";
                return await db.QueryAsync<SOModel>(strQry, commandType: CommandType.Text);
            }
        }

        public async Task<SOCreateTranResult> CreateBulkSO(IFormFile file, string path)
        {
            using (FileStream stream = new FileStream(path, FileMode.CreateNew))
            {
                await file.CopyToAsync(stream); //icopy sa path yung inupload
            }

            List<SOModelMod> Parameters = new List<SOModelMod>();

            if (file.FileName.ToLower().Contains(".csv"))
            {
                DataTable value = new DataTable();
                //Install Library : LumenWorksCsvReader 

                using (var csvReader = new CsvReader(new StreamReader(File.OpenRead(path)), true))
                {
                    value.Load(csvReader);
                };

                for (int i = 0; i < value.Rows.Count; i++)
                {
                    SOModelMod rows = new SOModelMod();
                    rows.SOHeader = new SOModel();
                    rows.SODetails = new List<SODetailModel>();
                    SODetailModel detail = new SODetailModel();

                    // get PO id number
                    var poId = await IdNumberRepo.GetNextIdNum("SO");
                    rows.SOHeader.SoId = poId;

                    rows.SOHeader.RefNumber = value.Rows[i][0] != null ? Convert.ToString(value.Rows[i][0]) : null;
                    if (string.IsNullOrEmpty(rows.SOHeader.RefNumber))
                    {
                        return new SOCreateTranResult()
                        {
                            ResultCode = SOTranResultCode.MISSINGREFNUMONE
                        };
                    }

                    rows.SOHeader.RefNumber2 = value.Rows[i][1] != null ? Convert.ToString(value.Rows[i][1]) : null;
                    if (string.IsNullOrEmpty(rows.SOHeader.RefNumber2))
                    {
                        rows.SOHeader.RefNumber2 = null;
                    }

                    string? orderDateValue = value.Rows[i][2]?.ToString();
                    if (string.IsNullOrEmpty(orderDateValue))
                    {
                        return new SOCreateTranResult()
                        {
                            ResultCode = SOTranResultCode.ORDERDATEISREQUIRED
                        };
                    }
                    else
                    {
                        DateTime orderDate;
                        if (!DateTime.TryParseExact(orderDateValue, "yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture, DateTimeStyles.None, out orderDate))
                        {
                            orderDate = DateTime.MinValue;
                        }
                        rows.SOHeader.OrderCreateDate = orderDate;
                    }

                    string? arrivalDateValue = value.Rows[i][3]?.ToString();
                    if (string.IsNullOrEmpty(arrivalDateValue))
                    {
                        rows.SOHeader.ArrivalDate = null;
                    }
                    else
                    {
                        DateTime arrivalDate;
                        if (!DateTime.TryParseExact(arrivalDateValue, "yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture, DateTimeStyles.None, out arrivalDate))
                        {
                            arrivalDate = DateTime.MinValue;
                        }
                        rows.SOHeader.ArrivalDate = arrivalDate;
                    }

                    // check if expected arrival date is valid
                    if (rows.SOHeader.ArrivalDate != null)
                    {
                        if (rows.SOHeader.OrderCreateDate > rows.SOHeader.ArrivalDate)
                        {
                            return new SOCreateTranResult()
                            {
                                ResultCode = SOTranResultCode.INVALIDARRIVALDATE
                            };
                        }
                    }

                    string? arrivalDate2Value = value.Rows[i][4]?.ToString();
                    if (string.IsNullOrEmpty(arrivalDate2Value))
                    {
                        rows.SOHeader.ArrivalDate2 = null;
                    }
                    else
                    {
                        DateTime arrivalDate2;
                        if (!DateTime.TryParseExact(arrivalDate2Value, "yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture, DateTimeStyles.None, out arrivalDate2))
                        {
                            arrivalDate2 = DateTime.MinValue;
                        }
                        rows.SOHeader.ArrivalDate2 = arrivalDate2;
                    }

                    //check if expected arrival2 date is valid
                    if (rows.SOHeader.ArrivalDate2 != null)
                    {
                        if (rows.SOHeader.OrderCreateDate > rows.SOHeader.ArrivalDate2)
                        {
                            return new SOCreateTranResult()
                            {
                                ResultCode = SOTranResultCode.INVALIDARRIVALDATE
                            };
                        }
                    }

                    rows.SOHeader.Remarks = value.Rows[i][7] != null ? Convert.ToString(value.Rows[i][7]) : null;
                    if (string.IsNullOrEmpty(rows.SOHeader.Remarks))
                    {
                        rows.SOHeader.Remarks = null;
                    }

                    rows.SOHeader.ConsigneeId = value.Rows[i][8] != null ? Convert.ToString(value.Rows[i][8]) : null;
                    if (string.IsNullOrEmpty(rows.SOHeader.ConsigneeId))
                    {
                        rows.SOHeader.ConsigneeId = null;
                    }
                    rows.SOHeader.ConsigneeName = value.Rows[i][9] != null ? Convert.ToString(value.Rows[i][9]) : null;
                    if (string.IsNullOrEmpty(rows.SOHeader.ConsigneeName))
                    {
                        rows.SOHeader.ConsigneeName = null;
                    }
                    rows.SOHeader.ConsigneeAddress = value.Rows[i][10] != null ? Convert.ToString(value.Rows[i][10]) : null;
                    if (string.IsNullOrEmpty(rows.SOHeader.ConsigneeAddress))
                    {
                        rows.SOHeader.ConsigneeAddress = null;
                    }
                    rows.SOHeader.ConsigneeContact = value.Rows[i][11] != null ? Convert.ToString(value.Rows[i][11]) : null;
                    if (string.IsNullOrEmpty(rows.SOHeader.ConsigneeContact))
                    {
                        rows.SOHeader.ConsigneeContact = null;
                    }
                    rows.SOHeader.ConsigneeEmail = value.Rows[i][12] != null ? Convert.ToString(value.Rows[i][12]) : null;
                    if (string.IsNullOrEmpty(rows.SOHeader.ConsigneeEmail))
                    {
                        rows.SOHeader.ConsigneeEmail = null;
                    }


                    rows.SOHeader.SupplierId = value.Rows[i][13] != null ? Convert.ToString(value.Rows[i][13]) : null;
                    if (string.IsNullOrEmpty(rows.SOHeader.SupplierId))
                    {
                        rows.SOHeader.SupplierId = null;
                    }
                    rows.SOHeader.SupplierName = value.Rows[i][14] != null ? Convert.ToString(value.Rows[i][14]) : null;
                    if (string.IsNullOrEmpty(rows.SOHeader.SupplierName))
                    {
                        rows.SOHeader.SupplierName = null;
                    }
                    rows.SOHeader.SupplierAddress = value.Rows[i][15] != null ? Convert.ToString(value.Rows[i][15]) : null;
                    if (string.IsNullOrEmpty(rows.SOHeader.SupplierAddress))
                    {
                        rows.SOHeader.SupplierAddress = null;
                    }
                    rows.SOHeader.SupplierContact = value.Rows[i][16] != null ? Convert.ToString(value.Rows[i][16]) : null;
                    if (string.IsNullOrEmpty(rows.SOHeader.SupplierContact))
                    {
                        rows.SOHeader.SupplierContact = null;
                    }
                    rows.SOHeader.SupplierEmail = value.Rows[i][17] != null ? Convert.ToString(value.Rows[i][17]) : null;
                    if (string.IsNullOrEmpty(rows.SOHeader.SupplierEmail))
                    {
                        rows.SOHeader.SupplierEmail = null;
                    }
                    rows.SOHeader.CarrierId = value.Rows[i][18] != null ? Convert.ToString(value.Rows[i][18]) : null;
                    if (string.IsNullOrEmpty(rows.SOHeader.CarrierId))
                    {
                        rows.SOHeader.CarrierId = null;
                    }
                    rows.SOHeader.CarrierName = value.Rows[i][19] != null ? Convert.ToString(value.Rows[i][19]) : null;
                    if (string.IsNullOrEmpty(rows.SOHeader.CarrierName))
                    {
                        rows.SOHeader.CarrierName = null;
                    }
                    rows.SOHeader.CarrierAddress = value.Rows[i][20] != null ? Convert.ToString(value.Rows[i][20]) : null;
                    if (string.IsNullOrEmpty(rows.SOHeader.CarrierAddress))
                    {
                        rows.SOHeader.CarrierAddress = null;
                    }
                    rows.SOHeader.CarrierContact = value.Rows[i][21] != null ? Convert.ToString(value.Rows[i][21]) : null;
                    if (string.IsNullOrEmpty(rows.SOHeader.CarrierContact))
                    {
                        rows.SOHeader.CarrierContact = null;
                    }
                    rows.SOHeader.CarrierEmail = value.Rows[i][22] != null ? Convert.ToString(value.Rows[i][22]) : null;
                    if (string.IsNullOrEmpty(rows.SOHeader.CarrierEmail))
                    {
                        rows.SOHeader.CarrierEmail = null;
                    }
                    DateTime currentDateTime = DateTime.Now;
                    rows.SOHeader.SoStatusId = SOStatus.CREATED.ToString();
                    rows.SOHeader.SoStatus = "Created";
                    rows.SOHeader.DateCreated = currentDateTime;
                    rows.SOHeader.DateModified = currentDateTime;
                    rows.SOHeader.CreatedBy = value.Rows[i][23] != null ? Convert.ToString(value.Rows[i][23]) : null;
                    rows.SOHeader.ModifiedBy = value.Rows[i][24] != null ? Convert.ToString(value.Rows[i][24]) : null;

                    // Populate SODetailModel
                    detail.Sku = value.Rows[i][5] != null ? Convert.ToString(value.Rows[i][5]) : null;
                    detail.orderQty = Convert.ToInt32(value.Rows[i][6]);
                    detail.DateCreated = currentDateTime;
                    detail.DateModified = currentDateTime;
                    detail.CreatedBy = value.Rows[i][23] != null ? Convert.ToString(value.Rows[i][23]) : null;
                    detail.ModifiedBy = value.Rows[i][24] != null ? Convert.ToString(value.Rows[i][24]) : null;
                    detail.Remarks = value.Rows[i][7] != null ? Convert.ToString(value.Rows[i][7]) : null;

                    // Add the detail to the PODetails collection
                    ((List<SODetailModel>)rows.SODetails).Add(detail);

                    Parameters.Add(rows);
                }

                if (Parameters.Count > 0)
                {
                    using (IDbConnection db = new MySqlConnection(ConnString))
                    {
                        db.Open();

                        foreach (SOModelMod rows in Parameters)
                        {
                            var parameters = new
                            {
                                soId = rows.SOHeader.SoId,
                                refNumber = rows.SOHeader.RefNumber,
                                refNumber2 = rows.SOHeader.RefNumber2,
                                orderCreateDate = rows.SOHeader.OrderCreateDate,
                                arrivalDate = rows.SOHeader.ArrivalDate,
                                arrivalDate2 = rows.SOHeader.ArrivalDate2,
                                remarks = rows.SOHeader.Remarks,
                                consigneeId = rows.SOHeader.ConsigneeId,
                                consigneeName = rows.SOHeader.ConsigneeName,
                                consigneeAddress = rows.SOHeader.ConsigneeAddress,
                                consigneeContact = rows.SOHeader.ConsigneeContact,
                                consigneeEmail = rows.SOHeader.ConsigneeEmail,
                                supplierId = rows.SOHeader.SupplierId,
                                supplierName = rows.SOHeader.SupplierName,
                                supplierAddress = rows.SOHeader.SupplierAddress,
                                supplierContact = rows.SOHeader.SupplierContact,
                                supplierEmail = rows.SOHeader.SupplierEmail,
                                carrierId = rows.SOHeader.CarrierId,
                                carrierName = rows.SOHeader.CarrierName,
                                carrierAddress = rows.SOHeader.CarrierAddress,
                                carrierContact = rows.SOHeader.CarrierContact,
                                carrierEmail = rows.SOHeader.CarrierEmail,
                                poStatusId = rows.SOHeader.SoStatusId,
                                poStatus = rows.SOHeader.SoStatus,
                                dateCreated = rows.SOHeader.DateCreated,
                                dateModified = rows.SOHeader.DateModified,
                                createdBy = rows.SOHeader.CreatedBy,
                                modifyBy = rows.SOHeader.ModifiedBy
                            };

                            // check if SO primary reference number are unique
                            if (!string.IsNullOrEmpty(rows.SOHeader.RefNumber))
                            {
                                var poCount = await ReferenceNumExists(db, rows.SOHeader.RefNumber);
                                if (poCount > 0)
                                {
                                    return new SOCreateTranResult()
                                    {
                                        ResultCode = SOTranResultCode.INVALIDREFNUMONE
                                    };
                                }
                            }

                            // check if SO secondary reference number are unique
                            if (!string.IsNullOrEmpty(rows.SOHeader.RefNumber2))
                            {
                                var soCount = await ReferenceNumExists(db, rows.SOHeader.RefNumber2);
                                if (soCount > 0)
                                {
                                    return new SOCreateTranResult()
                                    {
                                        ResultCode = SOTranResultCode.INVALIDREFNUMTWO
                                    };
                                }
                            }

                            // create header
                            var headCreated = await CreateSO(db, rows.SOHeader);

                            if (headCreated)
                            {
                                // init so user fields default data
                                var initSOUFld = await SOUFieldRepo.InitSOUField(db, rows.SOHeader.SoId);
                                if (!initSOUFld)
                                {
                                    return new SOCreateTranResult()
                                    {
                                        ResultCode = SOTranResultCode.USRFIELDSAVEFAILED
                                    };
                                }

                                // insert po user fields values
                                if (rows.SOUfields != null)
                                {
                                    var uFieldsCreated = await SOUFieldRepo.UpdateSOUField(db, rows.SOHeader.SoId, rows.SOHeader.CreatedBy, rows.SOUfields);
                                    if (!uFieldsCreated)
                                    {
                                        return new SOCreateTranResult()
                                        {
                                            ResultCode = SOTranResultCode.USRFIELDSAVEFAILED
                                        };
                                    }
                                }

                                // create detail
                                if (rows.SODetails.Any())
                                {
                                    var details = rows.SODetails.ToList();

                                    for (int i = 0; i < details.Count(); i++)
                                    {
                                        var detail = details[i];

                                        // check if similar SKU exists under this PO
                                        var skuExists = await SKUExistsInSO(db, detail.Sku, rows.SOHeader.SoId);
                                        if (skuExists)
                                        {
                                            return new SOCreateTranResult()
                                            {
                                                ResultCode = SOTranResultCode.SKUCONFLICT
                                            };
                                        }

                                        // set detail id, status and header po id
                                        detail.SoLineId = $"{rows.SOHeader.SoId}-{i + 1}";
                                        detail.SoLineStatusId = (SOLneStatus.CREATED).ToString();
                                        detail.SoId = rows.SOHeader.SoId;

                                        // create detail
                                        bool dtlSaved = await SODetailRepo.CreateSODetailMod(db, detail);

                                        // return false if either of detail failed to save
                                        if (!dtlSaved)
                                        {
                                            return new SOCreateTranResult()
                                            {
                                                ResultCode = SOTranResultCode.SOLINESAVEFAILED
                                            };
                                        }
                                    }
                                }
                            }
                        }

                        return new SOCreateTranResult()
                        {
                            ResultCode = SOTranResultCode.SUCCESS,
                            SOIds = Parameters.Select(p => p.SOHeader.SoId).ToArray()
                        };
                    }
                }
            }

            if (file.FileName.ToLower().Contains(".xlsx"))
            {
                DataSet dataSet;

                using (var stream = file.OpenReadStream())
                {
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();

                        // Validate the XLSX header
                        if (await ValidateXlsxHeader(worksheet))
                        {
                            FileStream filestream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                            IExcelDataReader reader = ExcelReaderFactory.CreateReader(filestream);
                            dataSet = reader.AsDataSet(
                                new ExcelDataSetConfiguration()
                                {
                                    UseColumnDataType = false,
                                    ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                                    {
                                        UseHeaderRow = true
                                    }

                                });

                            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                            {
                                SOModelMod rows = new SOModelMod();
                                rows.SOHeader = new SOModel();
                                rows.SODetails = new List<SODetailModel>();
                                SODetailModel detail = new SODetailModel();

                                // get SO id number
                                var soId = await IdNumberRepo.GetNextIdNum("SO");
                                rows.SOHeader.SoId = soId;

                                rows.SOHeader.RefNumber = dataSet.Tables[0].Rows[i].ItemArray[0] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[0]) : null;
                                if (string.IsNullOrEmpty(rows.SOHeader.RefNumber))
                                {
                                    return new SOCreateTranResult()
                                    {
                                        ResultCode = SOTranResultCode.MISSINGREFNUMONE
                                    };
                                }

                                rows.SOHeader.RefNumber2 = dataSet.Tables[0].Rows[i].ItemArray[0] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[1]) : null;
                                if (string.IsNullOrEmpty(rows.SOHeader.RefNumber2))
                                {
                                    rows.SOHeader.RefNumber2 = null;
                                }

                                string? orderDateValue = dataSet.Tables[0].Rows[i].ItemArray[2]?.ToString();
                                if (string.IsNullOrEmpty(orderDateValue))
                                {
                                    return new SOCreateTranResult()
                                    {
                                        ResultCode = SOTranResultCode.ORDERDATEISREQUIRED
                                    };
                                }
                                else
                                {
                                    DateTime orderDate;
                                    if (!DateTime.TryParseExact(orderDateValue, "yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture, DateTimeStyles.None, out orderDate))
                                    {
                                        orderDate = DateTime.MinValue;
                                    }
                                    rows.SOHeader.OrderCreateDate = orderDate;
                                }

                                string? arrivalDateValue = dataSet.Tables[0].Rows[i].ItemArray[3]?.ToString();
                                if (string.IsNullOrEmpty(arrivalDateValue))
                                {
                                    rows.SOHeader.ArrivalDate = null;
                                }
                                else
                                {
                                    DateTime arrivalDate;
                                    if (!DateTime.TryParseExact(arrivalDateValue, "yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture, DateTimeStyles.None, out arrivalDate))
                                    {
                                        arrivalDate = DateTime.MinValue;
                                    }
                                    rows.SOHeader.ArrivalDate = arrivalDate;
                                }

                                // check if expected arrival date is valid
                                if (rows.SOHeader.ArrivalDate != null)
                                {
                                    if (rows.SOHeader.OrderCreateDate > rows.SOHeader.ArrivalDate)
                                    {
                                        return new SOCreateTranResult()
                                        {
                                            ResultCode = SOTranResultCode.INVALIDARRIVALDATE
                                        };
                                    }
                                }
                                string? arrivalDate2Value = dataSet.Tables[0].Rows[i].ItemArray[4]?.ToString();
                                if (string.IsNullOrEmpty(arrivalDate2Value))
                                {
                                    rows.SOHeader.ArrivalDate2 = null;
                                }
                                else
                                {
                                    DateTime arrivalDate2;
                                    if (!DateTime.TryParseExact(arrivalDate2Value, "yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture, DateTimeStyles.None, out arrivalDate2))
                                    {
                                        arrivalDate2 = DateTime.MinValue;
                                    }
                                    rows.SOHeader.ArrivalDate2 = arrivalDate2;
                                }

                                //check if expected arrival2 date is valid
                                if (rows.SOHeader.ArrivalDate2 != null)
                                {
                                    if (rows.SOHeader.OrderCreateDate > rows.SOHeader.ArrivalDate2)
                                    {
                                        return new SOCreateTranResult()
                                        {
                                            ResultCode = SOTranResultCode.INVALIDARRIVALDATE
                                        };
                                    }
                                }

                                rows.SOHeader.Remarks = dataSet.Tables[0].Rows[i].ItemArray[7] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[7]) : null;
                                if (string.IsNullOrEmpty(rows.SOHeader.Remarks))
                                {
                                    rows.SOHeader.Remarks = null;
                                }

                                rows.SOHeader.ConsigneeId = dataSet.Tables[0].Rows[i].ItemArray[8] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[8]) : null;
                                if (string.IsNullOrEmpty(rows.SOHeader.ConsigneeId))
                                {
                                    rows.SOHeader.ConsigneeId = null;
                                }
                                rows.SOHeader.ConsigneeName = dataSet.Tables[0].Rows[i].ItemArray[9] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[9]) : null;
                                if (string.IsNullOrEmpty(rows.SOHeader.ConsigneeName))
                                {
                                    rows.SOHeader.ConsigneeName = null;
                                }
                                rows.SOHeader.ConsigneeAddress = dataSet.Tables[0].Rows[i].ItemArray[10] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[10]) : null;
                                if (string.IsNullOrEmpty(rows.SOHeader.ConsigneeAddress))
                                {
                                    rows.SOHeader.ConsigneeAddress = null;
                                }
                                rows.SOHeader.ConsigneeContact = dataSet.Tables[0].Rows[i].ItemArray[11] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[11]) : null;
                                if (string.IsNullOrEmpty(rows.SOHeader.ConsigneeContact))
                                {
                                    rows.SOHeader.ConsigneeContact = null;
                                }
                                rows.SOHeader.ConsigneeEmail = dataSet.Tables[0].Rows[i].ItemArray[12] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[12]) : null;
                                if (string.IsNullOrEmpty(rows.SOHeader.ConsigneeEmail))
                                {
                                    rows.SOHeader.ConsigneeEmail = null;
                                }

                                rows.SOHeader.SupplierId = dataSet.Tables[0].Rows[i].ItemArray[13] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[13]) : null;
                                if (string.IsNullOrEmpty(rows.SOHeader.SupplierId))
                                {
                                    rows.SOHeader.SupplierId = null;
                                }
                                rows.SOHeader.SupplierName = dataSet.Tables[0].Rows[i].ItemArray[14] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[14]) : null;
                                if (string.IsNullOrEmpty(rows.SOHeader.SupplierName))
                                {
                                    rows.SOHeader.SupplierName = null;
                                }
                                rows.SOHeader.SupplierAddress = dataSet.Tables[0].Rows[i].ItemArray[15] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[15]) : null;
                                if (string.IsNullOrEmpty(rows.SOHeader.SupplierAddress))
                                {
                                    rows.SOHeader.SupplierAddress = null;
                                }
                                rows.SOHeader.SupplierContact = dataSet.Tables[0].Rows[i].ItemArray[16] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[16]) : null;
                                if (string.IsNullOrEmpty(rows.SOHeader.SupplierContact))
                                {
                                    rows.SOHeader.SupplierContact = null;
                                }
                                rows.SOHeader.SupplierEmail = dataSet.Tables[0].Rows[i].ItemArray[17] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[17]) : null;
                                if (string.IsNullOrEmpty(rows.SOHeader.SupplierEmail))
                                {
                                    rows.SOHeader.SupplierEmail = null;
                                }
                                rows.SOHeader.CarrierId = dataSet.Tables[0].Rows[i].ItemArray[18] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[18]) : null;
                                if (string.IsNullOrEmpty(rows.SOHeader.CarrierId))
                                {
                                    rows.SOHeader.CarrierId = null;
                                }
                                rows.SOHeader.CarrierName = dataSet.Tables[0].Rows[i].ItemArray[19] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[19]) : null;
                                if (string.IsNullOrEmpty(rows.SOHeader.CarrierName))
                                {
                                    rows.SOHeader.CarrierName = null;
                                }
                                rows.SOHeader.CarrierAddress = dataSet.Tables[0].Rows[i].ItemArray[20] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[20]) : null;
                                if (string.IsNullOrEmpty(rows.SOHeader.CarrierAddress))
                                {
                                    rows.SOHeader.CarrierAddress = null;
                                }
                                rows.SOHeader.CarrierContact = dataSet.Tables[0].Rows[i].ItemArray[21] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[21]) : null;
                                if (string.IsNullOrEmpty(rows.SOHeader.CarrierContact))
                                {
                                    rows.SOHeader.CarrierContact = null;
                                }
                                rows.SOHeader.CarrierEmail = dataSet.Tables[0].Rows[i].ItemArray[22] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[22]) : null;
                                if (string.IsNullOrEmpty(rows.SOHeader.CarrierEmail))
                                {
                                    rows.SOHeader.CarrierEmail = null;
                                }
                                DateTime currentDateTime = DateTime.Now;
                                rows.SOHeader.SoStatusId = SOStatus.CREATED.ToString();
                                rows.SOHeader.SoStatus = "Created";
                                rows.SOHeader.DateCreated = currentDateTime;
                                rows.SOHeader.DateModified = currentDateTime;
                                rows.SOHeader.CreatedBy = dataSet.Tables[0].Rows[i].ItemArray[23] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[23]) : null;
                                rows.SOHeader.ModifiedBy = dataSet.Tables[0].Rows[i].ItemArray[24] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[24]) : null;

                                // Populate PODetailModel
                                detail.Sku = dataSet.Tables[0].Rows[i].ItemArray[5] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[5]) : null;
                                detail.orderQty = Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[6]);
                                detail.DateCreated = currentDateTime;
                                detail.DateModified = currentDateTime;
                                detail.CreatedBy = dataSet.Tables[0].Rows[i].ItemArray[23] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[23]) : null;
                                detail.ModifiedBy = dataSet.Tables[0].Rows[i].ItemArray[24] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[24]) : null;
                                detail.Remarks = dataSet.Tables[0].Rows[i].ItemArray[7] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[7]) : null;

                                // Add the detail to the PODetails collection
                                ((List<SODetailModel>)rows.SODetails).Add(detail);

                                Parameters.Add(rows);
                            }

                            filestream.Close();

                            if (Parameters.Count > 0)
                            {
                                using (IDbConnection db = new MySqlConnection(ConnString))
                                {
                                    db.Open();

                                    foreach (SOModelMod rows in Parameters)
                                    {
                                        var parameters = new
                                        {
                                            soId = rows.SOHeader.SoId,
                                            refNumber = rows.SOHeader.RefNumber,
                                            refNumber2 = rows.SOHeader.RefNumber2,
                                            orderCreateDate = rows.SOHeader.OrderCreateDate,
                                            arrivalDate = rows.SOHeader.ArrivalDate,
                                            arrivalDate2 = rows.SOHeader.ArrivalDate2,
                                            remarks = rows.SOHeader.Remarks,
                                            consigneeId = rows.SOHeader.ConsigneeId,
                                            consigneeName = rows.SOHeader.ConsigneeName,
                                            consigneeAddress = rows.SOHeader.ConsigneeAddress,
                                            consigneeContact = rows.SOHeader.ConsigneeContact,
                                            consigneeEmail = rows.SOHeader.ConsigneeEmail,
                                            supplierId = rows.SOHeader.SupplierId,
                                            supplierName = rows.SOHeader.SupplierName,
                                            supplierAddress = rows.SOHeader.SupplierAddress,
                                            supplierContact = rows.SOHeader.SupplierContact,
                                            supplierEmail = rows.SOHeader.SupplierEmail,
                                            carrierId = rows.SOHeader.CarrierId,
                                            carrierName = rows.SOHeader.CarrierName,
                                            carrierAddress = rows.SOHeader.CarrierAddress,
                                            carrierContact = rows.SOHeader.CarrierContact,
                                            carrierEmail = rows.SOHeader.CarrierEmail,
                                            poStatusId = rows.SOHeader.SoStatusId,
                                            poStatus = rows.SOHeader.SoStatus,
                                            dateCreated = rows.SOHeader.DateCreated,
                                            dateModified = rows.SOHeader.DateModified,
                                            createdBy = rows.SOHeader.CreatedBy,
                                            modifyBy = rows.SOHeader.ModifiedBy
                                        };

                                        // check if SO primary reference number are unique
                                        if (!string.IsNullOrEmpty(rows.SOHeader.RefNumber))
                                        {
                                            var soCount = await ReferenceNumExists(db, rows.SOHeader.RefNumber);
                                            if (soCount > 0)
                                            {
                                                return new SOCreateTranResult()
                                                {
                                                    ResultCode = SOTranResultCode.INVALIDREFNUMONE
                                                };
                                            }
                                        }

                                        // check if SO secondary reference number are unique
                                        if (!string.IsNullOrEmpty(rows.SOHeader.RefNumber2))
                                        {
                                            var poCount = await ReferenceNumExists(db, rows.SOHeader.RefNumber2);
                                            if (poCount > 0)
                                            {
                                                return new SOCreateTranResult()
                                                {
                                                    ResultCode = SOTranResultCode.INVALIDREFNUMTWO
                                                };
                                            }
                                        }

                                        // create header
                                        var headCreated = await CreateSO(db, rows.SOHeader);

                                        if (headCreated)
                                        {
                                            // init po user fields default data
                                            var initPOUFld = await SOUFieldRepo.InitSOUField(db, rows.SOHeader.SoId);
                                            if (!initPOUFld)
                                            {
                                                return new SOCreateTranResult()
                                                {
                                                    ResultCode = SOTranResultCode.USRFIELDSAVEFAILED
                                                };
                                            }

                                            // insert so user fields values
                                            if (rows.SOUfields != null)
                                            {
                                                var uFieldsCreated = await SOUFieldRepo.UpdateSOUField(db, rows.SOHeader.SoId, rows.SOHeader.CreatedBy, rows.SOUfields);
                                                if (!uFieldsCreated)
                                                {
                                                    return new SOCreateTranResult()
                                                    {
                                                        ResultCode = SOTranResultCode.USRFIELDSAVEFAILED
                                                    };
                                                }
                                            }

                                            // create detail
                                            if (rows.SODetails.Any())
                                            {
                                                var details = rows.SODetails.ToList();

                                                for (int i = 0; i < details.Count(); i++)
                                                {
                                                    var detail = details[i];

                                                    // check if similar SKU exists under this PO
                                                    var skuExists = await SKUExistsInSO(db, detail.Sku, rows.SOHeader.SoId);
                                                    if (skuExists)
                                                    {
                                                        return new SOCreateTranResult()
                                                        {
                                                            ResultCode = SOTranResultCode.SKUCONFLICT
                                                        };
                                                    }

                                                    // set detail id, status and header po id
                                                    detail.SoLineId = $"{rows.SOHeader.SoId}-{i + 1}";
                                                    detail.SoLineStatusId = (SOLneStatus.CREATED).ToString();
                                                    detail.SoId = rows.SOHeader.SoId;

                                                    // create detail
                                                    bool dtlSaved = await SODetailRepo.CreateSODetailMod(db, detail);

                                                    // return false if either of detail failed to save
                                                    if (!dtlSaved)
                                                    {
                                                        return new SOCreateTranResult()
                                                        {
                                                            ResultCode = SOTranResultCode.SOLINESAVEFAILED
                                                        };
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    return new SOCreateTranResult()
                                    {
                                        ResultCode = SOTranResultCode.SUCCESS,
                                        SOIds = Parameters.Select(p => p.SOHeader.SoId).ToArray()
                                    };
                                }
                            }
                        }
                        else
                        {
                            return new SOCreateTranResult()
                            {
                                ResultCode = SOTranResultCode.INVALIDHEADER
                            };
                        }
                    }
                }
            }

            return new SOCreateTranResult()
            {
                ResultCode = SOTranResultCode.FAILED
            };
        }

        public bool ValidateCsvHeader(string headerLine)
        {
            // Perform your validation logic here
            // Example validation: Check if the header contains specific column names
            string[] expectedHeaders = { "Reference Number", "2nd Reference Number", "Order Date Created",
                                         "Arrival Date", "Arrival Date 2", "SKU",
                                         "Order Qty", "Remarks", "Consignee Id",
                                         "Consignee Name", "Consignee Address", "Consignee Contact",
                                         "Consignee Email", "Supplier Id","Supplier Name",
                                         "Supplier Address", "Supplier Contact", "Supplier Email",
                                         "Carrier Id", "Carrier Name", "Carrier Address",
                                         "Carrier Contact", "Carrier Email", "Created By",
                                         "Modified By"
                                       };

            string[] actualHeaders = headerLine.Split(',');

            if (actualHeaders.Length != expectedHeaders.Length)
            {
                return false;
            }

            for (int i = 0; i < actualHeaders.Length; i++)
            {
                if (actualHeaders[i].Trim() != expectedHeaders[i])
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<bool> ValidateXlsxHeader(ExcelWorksheet worksheet)
        {
            string[] expectedHeaders = { "Reference Number", "2nd Reference Number", "Order Date Created",
                                         "Arrival Date", "Arrival Date 2", "SKU",
                                         "Order Qty", "Remarks", "Consignee Id",
                                         "Consignee Name", "Consignee Address", "Consignee Contact",
                                         "Consignee Email", "Supplier Id","Supplier Name",
                                         "Supplier Address", "Supplier Contact", "Supplier Email",
                                         "Carrier Id", "Carrier Name", "Carrier Address",
                                         "Carrier Contact", "Carrier Email", "Created By",
                                         "Modified By"
                                       };

            int columnCount = await Task.Run(() => worksheet.Dimension.Columns);

            if (columnCount != expectedHeaders.Length)
            {
                return false;
            }

            for (int column = 1; column <= columnCount; column++)
            {
                string? headerCell = await Task.Run(() => worksheet.Cells[1, column].Value?.ToString());

                if (headerCell?.Trim() != expectedHeaders[column - 1])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
