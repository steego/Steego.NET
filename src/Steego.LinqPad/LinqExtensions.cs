using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Steego.Conversions {
    public enum SortDirection {
        Ascending, Descending
    }

    public struct SortParameter {
        public string Column;
        public SortDirection Direction;
    }

    public static class LinqExtensions {

        private static SortParameter ParseOrderBy(string orderBy) {
            if(orderBy == null)
                throw new ArgumentNullException(nameof(orderBy));
            var orderByParts = orderBy.Split(' ');

            var orderByColumn = orderByParts[0];
            var orderByDirection = SortDirection.Ascending;
            if(orderByParts.Length > 1 && orderByParts[1].ToLower() == "desc") {
                orderByDirection = SortDirection.Descending;
            }
            return new SortParameter() { Column = orderByColumn, Direction = orderByDirection };
        }

        /// <summary>
        /// Used to perform typed and dynamic order by.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="orderBy">The full order by expression, e.g. AuthorName DESC</param>
        public static IQueryable<T> SortBy<T>(this IQueryable<T> table, string orderBy) {
            if(String.IsNullOrEmpty(orderBy))
                return table;

            // Parse order by            
            var orderByColumn = string.Empty;
            var sortParameter = ParseOrderBy(orderBy);

            return table.SortBy(sortParameter.Column, sortParameter.Direction);
        }

        public static IQueryable<T> SortBy<T>(this IQueryable<T> table, string orderByColumn, SortDirection orderByDirection) {

            var TypeInfo = new TypeInfo<T>();

            if(orderByColumn.IsBlank()) {
                return table;
            }
            // Get sort results
            var member = TypeInfo.Member(orderByColumn);
            if(member == null) {
                throw new Exception("Cannot sort column " + orderByColumn);
            }
            var propType = member.Type;

            if(propType == typeof(DateTime?)) {
                return GetDynamicSortResults<T, DateTime?>(table, orderByColumn, orderByDirection);
            } else if(propType == typeof(int?)) {
                return GetDynamicSortResults<T, int?>(table, orderByColumn, orderByDirection);
            }

            switch(Type.GetTypeCode(propType)) {
                case TypeCode.Boolean:
                    return GetDynamicSortResults<T, bool>(table, orderByColumn, orderByDirection);
                case TypeCode.Byte:
                    return GetDynamicSortResults<T, byte>(table, orderByColumn, orderByDirection);
                case TypeCode.Char:
                    return GetDynamicSortResults<T, char>(table, orderByColumn, orderByDirection);
                case TypeCode.DateTime:
                    return GetDynamicSortResults<T, DateTime>(table, orderByColumn, orderByDirection);
                case TypeCode.Decimal:
                    return GetDynamicSortResults<T, decimal>(table, orderByColumn, orderByDirection);
                case TypeCode.Double:
                    return GetDynamicSortResults<T, double>(table, orderByColumn, orderByDirection);
                case TypeCode.Int16:
                    return GetDynamicSortResults<T, Int16>(table, orderByColumn, orderByDirection);
                case TypeCode.Int32:
                    return GetDynamicSortResults<T, Int32>(table, orderByColumn, orderByDirection);
                case TypeCode.Int64:
                    return GetDynamicSortResults<T, Int64>(table, orderByColumn, orderByDirection);
                case TypeCode.Single:
                    return GetDynamicSortResults<T, float>(table, orderByColumn, orderByDirection);
                case TypeCode.String:
                    return GetDynamicSortResults<T, string>(table, orderByColumn, orderByDirection);
                default:
                    throw new Exception("Cannot sort column " + orderByColumn + " because of its type:" + member.Type.Name);
            }

        }

        /// <summary>
        /// Dynamically builds a Linq expression that sorts by a particular column
        /// </summary>
        /// <typeparam name="S">The return type of the Lambda</typeparam>
        /// <param name="orderByColumn">The name of the order by column</param>
        /// <param name="orderByDirection">The direction of the order by sort</param>
        private IQueryable<T> GetDynamicSortResults<T, S>(IQueryable<T> table, string orderByColumn, SortDirection orderByDirection) {

            ParameterExpression sortParameterExpression = Expression.Parameter(typeof(T), "e");
            var sortPropertyExpression = Expression.PropertyOrField(sortParameterExpression, orderByColumn);

            var sortExpression = Expression.Lambda<Func<T, S>>(sortPropertyExpression, sortParameterExpression);

            if(orderByDirection != SortDirection.Descending) {
                return table.OrderBy(sortExpression);
            } else {
                return table.OrderByDescending(sortExpression);
            }
        }

        /// <summary>
        /// Dynamically builds a Linq expression that sorts by a particular column
        /// </summary>
        /// <typeparam name="S">The return type of the Lambda</typeparam>
        /// <param name="orderByColumn">The name of the order by column</param>
        /// <param name="orderByDirection">The direction of the order by sort</param>
        private IQueryable<T> GetDynamicSortResults<T, S>(IQueryable<T> table, string orderByColumn, string orderByDirection) {
            return GetDynamicSortResults<T, S>(table, orderByColumn, ((orderByDirection.IfBlank("").ToLower() == "desc") ? SortDirection.Ascending : SortDirection.Descending));
        }


    }

}
