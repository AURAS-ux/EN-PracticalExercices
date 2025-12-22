using AMT.Application.Services;
using AMT.Application.Services.Interfaces;
using AMT.Application.Validators;
using AMT.Infrastructure.Data;
using AMT.Infrastructure.Interfaces;
using AMT.Infrastructure.Interfaces.Repos;
using AMT.Infrastructure.Middlewares;
using AMT.Infrastructure.Repository;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: Serilog.RollingInterval.Day));

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AirportManagementContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("AirportManagementDb"));
});

builder.Services.AddIdentityCore<IdentityUser>(options =>
{
    options.User.RequireUniqueEmail = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AirportManagementContext>()
.AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    var jwtConfigs = builder.Configuration.GetSection("Jwt");
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtConfigs["Issuer"],
        ValidAudience = jwtConfigs["Audience"],
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtConfigs["Key"]!))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AirportAdmin", policy => policy.RequireRole("AirportAdmin"));
});

builder.Services.AddScoped<IAirlineRepository, AirlineRepository>();
builder.Services.AddScoped<IAirportRepository, AirportRepository>();
builder.Services.AddScoped<IAircraftRepository, AircraftRepository>();
builder.Services.AddScoped<IFlightRepository, FlightRepository>();
builder.Services.AddScoped<IFlightScheduleRepository, FlightScheduleRepositroy>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IGateRepository, GateRepository>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddValidatorsFromAssemblyContaining<FlightValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ScheduleValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<TicketValidator>();

builder.Services.AddScoped<IFlightService, FlightService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IAirportService, AirportService>();
builder.Services.AddScoped<IGateService, GateService>();
builder.Services.AddScoped<IAirlineService, AirlineService>();
builder.Services.AddScoped<IAircraftService, AircraftService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();
app.Run();