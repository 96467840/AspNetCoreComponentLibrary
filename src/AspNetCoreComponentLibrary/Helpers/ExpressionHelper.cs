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
        public static Expression NullString => Expression.Constant(null, typeof(string));
        public static Expression NullObject => Expression.Constant(null, typeof(object));

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

        public static Expression<Func<T, bool>> ComparePropertyWithConst<T>(string propertyName, Type PropertyType, object value, Func<Expression, Expression, Expression> expr)
        {
            var param = Expression.Parameter(typeof(T), "x");
            var key = typeof(T).GetProperty(propertyName);
            var lhs = Expression.MakeMemberAccess(param, key);

            //var rhs = Expression.Constant(Expression.Convert(value, PropertyType), PropertyType);
            Expression rhs = value == null ? (Expression)Expression.Convert(Expression.Constant(value), PropertyType) : Expression.Constant(value, PropertyType);// без Convert будет жопа с нулами
            
            var body = expr(lhs, rhs);

            var lambda = Expression.Lambda<Func<T, bool>>(body, param);

            return lambda;//.Compile();
        }

        public static Expression<Func<T, bool>> ComparePropertyWithConstArray<T>(string propertyName, Type PropertyType, List<object> values, Func<Expression, Expression, Expression> expr)
        {
            if (values.Count == 1) return ComparePropertyWithConst<T>(propertyName, PropertyType, values[0], expr);

            var param = Expression.Parameter(typeof(T), "x");
            var key = typeof(T).GetProperty(propertyName);
            var lhs = Expression.MakeMemberAccess(param, key);

            //var rhs = Expression.Constant(values[0], PropertyType);
            var rhs = values[0] == null ? (Expression)Expression.Convert(Expression.Constant(values[0]), PropertyType) : Expression.Constant(values[0], PropertyType);// без Convert будет жопа с нулами

            var body = expr(lhs, rhs);
            for (var i = 1; i < values.Count; i++)
            {
                Expression rhs1 = values[i] == null ? (Expression)Expression.Convert(Expression.Constant(values[i]), PropertyType) : Expression.Constant(values[i], PropertyType);
                
                body = Expression.OrElse(body, expr(lhs, rhs1));
            }
            
            var lambda = Expression.Lambda<Func<T, bool>>(body, param);

            return lambda;//.Compile();
        }

        public static Expression<Func<T, OptionVM>> SelectOptionVM<T>(PropertyInfo propValue, PropertyInfo propTitle, PropertyInfo propParent)
        {
            var param = Expression.Parameter(typeof(T), "x");

            // id не может быть нулом!!!
            var _idExpr = Expression.MakeMemberAccess(param, propValue);
            //var idExpr = Expression.Call(_idExpr, propValue.PropertyType.GetMethod("ToString", new Type[] { }));
            var idExprObj = Expression.Convert(_idExpr, typeof(object));

            // а title у нас может быть не строкой?
            var titleExpr = Expression.Convert(Expression.MakeMemberAccess(param, propTitle), typeof(string));

            // а на первом уровне парент будет нулл.
            // если так сделать будут не очень хорошие запросы в БД
            /*Expression parentExpr = propParent == null ?
                NullString :
                Expression.Condition(
                    Expression.Equal(Expression.MakeMemberAccess(param, propParent), Expression.Constant(null, propParent.PropertyType)),
                    NullString,
                    Expression.Call(Expression.MakeMemberAccess(param, propParent), propParent.PropertyType.GetMethod("ToString", new Type[] { }))
                );*/

            var parentExpr = propParent == null ?
                NullObject :
                Expression.Convert(Expression.MakeMemberAccess(param, propParent), typeof(object));

            var ctor = typeof(OptionVM).GetConstructor(new Type[] { typeof(object), typeof(string), typeof(object) });
            var body = Expression.New(ctor, idExprObj, titleExpr, parentExpr);

            var lambda = Expression.Lambda<Func<T, OptionVM>>(body, param);
            return lambda;
        }

        public static Func<Expression, Expression, Expression> GetCompareExpression(this EnumFilterCompare compare)
        {
            Func<Expression, Expression, Expression> expr = Expression.Equal;
            switch (compare)
            {
                case EnumFilterCompare.LT:
                    expr = Expression.LessThan;
                    break;
                case EnumFilterCompare.LTE:
                    expr = Expression.LessThanOrEqual;
                    break;
                case EnumFilterCompare.GT:
                    expr = Expression.GreaterThan;
                    break;
                case EnumFilterCompare.GTE:
                    expr = Expression.GreaterThanOrEqual;
                    break;
                case EnumFilterCompare.Begins:
                    expr = (Expression exp1, Expression exp2) =>
                    {
                        // увы с case ignore орм не может составить запрос и выполняет фильтрацию после запроса
                        // но мы можем вызвать ToLower(), но это тоже не вариант ибо индексы идут лесом, а хотя индексы и так идут лесом
                        var meth = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string)/*, typeof(StringComparison)*/ });
                        var func = Expression.Call(exp1, meth, exp2/*, Expression.Constant(StringComparison.OrdinalIgnoreCase, typeof(StringComparison))*/);
                        return func;
                    };
                    break;
                case EnumFilterCompare.Ends:
                    // todo
                    break;
                case EnumFilterCompare.Include:
                    // todo
                    break;
            }
            return expr;
        }

        // https://stackoverflow.com/questions/307512/how-do-i-apply-orderby-on-an-iqueryable-using-a-string-column-name-within-a-gene
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string ordering, bool asc, bool isFirst)
        {
            var type = typeof(T);
            var property = type.GetProperty(ordering);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);

            MethodCallExpression resultExp = isFirst ?
                Expression.Call(typeof(Queryable), asc ? "OrderBy" : "OrderByDescending", new Type[] { type, property.PropertyType }, source.Expression, Expression.Quote(orderByExp)) :
                Expression.Call(typeof(Queryable), asc ? "ThenBy" : "ThenByDescending", new Type[] { type, property.PropertyType }, source.Expression, Expression.Quote(orderByExp));
            return source.Provider.CreateQuery<T>(resultExp);
        }

        /*public static Expression<Func<T, object>> MakeMemberAccess<T>(string propertyName)
        {
            var param = Expression.Parameter(typeof(T), "x");
            var key = typeof(T).GetProperty(propertyName);
            var body = Expression.MakeMemberAccess(param, key);
            return Expression.Lambda<Func<T, object>>(body, param);
        }*/

        /*public static T Convert<T>(this object source)
        {
            Expression convertExpr = Expression.Convert(
                            Expression.Constant(source),
                            typeof(T)
                        );

            return Expression.Lambda<Func<T>>(convertExpr).Compile()();
        }*/

        /*public static Expression<Func<T>> ggg<T>(Type t)
        {
            return Expression.c
        }*/
    }
}
