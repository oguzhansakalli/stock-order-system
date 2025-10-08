using Inventory.Infrastructure;
using SharedKernel.Application.Abstractions;
using SharedKernel.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FluentValidation;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Stock Order System API",
        Version = "v1",
        Description = "Clean Architecture API with Modular Monolith"
    });
});

// Add HttpContextAccessor for TenantProvider
builder.Services.AddHttpContextAccessor();

// Register TenantProvider
builder.Services.AddScoped<ITenantProvider, TenantProvider>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    // Add Inventory.Application assembly
    cfg.RegisterServicesFromAssemblyContaining<Inventory.Application.Products.Commands.CreateProduct.CreateProductCommand>();
});

// Add FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<Inventory.Application.Products.Commands.CreateProduct.CreateProductCommandValidator>();

// Add Infrastructure modules
builder.Services.AddInventoryInfrastructure(builder.Configuration);

// Add JWT Authentication (will be configured later with Users module)
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "your-very-long-secret-key-min-32-chars-for-development-only";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"] ?? "StockOrderSystem",
        ValidAudience = jwtSettings["Audience"] ?? "StockOrderSystemUsers",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Stock Order System API v1");
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();