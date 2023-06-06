using aims_api.Models;
using aims_api.Utilities.Interface;
using aims_printsvc.Enums;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Utilities
{
    public class PrintHelper
    {
        private string ConnString;

        public PrintHelper(ITenantProvider tenantProvider)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
        }

        public async Task<IEnumerable<ZPLDetail>> BuildZplDetails(List<BCodeLabelToPrintModel> docsToPrint)
        {
            var zplDetails = new List<ZPLDetail>();

            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                foreach (var itm in docsToPrint)
                {
                    // build specific zpl code
                    var docTypeId = (itm.DocType).ToString();

                    switch (itm.DocType)
                    {
                        // for labels having description and barcode content only
                        case PrinterDocType.TrackId:
                        case PrinterDocType.LotAttId:
                        case PrinterDocType.LPNId:
                            var res = await DefineLabelZPL(db, docTypeId, itm);
                            if (res != null && res.Any()) zplDetails.AddRange(res);
                            break;

                        default:
                            break;
                    }
                }
            }

            return zplDetails;
        }

        private async Task<IEnumerable<ZPLDetail>> GetZPLByTypeId(IDbConnection db, string docTypeId)
        {
            string strQry = @"select * from zpldetails where docTypeId = @docTypeId";

            var param = new DynamicParameters();
            param.Add("@docTypeId", docTypeId);
            return await db.QueryAsync<ZPLDetail>(strQry, param, commandType: CommandType.Text);
        }

        private async Task<IEnumerable<ZPLDetail>> DefineLabelZPL(IDbConnection db, string docTypeId, BCodeLabelToPrintModel data)
        {
            var zplDetails = await GetZPLByTypeId(db, docTypeId);
            var ret = new List<ZPLDetail>();

            if (zplDetails.Any())
            {
                foreach (var zpl in zplDetails)
                {
                    string? zplString = await BytesToStringConverted(zpl.ZPLCode);

                    if (!string.IsNullOrEmpty(zplString))
                    {
                        zplString = zplString.Replace("@description@", data.Description);
                        zplString = zplString.Replace("@barcode@", data.BarcodeContent);

                        zpl.ZPLLines = zplString;

                        // remove zpl code to lessen data traffic
                        zpl.ZPLCode = null;

                        ret.Add(zpl);
                    }
                }
            }

            return ret;
        }

        private async Task<string?> BytesToStringConverted(byte[] bytes)
        {
            MemoryStream m = new MemoryStream(bytes);
            StreamReader file = new StreamReader(m);
            string zplByteStr = await file.ReadToEndAsync();

            if (!string.IsNullOrEmpty(zplByteStr))
            {
                var zplBase64 = Convert.FromBase64String(zplByteStr);
                string zplString = Encoding.UTF8.GetString(zplBase64);
                return zplString;
            }

            return null;
        }
    }
}
