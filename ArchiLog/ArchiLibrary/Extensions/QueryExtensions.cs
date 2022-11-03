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
                string champ = "";

                if (!string.IsNullOrWhiteSpace(p.Asc))
                {
                    champ = p.Asc;
                } else if (!string.IsNullOrWhiteSpace(p.Desc))
                {
                    champ = p.Desc;
                }

                var parameter = Expression.Parameter(typeof(TModel), "x");
                var property = Expression.Property(parameter, champ/*"Name"*/);

                //créer lambda
                var o = Expression.Convert(property, typeof(object));
                var lambda = Expression.Lambda<Func<TModel, object>>(o, parameter);

                //utilisation lambda
                if (!string.IsNullOrWhiteSpace(p.Asc)) return query.OrderBy(lambda);
                else if (!string.IsNullOrWhiteSpace(p.Desc)) return query.OrderByDescending(lambda);
                else return (IOrderedQueryable<TModel>)query;
                //return query.OrderBy(x => x.Name);
            }
            else return (IOrderedQueryable<TModel>)query;
            
        }
    }
}
