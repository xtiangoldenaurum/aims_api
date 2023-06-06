using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Utilities
{
    public class DataValidator
    {
        public bool Invalid { get; set; }

        public List<DataFieldError> ErrorFields { get; set; } = new List<DataFieldError>();

        public async Task AddErrorField(string fieldName)
        {
            await Task.Run(() =>
            {
                var errorField = ErrorFields.FirstOrDefault(x => x.FieldName == fieldName);
                if (errorField == null)
                {
                    var errMessage = $"Empty field: {fieldName}";
                    ErrorFields.Add(new DataFieldError() { FieldName = fieldName, ErrorMessage = errMessage });
                    Invalid = true;
                }
            });
        }
    }
}
