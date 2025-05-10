
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MySchool.API.Extensions
{
    public static partial class ApiExtensions
    {

        public static void AddApiConfigurationExtensions(this IServiceCollection services)
        {
            services.Configure<JsonOptions>(options =>
             {

                 options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
                 options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());





                 //options.JsonSerializerOptions.Converters.Add(new TimeOnlyConverter());
             });

            services.AddControllers(options =>
            {
                options.Conventions.Add(new RouteTokenTransformerConvention(new SnakeCaseParameterTransformer()));

            });

            services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
                options.ConstraintMap["snakecase"] = typeof(SnakeCaseParameterTransformer);

            });

        }

        public class SnakeCaseParameterTransformer : IOutboundParameterTransformer
        {
            public string? TransformOutbound(object? value)
            {
                if (value == null)
                    return null;

                return JsonNamingPolicy.SnakeCaseLower.ConvertName(value.ToString()!);
            }
        }

        //public class TimeOnlyConverter : JsonConverter<TimeOnly>
        //{
        //    private const string Format = "HH:mm:ss";

        //    public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        //    {
        //        return TimeOnly.ParseExact(reader.GetString()!, Format);
        //    }

        //    public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
        //    {
        //        writer.WriteStringValue(value.ToString(Format));
        //    }
        //}
    }
}
