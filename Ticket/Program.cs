using Common;
using Data.Contracts;
using Data.Repositories;
using ElmahCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Web;
using Services;
using Services.Services;
using WebFramework.Configuration;
using WebFramework.CustomMapping;
using WebFramework.Middlewares;
using WebFramework.Swagger;


var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    var siteSettings = builder.Configuration.GetSection(nameof(SiteSettings)).Get<SiteSettings>();

    builder.Services.Configure<SiteSettings>(builder.Configuration.GetSection(nameof(SiteSettings)));
    builder.Services.AddControllers(options => { options.Filters.Add(new AuthorizeFilter()); });



    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwagger(siteSettings.Url);

    builder.Services.AddDbContext(builder.Configuration);
    builder.Services.AddCustomIdentity(siteSettings.IdentitySettings);

    builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IJwtService, JwtService>();
    //builder.Services.AddScoped<IDataInitializer, UserDataInitializer>();

    builder.Services.AddJwtAuthentication(siteSettings.JwtSettings);

    builder.Services.InitializeAutoMapper();
    builder.Services.AddCustomApiVersioning();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception exception)
{
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    LogManager.Shutdown();
}

