using ArchiLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
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

        public static IQueryable<TModel> Filter<TModel>(this IQueryable<TModel> query, Params p, Dictionary<string, string> arrayProperties)
        {
            //arrayProperties = arrayProperties ?? throw new ArgumentNullException(nameof(arrayProperties));
            BinaryExpression binaryExpression = null;
            ConstantExpression c = null;
            bool isRange = false;

            var parameter = Expression.Parameter(typeof(TModel), "x");
            UnaryExpression o = null;
            foreach (var item in arrayProperties)
            {
                if (!string.IsNullOrWhiteSpace(item.Key) && !string.IsNullOrWhiteSpace(item.Value))
                {
                    var key = item.Key;
                    var value = item.Value;
                    var property = Expression.Property(parameter, key);

                    if (item.Value.Contains("[") && item.Value.Contains("]")) 
                    {
                        isRange = true;
                        value = value.Replace("[", "");
                        value = value.Replace("]", "");
                    }

                    c = Expression.Constant(value);

                    if (property.Type == typeof(string))
                    {
                        o = Expression.Convert(property, typeof(string));
                    }
                    else if (property.Type == typeof(int) && !item.Value.Contains(",") && isRange == false)
                    {
                        int v = Convert.ToInt32(value);
                        c = Expression.Constant(v);
                        o = Expression.Convert(property, typeof(int));
                    }

                    BinaryExpression? lambda = null;

                    if (item.Value.Contains(","))
                    {
                        var index = value.IndexOf(",");
                        string[] rangeValues = value.Split(",");
                        string? before = null;
                        string? after = null;
                        var type = property.GetType();

                        if (index != -1 && property.Type == typeof(int))
                        {
                            o = Expression.Convert(property, typeof(int));
                            before = value.Substring(0, index);
                            after = value.Substring(index, value.Length - 1);
                        }

                        //inferieur ou égal
                        if (before != null && before == "" && isRange == true)
                        {
                            if (o.Type == typeof(int))
                            {
                                lambda = Expression.LessThanOrEqual(property, Expression.Constant(int.Parse(rangeValues[1])));
                            }
                            else if (o.Type == typeof(string))
                            {
                                lambda = Expression.Equal(property, Expression.Constant(rangeValues[1].ToString()));
                            }
                        }
                        //supérieur ou égal
                        else if (after != null && after == "," && o.Type != typeof(string) && isRange == true)
                        {
                            if (o.Type == typeof(int))
                            {
                                lambda = Expression.GreaterThanOrEqual(property, Expression.Constant(int.Parse(rangeValues[0])));
                            }
                            else if (o.Type == typeof(string))
                            {
                                lambda = Expression.Equal(property, Expression.Constant(rangeValues[0].ToString()));
                            }
                        }
                        else
                        {
                            if (isRange == true)
                            {
                                if (o.Type == typeof(int))
                                {

                                    lambda = Expression.And(Expression.GreaterThanOrEqual(o, Expression.Constant(int.Parse(rangeValues[0]))), Expression.LessThanOrEqual(o, Expression.Constant(int.Parse(rangeValues[1]))));
                                }
                                else if (o.Type == typeof(string))
                                {
                                    lambda = Expression.Or(Expression.Equal(o, Expression.Constant(rangeValues[0].ToString())), Expression.Equal(o, Expression.Constant(rangeValues[1].ToString())));
                                }
                            }
                            else
                            {
                                if (o.Type == typeof(int))
                                {
                                    lambda = Expression.Or(Expression.Equal(o, Expression.Constant(int.Parse(rangeValues[0]))), Expression.Equal(o, Expression.Constant(int.Parse(rangeValues[1]))));
                                }
                                else if (o.Type == typeof(string))
                                {
                                    lambda = Expression.Or(Expression.Equal(o, Expression.Constant(rangeValues[0].ToString())), Expression.Equal(o, Expression.Constant(rangeValues[1].ToString())));
                                }
                            }

                        }
                    }

                    else
                    {
                            lambda = Expression.Equal(o, c);
                    }

                    if (binaryExpression == null)
                    {
                        binaryExpression = lambda;
                    }
                    else
                        binaryExpression = Expression.And(binaryExpression, lambda);
                }
            }
            if (binaryExpression != null)
            {
                return query.Where(Expression.Lambda<Func<TModel, bool>>(binaryExpression, parameter));
            }
            else
                return (IQueryable<TModel>)query;
        }
    }
}
