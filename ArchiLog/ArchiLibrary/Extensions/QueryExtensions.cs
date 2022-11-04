using ArchiLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ArchiLibrary.Extensions
{
    public static class QueryExtensions
    {

        public static IOrderedQueryable<TModel> Sort<TModel>(this IQueryable<TModel> query, Params p)
        {
            if (!string.IsNullOrWhiteSpace(p.Asc) || !string.IsNullOrWhiteSpace(p.Desc))
            {
                string? champ = string.IsNullOrEmpty(p.Asc) ? p.Desc : p.Asc;

                var parameter = Expression.Parameter(typeof(TModel), "x");
                var property = Expression.Property(parameter, champ);

                var o = Expression.Convert(property, typeof(object));
                var lambda = Expression.Lambda<Func<TModel, object>>(o, parameter);

                if (!string.IsNullOrWhiteSpace(p.Asc)) return query.OrderBy(lambda);
                else return query.OrderByDescending(lambda);
            }
            else return (IOrderedQueryable<TModel>)query;
        }

        public static IOrderedQueryable<TModel> Pagination<TModel>(this IQueryable<TModel> query, int startIndex, int endIndex)
        {
            return (IOrderedQueryable<TModel>)query.Skip(startIndex <= 0 ? 0 : startIndex - 1).Take(endIndex - startIndex + 1);
        }
    }
}
