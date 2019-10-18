using HHSBoard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace HHSBoard.Extensions
{
    public static class SortingExtensions
    {
        public static IQueryable<T> Sort<T>(this IQueryable<T> queryable, Type type, string propertyName, string order)
        {
            if (string.IsNullOrEmpty(propertyName) || string.IsNullOrEmpty(order))
            {
                return queryable;
            }

            PropertyInfo sortProperty = type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (order.Equals("asc"))
            {
                queryable = queryable.OrderBy(c => sortProperty.GetValue(c, null));
            }
            else
            {
                queryable = queryable.OrderByDescending(c => sortProperty.GetValue(c, null));
            }

            return queryable;
        } 
    }
}
