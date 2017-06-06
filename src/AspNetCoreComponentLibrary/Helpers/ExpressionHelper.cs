using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
    public static class ExpressionHelper
    {
        public static Expression<Func<T, bool>> ComparePropertyWithConst<T, PropertyT>(string propertyName, PropertyT value)
        {
            var param = Expression.Parameter(typeof(T), "x");
            var key = typeof(T).GetProperty(propertyName);
            var lhs = Expression.Constant(value, typeof(PropertyT));
            var rhs = Expression.MakeMemberAccess(param, key);
            var body = Expression.Equal(lhs, rhs);
            var lambda = Expression.Lambda<Func<T, bool>>(body, param);

            return lambda;//.Compile();
        }
    }
}
