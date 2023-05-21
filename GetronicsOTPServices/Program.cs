using GetronicsOTPServices.CommonServices;
using GetronicsOTPServices.Configs;
using GetronicsOTPServices.Contexts;
using GetronicsOTPServices.Interfaces;
using GetronicsOTPServices.Repositories;
using GetronicsOTPServices.Validation;
using log4net.Config;
using log4net;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

#region Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Getronics OTP Services", Version = "v1" });
});

#endregion

#region Dependency Injection

builder.Services.AddScoped<IOTPRepository, OTPRepository>();
builder.Services.AddScoped<IEmailValidation, EmailValidation>();
builder.Services.AddScoped<IEmailService, EmailService>();

#endregion

#region Config App Setting

builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
var configuration = builder.Configuration;

var emailConfig = new EmailConfig();
configuration.GetSection("EmailConfig").Bind(emailConfig);
builder.Services.AddSingleton(emailConfig);

#endregion

#region SQL Connection

builder.Services.AddDbContext<DBContext>(x => x.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

#endregion

#region Log4Net

// Initialize log4net
var logRepository = LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
XmlConfigurator.Configure(logRepository, new System.IO.FileInfo("log4net.config"));

#endregion

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
