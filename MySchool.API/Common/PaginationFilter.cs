using FluentValidation;
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
        [DefaultValue(1)]
        [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
        public int Page { get; set; } = 1;

        /// <summary>
        /// Number of items per page (default is 10, min 0, max 500).
        /// </summary>
        /// <example>10</example>
        [DefaultValue(10)]
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Filtering criteria as key-value pairs.
        /// </summary>
        public Dictionary<string, string>? Where { get; set; }

        /// <summary>
        /// Sorting criteria as key-value pairs.
        /// </summary>
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

            PageSize = Math.Clamp(PageSize, 10, 500);
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
            var properties = typeof(TResponse).GetProperties()
                .Where(p => p.PropertyType == typeof(string))
                .ToList();

            if (!properties.Any())
                return query;

            var parameter = Expression.Parameter(typeof(TResponse), "x");
            Expression? searchExpression = null;

            foreach (var property in properties)
            {
                var propertyAccess = Expression.Property(parameter, property);
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
                return query;

            var lambda = Expression.Lambda<Func<TResponse, bool>>(searchExpression, parameter);
            return query.Where(lambda);
        }

        private IQueryable<TResponse> ApplyFilters(IQueryable<TResponse> query, Dictionary<string, string> filters)
        {
            var parameter = Expression.Parameter(typeof(TResponse), "x");
            Expression? filterExpression = null;

            foreach (var filter in filters)
            {
                var property = GetProperty(typeof(TResponse), filter.Key);

                if (property == null)
                    continue;

                try
                {
                    var propertyAccess = Expression.Property(parameter, property);
                    object? convertedValue;
                    var underlyingType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

                    if (underlyingType == typeof(DateOnly))
                    {
                        convertedValue = DateOnly.Parse(filter.Value);
                    }
                    else if (underlyingType.IsEnum)
                    {
                        convertedValue = Enum.Parse(underlyingType, filter.Value);
                    }
                    else
                    {
                        convertedValue = Convert.ChangeType(filter.Value, underlyingType);
                    }
                    var filterValue = Expression.Constant(convertedValue, property.PropertyType);
                    var equalsExpression = Expression.Equal(propertyAccess, filterValue);

                    filterExpression = filterExpression == null
                        ? equalsExpression
                        : Expression.AndAlso(filterExpression, equalsExpression);
                }
                catch
                {
                    continue;
                }
            }

            if (filterExpression == null)
                return query;

            var lambda = Expression.Lambda<Func<TResponse, bool>>(filterExpression, parameter);
            return query.Where(lambda);
        }

        private IQueryable<TResponse> ApplySorting(IQueryable<TResponse> query, Dictionary<string, string> orderBy)
        {
            bool firstSort = true;
            var parameter = Expression.Parameter(typeof(TResponse), "x");
            foreach (var sort in orderBy)
            {
                var property = GetProperty(typeof(TResponse), sort.Key);

                if (property == null)
                    continue;

                var propertyAccess = Expression.Property(parameter, property);
                var lambda = Expression.Lambda(propertyAccess, parameter);

                string methodName;
                if (firstSort)
                {
                    methodName = sort.Value.StartsWith("desc", StringComparison.OrdinalIgnoreCase) ? "OrderByDescending" : "OrderBy";
                    firstSort = false;
                }
                else
                {
                    methodName = sort.Value.StartsWith("desc", StringComparison.OrdinalIgnoreCase) ? "ThenByDescending" : "ThenBy";
                }

                var method = typeof(Queryable).GetMethods()
                    .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(TResponse), property.PropertyType);

                query = (IQueryable<TResponse>)method.Invoke(null, new object[] { query, lambda })!;
            }

            return query;
        }

        private PropertyInfo? GetProperty(Type type, string propertyPath)
        {
            PropertyInfo? property = null;
            foreach (var part in propertyPath.Replace("_", "").Split('.'))
            {
                property = type.GetProperty(part, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (property == null)
                    return null;
                type = property.PropertyType;
            }
            return property;
        }

    }

}
