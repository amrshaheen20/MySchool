using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using MySchool.API.Common;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MySchool.API.Extensions
{
    public static partial class ApiExtensions
    {
        public class AuthResponsesOperationFilter : IOperationFilter
        {
            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                var declaringType = context.MethodInfo.DeclaringType;
                var methodAttributes = context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>();

                IEnumerable<AuthorizeAttribute> classAttributes = declaringType != null
                    ? declaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>()
                    : Enumerable.Empty<AuthorizeAttribute>();

                var authAttributes = methodAttributes.Union(classAttributes);

                if (authAttributes.Any())
                {
                    //operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });

                    var securityScheme = new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    };

                    operation.Security = new List<OpenApiSecurityRequirement>
                    {
                        new OpenApiSecurityRequirement
                        {
                            [securityScheme] = new List<string>()
                        }
                    };
                }


                //if (context.MethodInfo.GetParameters().Length > 0)
                //{
                //    operation.Responses.TryAdd("400", new OpenApiResponse
                //    {
                //        Description = "Bad Request",
                //        Content = new Dictionary<string, OpenApiMediaType>
                //        {
                //            ["application/json"] = new OpenApiMediaType
                //            {
                //                Schema = context.SchemaGenerator.GenerateSchema(typeof(BaseResponse), context.SchemaRepository)
                //            }
                //        }
                //    });
                //}


                operation.Responses.TryAdd("400>=", new OpenApiResponse
                {
                    Description = "Client error response",
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new OpenApiMediaType
                        {
                            Schema = context.SchemaGenerator.GenerateSchema(typeof(BaseResponse), context.SchemaRepository)
                        }
                    }
                });

            }

        }



        public class SchemaFilter : ISchemaFilter
        {
            public void Apply(OpenApiSchema schema, SchemaFilterContext context)
            {

                if (context.Type == typeof(TimeOnly))
                {
                    schema.Type = "string";
                    schema.Format = "time";
                    schema.Example = new OpenApiString("14:30:00");
                }


                if (context.Type == typeof(TimeOnly?))
                {
                    schema.Type = "string";
                    schema.Format = "time";
                    schema.Example = new OpenApiString("14:30:00");
                }



                var rangeAttribute = context.MemberInfo?
                    .GetCustomAttributes(typeof(RangeAttribute), false)
                    .Cast<RangeAttribute>()
                    .FirstOrDefault();

                if (rangeAttribute != null)
                {
                    if (rangeAttribute.Minimum is IConvertible min)
                    {
                        schema.Minimum = Convert.ToDecimal(min);
                    }

                    if (rangeAttribute.Maximum is IConvertible max)
                    {
                        schema.Maximum = Convert.ToDecimal(max);
                    }
                }
            }
        }


        public class RolesOperationFilter : IOperationFilter
        {
            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                var authAttributes = context.MethodInfo
                    .GetCustomAttributes(true)
                    .OfType<AuthorizeAttribute>()
                    .Concat(context.MethodInfo.DeclaringType?.GetCustomAttributes(true).OfType<AuthorizeAttribute>() ?? []);

                foreach (var attribute in authAttributes)
                {
                    if (!string.IsNullOrEmpty(attribute.Roles))
                    {
                        AppendToDescription(operation, $"**Allowed Roles:** {attribute.Roles}");
                    }
                    else if (!string.IsNullOrEmpty(attribute.Policy) &&
                             Policies.PolicyRolesMap.TryGetValue(attribute.Policy, out var roles))
                    {
                        var roleList = string.Join(", ", roles);
                        AppendToDescription(operation, $"**Allowed Roles:** {roleList}");
                        //AppendToDescription(operation, $"**Required Policy:** {attribute.Policy}\n\n**Allowed Roles:** {roleList}");
                    }
                }
            }

            private void AppendToDescription(OpenApiOperation operation, string text)
            {
                operation.Description ??= string.Empty;
                operation.Description += $"\n\n{text}";
            }
        }

        public static void AddSwaggerSupport(this IServiceCollection services, IConfiguration configuration)
        {
            var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;


            var swaggerSettings = configuration.GetSection("Swagger");

            var version = swaggerSettings.GetValue<string>("Version") ?? "v1";
            var title = swaggerSettings.GetValue<string>("Title") ?? assemblyName;
            var description = swaggerSettings.GetValue<string>("Description") ?? "";



            services.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = version,
                    Title = title,
                    Description = description,
                });

                var xmlFile = $"{assemblyName}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                swagger.IncludeXmlComments(xmlPath);


                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your valid token in the text input below.\r\n\r\nExample: \"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
                });



                swagger.AddSignalRSwaggerGen();


                swagger.TagActionsBy(api =>
                {
                    if (api.GroupName != null)
                    {
                        return new[] { api.GroupName };
                    }

                    if (api.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
                    {
                        return new[] { controllerActionDescriptor.ControllerName };
                    }

                    throw new InvalidOperationException("Unable to determine tag for endpoint.");
                });


                swagger.DocInclusionPredicate((name, api) => true);
                swagger.OperationFilter<AuthResponsesOperationFilter>();
                swagger.SchemaFilter<SchemaFilter>();
                swagger.OperationFilter<RolesOperationFilter>();
            });
        }
    }
}
