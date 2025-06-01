using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json.Serialization;

namespace MySchool.API.Common
{
    public class PaginateBlock<T>
    {
        [JsonIgnore]
        public bool IsList { get; set; } = false;
        public int TotalRecords { get; set; }
        public int RemainingRecords { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public IEnumerable<T> Data { get; set; } = default!;
    }

    public class PaginationFilter<TResponse>
    {
        /// <summary>
        /// Search query filter.
        /// </summary>
        public string? Query { get; set; }

        /// <summary>
        /// Page number (default is 1, min 1).
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Number of items per page (default is 10, min 0, max 500).
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Filtering criteria as key-value pairs.
        /// </summary>
        [DefaultValue(null)]
        public Dictionary<string, string>? Where { get; set; }

        /// <summary>
        /// Sorting criteria as key-value pairs.
        /// </summary>
        [DefaultValue(null)]
        public Dictionary<string, string>? OrderBy { get; set; }

        /// <summary>
        /// To retrun list of items
        /// </summary>        
        public bool IsList { get; set; } = false;

        public PaginateBlock<TResponse> Apply(IQueryable<TResponse> query)
        {
            if (!string.IsNullOrWhiteSpace(Query))
            {
                query = ApplySearchQuery(query);
            }

            if (Where != null && Where.Any())
            {
                query = ApplyFilters(query, Where);
            }

            if (OrderBy != null && OrderBy.Any())
            {
                query = ApplySorting(query, OrderBy);
            }

            int totalRecords = query.Count();

            PageSize = Math.Clamp(PageSize, 0, 500);
            Page = Math.Clamp(Page, 1, int.MaxValue);
            query = query.Skip((Page - 1) * PageSize).Take(PageSize);

            return new PaginateBlock<TResponse>
            {
                TotalRecords = totalRecords,
                RemainingRecords = Math.Max(0, totalRecords - (Page * PageSize)),
                Page = Page,
                PageSize = PageSize,
                Data = query.ToList(),
                IsList = IsList
            };
        }

        private IQueryable<TResponse> ApplySearchQuery(IQueryable<TResponse> query)
        {
            if (string.IsNullOrWhiteSpace(Query))
                return query;

            var propertyPaths = GetStringPropertyPaths(typeof(TResponse));
            if (!propertyPaths.Any())
                return query.Where(x => false);

            var parameter = Expression.Parameter(typeof(TResponse), "x");
            Expression? searchExpression = null;

            foreach (var path in propertyPaths)
            {
                var propertyAccess = BuildPropertyPathExpression(parameter, path);
                if (propertyAccess == null || propertyAccess.Type != typeof(string))
                    continue;

                var searchValue = Expression.Constant(Query);
                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

                if (containsMethod != null)
                {
                    var containsCall = Expression.Call(propertyAccess, containsMethod, searchValue);
                    searchExpression = searchExpression == null
                        ? containsCall
                        : Expression.OrElse(searchExpression, containsCall);
                }
            }

            if (searchExpression == null)
                return query.Where(x => false);

            var lambda = Expression.Lambda<Func<TResponse, bool>>(searchExpression, parameter);
            return query.Where(lambda);
        }

        private IQueryable<TResponse> ApplyFilters<TResponse>(IQueryable<TResponse> query, Dictionary<string, string> filters)
        {
            var parameter = Expression.Parameter(typeof(TResponse), "x");
            Expression? filterExpression = null;

            foreach (var filter in filters)
            {
                var propertyAccess = BuildPropertyPathExpression(parameter, filter.Key);
                if (propertyAccess == null)
                    throw new ValidationException($"Property path '{filter.Key}' not found");

                try
                {
                    var condition = BuildCondition(propertyAccess, filter.Value);
                    filterExpression = filterExpression == null
                        ? condition
                        : Expression.AndAlso(filterExpression, condition);
                }
                catch (Exception ex)
                {
                    throw new ValidationException($"Failed to apply filter for property '{filter.Key}'", ex);
                }
            }

            if (filterExpression != null)
            {
                var lambda = Expression.Lambda<Func<TResponse, bool>>(filterExpression, parameter);
                return query.Where(lambda);
            }

            return query.Where(x => false);
        }

