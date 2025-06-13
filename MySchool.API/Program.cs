using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySchool.API;
using MySchool.API.DataSeed;
using MySchool.API.Extensions;
using MySchool.API.Hubs;
using MySchool.API.Middlewares;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        builder.Services.AddDbContext<DataBaseContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddSwaggerSupport(builder.Configuration);
        builder.Services.AddApiConfigurationExtensions();
        builder.Services.AddJwtConfiguration(builder);
        builder.Services.AddAutoServices();
        builder.Services.AddMemoryCache();
        builder.Services.AddSignalR(options =>
        {

            options.EnableDetailedErrors = true;

        });


        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = CustomInvalidModelResponse.Generate;
        });

        builder.Services.AddAuthorizationBuilder()
           .AddPolicy(Policies.Admin, policy => policy.RequireRole(Policies.PolicyRolesMap[Policies.Admin]))
           .AddPolicy(Policies.Teacher, policy => policy.RequireRole(Policies.PolicyRolesMap[Policies.Teacher]))
           .AddPolicy(Policies.Student, policy => policy.RequireRole(Policies.PolicyRolesMap[Policies.Student]))
           .AddPolicy(Policies.Guardian, policy => policy.RequireRole(Policies.PolicyRolesMap[Policies.Guardian]))
           .AddPolicy(Policies.AllUsers, policy => policy.RequireRole(Policies.PolicyRolesMap[Policies.AllUsers]))
           .AddPolicy(Policies.Moderator, policy => policy.RequireRole(Policies.PolicyRolesMap[Policies.Moderator]));

        var app = builder.Build();



        app.UseMiddleware<ResponseTimeMiddleware>();
        app.UseMiddleware<ErrorHandlingMiddleware>();

        if (builder.Configuration.GetValue<bool>("FeatureFlags:MigrateAtStartup"))
        {
            using (var scope = app.Services.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<DataBaseContext>();
                    if (dbContext.Database.GetPendingMigrations().Any())
                    {
                        dbContext.Database.Migrate();
                    }

                    if (builder.Configuration.GetValue<bool>("FeatureFlags:SeedAtStartup"))
                    {
                        await DataSeeder.Instance.ResetDatabaseWithGeneratedDataAsync(dbContext);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error occurred while migrating or seeding the database.");
                }
            }
        }


        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
            c.ConfigObject.AdditionalItems.Add("deepLinking", "true");
        });

        app.UseRouting();
        app.UseErrorHandler();

        app.UseCors(c => c.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowed(origin => true));


        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseAuthentication();
        app.UseMiddleware<ForcePasswordChangeMiddleware>();
        app.UseAuthorization();

        app.MapControllers();
        app.UseWebSockets();
        app.MapChatHub();

        app.Run();
    }
}