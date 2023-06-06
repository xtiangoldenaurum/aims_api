using aims_api.Models;
using aims_api.Repositories.Interface;
using aims_api.Utilities;
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

namespace aims_api.Repositories.Implementation
{
    public class PrintHelperRepository : IPrintHelperRepository
    {
        private string ConnString;
        EPCConverter EPCConverter;
        private string EPCPrintConversion;

        public PrintHelperRepository(ITenantProvider tenantProvider, EPCConverter epcConverter)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            EPCConverter = epcConverter;
            EPCPrintConversion = tenantProvider.GetTenant().EPCPrintConversion;
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
                        case PrinterDocType.SN:
                        case PrinterDocType.EPC:
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
            // remove word "Sys" which flags a system generated id doc type
            docTypeId = docTypeId.Replace("Sys", "");

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

                        // epc conversion
                        var tempEPC = data.EPC;

                        // apply epc print conversion on epc doctype
                        if (data.DocType == PrinterDocType.EPC)
                        {
                            tempEPC = await EPCConverter.ConvertString(EPCPrintConversion, tempEPC);
                        }
                        else // auto-hex IDs that are system generated such as POId, LPNId etc.
                        {
                            tempEPC = await EPCConverter.ConvertString("ToHex", tempEPC);
                        }
                        zpl.EPC = tempEPC;
                        zpl.UntouchedEPC = data.EPC;

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

        public async Task<IEnumerable<BCodeLabelToPrintModel>?> BuildSNZpls(IEnumerable<UniqueTagsModel> serials)
        {
            return await Task.Run(() =>
            {
                var ret = new List<BCodeLabelToPrintModel>();

                foreach (var sn in serials)
                {
                    var itm = new BCodeLabelToPrintModel()
                    {
                        DocType = PrinterDocType.SN,
                        DocTypeId = (PrinterDocType.SN).ToString(),
                        Description = sn.Serialno,
                        BarcodeContent = sn.Serialno,
                        EPC = sn.EPC
                    };

                    ret.Add(itm);
                }

                return ret;
            });
        }

        public async Task<IEnumerable<BCodeLabelToPrintModel>?> BuildEPCZpls(IEnumerable<UniqueTagsModel> tags)
        {
            return await Task.Run(() =>
            {
                var ret = new List<BCodeLabelToPrintModel>();

                foreach (var tag in tags)
                {
                    var itm = new BCodeLabelToPrintModel()
                    {
                        DocType = PrinterDocType.EPC,
                        DocTypeId = (PrinterDocType.EPC).ToString(),
                        Description = tag.EPC,
                        BarcodeContent = tag.EPC,
                        EPC = tag.EPC
                    };

                    ret.Add(itm);
                }

                return ret;
            });
        }

        public async Task<IEnumerable<BCodeLabelToPrintModel>?> BuildEPCZplsFromList(List<string> epcList)
        {
            return await Task.Run(() =>
            {
                var ret = new List<BCodeLabelToPrintModel>();

                foreach (var epc in epcList)
                {
                    var itm = new BCodeLabelToPrintModel()
                    {
                        DocType = PrinterDocType.SysEPC,
                        DocTypeId = (PrinterDocType.SysEPC).ToString(),
                        Description = epc,
                        BarcodeContent = epc,
                        EPC = epc
                    };

                    ret.Add(itm);
                }

                return ret;
            });
        }

        private async Task<string> ConvertString(string method, string origString)
        {
            return await Task.Run(() =>
            {
                string val = string.Empty;

                if (method == "ToAscii")
                {
                    for (int i = 0; i < origString.Length; i += 2)
                    {
                        string hs = string.Empty;

                        hs = origString.Substring(i, 2); // Hex string value sample  2 = two digit of number.
                        ulong decval = Convert.ToUInt64(hs, 16);  // Decration Value 
                        long deccc = Convert.ToInt64(hs, 16); // Decration character
                        char character = Convert.ToChar(deccc); // convert int to chart or value.
                        val += character;
                    }
                    return val;
                }
                else if (method == "ToHex")
                {
                    StringBuilder sb = new StringBuilder();

                    byte[] inputByte = Encoding.UTF8.GetBytes(origString);

                    foreach (byte HexType in inputByte)
                    {
                        sb.Append(string.Format("{0:x2}", HexType));
                    }
                    return sb.ToString();
                }
                else
                {
                    return origString;
                }
            });
        }
    }
}