        private Expression BuildCondition(Expression propertyAccess, string value)
        {
            var propertyType = propertyAccess.Type;
            var underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

            if (underlyingType == typeof(DateOnly))
                return BuildDateOnlyCondition(propertyAccess, value);

            if (underlyingType.IsEnum)
            {
                var enumValue = Enum.Parse(underlyingType, value);
                var constant = Expression.Constant(enumValue, propertyType);
                return Expression.Equal(propertyAccess, constant);
            }

            var convertedValue = Convert.ChangeType(value, underlyingType);
            var valueConst = Expression.Constant(convertedValue, propertyType);
            return Expression.Equal(propertyAccess, valueConst);
        }

        private Expression BuildDateOnlyCondition(Expression propertyAccess, string value)
        {
            Expression BuildDateRangeExpression(Expression propertyAccess, DateOnly start, DateOnly end)
            {
                var startConst = Expression.Constant(start, typeof(DateOnly));
                var endConst = Expression.Constant(end, typeof(DateOnly));

                return Expression.AndAlso(
                    Expression.GreaterThanOrEqual(propertyAccess, startConst),
                    Expression.LessThan(propertyAccess, endConst)
                );
            }



            var parts = value.Split('-');

            if (parts.Length == 1 && int.TryParse(parts[0], out int year))
            {
                var start = new DateOnly(year, 1, 1);
                var end = start.AddYears(1);
                return BuildDateRangeExpression(propertyAccess, start, end);
            }

            if (parts.Length == 2 &&
                int.TryParse(parts[0], out int year2) &&
                int.TryParse(parts[1], out int month))
            {
                var start = new DateOnly(year2, month, 1);
                var end = start.AddMonths(1);
                return BuildDateRangeExpression(propertyAccess, start, end);
            }

            var exact = DateOnly.Parse(value);
            var exactConst = Expression.Constant(exact, typeof(DateOnly));
            return Expression.Equal(propertyAccess, exactConst);
        }


        private IQueryable<TResponse> ApplySorting(IQueryable<TResponse> query, Dictionary<string, string> orderBy)
        {
            bool firstSort = true;
            var parameter = Expression.Parameter(typeof(TResponse), "x");

            foreach (var sort in orderBy)
            {

                var propertyAccess = BuildPropertyPathExpression(parameter, sort.Key);
                if (propertyAccess == null)
                    throw new ArgumentException($"Property path '{sort.Key}' not found");

                try
                {
                    var lambda = Expression.Lambda(propertyAccess, parameter);

                    string methodName;
                    if (firstSort)
                    {
                        methodName = sort.Value.StartsWith("desc", StringComparison.OrdinalIgnoreCase)
                            ? "OrderByDescending"
                            : "OrderBy";
                        firstSort = false;
                    }
                    else
                    {
                        methodName = sort.Value.StartsWith("desc", StringComparison.OrdinalIgnoreCase)
                            ? "ThenByDescending"
                            : "ThenBy";
                    }

                    var method = typeof(Queryable).GetMethods()
                        .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                        .MakeGenericMethod(typeof(TResponse), propertyAccess.Type);

                    query = (IQueryable<TResponse>)method.Invoke(null, new object[] { query, lambda })!;
                }
                catch (Exception ex)
                {
                    throw new ValidationException($"Failed to apply sorting for property '{sort.Key}'", ex);
                }
            }

            return query;
        }

        private Expression? BuildPropertyPathExpression(ParameterExpression parameter, string propertyPath)
        {
            propertyPath = propertyPath.Replace("_", "");
            string[] parts = propertyPath.Split('.');

            if (parts.Length == 0)
                return null;

            Type currentType = parameter.Type;
            Expression? expression = parameter;

            foreach (var part in parts)
            {
                PropertyInfo? property = currentType.GetProperty(part,
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (property == null)
                    return null;

                expression = Expression.Property(expression, property);
                currentType = property.PropertyType;
            }

            return expression;
        }

        private List<string> GetStringPropertyPaths(Type type, string prefix = "")
        {
            var paths = new List<string>();

            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var propPath = string.IsNullOrEmpty(prefix) ? prop.Name : $"{prefix}.{prop.Name}";

                if (prop.PropertyType == typeof(string))
                {
                    paths.Add(propPath);
                }
                else if (!prop.PropertyType.IsPrimitive &&
                         prop.PropertyType != typeof(DateTime) &&
                         !prop.PropertyType.IsEnum &&
                         !prop.PropertyType.IsGenericType &&
                         !prop.PropertyType.IsArray &&
                         prop.PropertyType.Assembly == type.Assembly)
                {
                    paths.AddRange(GetStringPropertyPaths(prop.PropertyType, propPath));
                }
            }

            return paths;
        }


    }

}
