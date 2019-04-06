using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace HHSBoard.Helpers
{
    public class ConvertHelper
    {
        public static object ConvertType(Type type, string value)
        {
            if (type == typeof(DateTime))
            {
                if (DateTime.TryParse(value, out DateTime date))
                {
                    return date;
                }

                return null;
            }
            else if (type == typeof(PickChart))
            {
                return (PickChart)int.Parse(value);
            }
            else if (type == typeof(bool))
            {
                return int.Parse(value);
            }
            else
            {
                return HttpUtility.HtmlEncode(value);
            }
        }
    }
}
