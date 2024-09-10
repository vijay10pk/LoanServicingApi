using Microsoft.EntityFrameworkCore;
using System;
using LoanServicingApi.Data;
using LoanServicingApi.Models;
using LoanServicingApi.Interfaces;
using LoanServicingApi.Repositories;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using LoanServicingApi.Helpers;
using LoanServicingApi.Middleware;
using LoanServicingApi.Filters;
using LoanServicingApi.Services;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = config["JwtSettings:Issuer"],
        ValidAudience = config["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtSettings:Key"]!)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true
    };
});
builder.Services.AddAuthorization();

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddTransient<Seed>();
//builder.Services.AddControllers(options => options.Filters.Add<ErrorHandlingFilterAttribute>()).AddJsonOptions(x =>
//x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles); //Here we have added the error handling filter and so that we can use in controller as attributes
builder.Services.AddControllers().AddJsonOptions(x =>
x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Add connection to MySOL database
builder.Services.AddEntityFrameworkMySql()
    .AddDbContext<LoanServicingContext>(options =>
                                         options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
                                         new MySqlServerVersion(new Version()))
                                         );
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<LoanServicingApiHelper>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
builder.Services.AddScoped<ILoanRepository, LoanRepository>();
builder.Services.AddScoped<ILoanOfficerManagementRepository, LoanOfficerManagementRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

SeedData(app);

void SeedData(IHost app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();
    using (var scope = scopedFactory.CreateScope())
    {
        var service = scope.ServiceProvider.GetService<Seed>();
        service.SeedDataContext();

    }
}

//app.UseMiddleware<ErrorHandlingMiddleware>();
LoanServicingApiHelper.Initialize(app.Services);
app.UseExceptionHandler("/error");
app.UseHttpsRedirection();

app.UseAuthentication(); // This orders matters so authentication should always comes first before authorization
app.UseAuthorization();

app.MapControllers();

app.Run();
