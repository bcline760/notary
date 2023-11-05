using Autofac;
using Autofac.Extensions.DependencyInjection;
using log4net;

using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;

using Notary.Configuration;
using Notary.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddControllers();
builder.Services.AddHealthChecks();

builder.Services.AddCors(o =>
{
    o.AddPolicy("DebugAllow", b => b.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
    o.AddPolicy("ProductionCors", b => b.WithMethods("GET", "POST", "PUT", "DELETE"));
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
var config = builder.Configuration
    .GetSection("Notary")
    .Get<NotaryConfiguration>();

if (config == null)
    throw new InvalidOperationException("Configuration not found. Please ensure a configuration file is present.");
builder.Host.ConfigureContainer<ContainerBuilder>(c =>
{
    c.RegisterInstance(config).SingleInstance();

    c.Register(r => LogManager.GetLogger(typeof(Program))).As<ILog>().SingleInstance();
    RegisterModules.Register(c);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors("DebugAllow");
    app.UseSwagger();
    app.UseSwaggerUI();
}
else if (app.Environment.IsProduction())
{
    app.UseCors("ProductionCors");
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();