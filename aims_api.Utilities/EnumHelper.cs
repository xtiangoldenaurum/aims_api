using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace aims_api.Utilities
{
    public class EnumHelper
    {
        public async Task<string> GetDescription(Enum val)
        {
            return await Task.Run(() => {
                // get enum filed info
                var fieldInfo = val.GetType().GetField(val.ToString());

                if(fieldInfo != null)
                {
                    // get attributes in array
                    var arrAtt = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

                    if (arrAtt != null && arrAtt.Length > 0)
                    {
                        var descProps = arrAtt[0] as DescriptionAttribute;
                        if(descProps != null)
                        {
                            var description = descProps.Description;
                            if (!string.IsNullOrEmpty(description))
                            {
                                return description;
                            }
                        }
                    }
                }

                return "Failed to retreived actual response message.";
            });
        }
    }
}