using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Utilities
{
    public class QryHelper
    {
        public async Task<string?> GetUpdateQry(dynamic data)
        {
            if ((object)data != null)
            {
                return await Task.Run(() => {
                    var strQry = string.Empty;

                    Dictionary<string, string> temp = JsonConvert.DeserializeObject<Dictionary<string, string>>(Convert.ToString(data));

                    foreach (var itm in temp.Keys)
                    {
                        if (strQry.Length > 0)
                        {
                            strQry += ", ";
                        }

                        strQry += $"{itm} = @{itm}";
                    }

                    return strQry;
                });
            }

            return null;
        }

        public async Task<DynamicParameters?> GetParams(dynamic data)
        {
            if ((object)data != null)
            {
                return await Task.Run(() => {
                    var param = new DynamicParameters();

                    Dictionary<string, string> temp = JsonConvert.DeserializeObject<Dictionary<string, string>>(Convert.ToString(data));

                    foreach (var itm in temp.Keys)
                    {
                        param.Add($"@{itm}", temp[itm]);
                    }

                    return param;
                });
            }

            return null;
        }
    }
}
