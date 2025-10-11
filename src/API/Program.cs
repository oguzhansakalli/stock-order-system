using Inventory.Infrastructure;
using Users.Infrastructure;
using SharedKernel.Application.Abstractions;
using SharedKernel.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FluentValidation;
using MediatR;
using Orders.Infrastructure;

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
    // Add JWT authentication to Swagger
    options.AddSecurityDefinition("Bearer", new()
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token."
    });

    options.AddSecurityRequirement(new()
    {
        {
            new()
            {
                Reference = new()
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
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
    // Add Orders.Application assembly
    cfg.RegisterServicesFromAssemblyContaining<Orders.Application.Commands.CreateOrder.CreateOrderCommand>();
    // Add Users.Application assembly
    cfg.RegisterServicesFromAssemblyContaining<Users.Application.Commands.Register.RegisterCommand>();
});

// Add FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<Inventory.Application.Products.Commands.CreateProduct.CreateProductCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<Orders.Application.Commands.CreateOrder.CreateOrderCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<Users.Application.Commands.Register.RegisterCommandValidator>();

// Add Infrastructure modules
builder.Services.AddInventoryInfrastructure(builder.Configuration);
builder.Services.AddOrdersInfrastructure(builder.Configuration);
builder.Services.AddUsersInfrastructure(builder.Configuration);

// Add JWT Authentication
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
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"] ?? "StockOrderSystem",
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"] ?? "StockOrderSystemUsers",
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
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