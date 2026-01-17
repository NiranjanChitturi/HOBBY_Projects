using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MPHMS.Application.Common.Interfaces;
using MPHMS.Infrastructure.DependencyInjection;
using MPHMS.Api.Services;
using System.Text;


/// <summary>
/// Entry point of MPHMS API application.
///
/// This file configures:
/// ---------------------
/// ✔ Dependency Injection
/// ✔ Infrastructure Layer
/// ✔ Authentication (JWT)
/// ✔ Authorization
/// ✔ HTTP Middleware Pipeline
///
/// Architecture Flow:
/// ------------------
/// Client Request
///     ↓
/// Middleware Pipeline
///     ↓
/// Authentication
///     ↓
/// Authorization
///     ↓
/// Controllers (API Layer)
///     ↓
/// Application Layer
///     ↓
/// Domain Layer
///     ↓
/// Infrastructure Layer
///
/// IMPORTANT:
/// ----------
/// This file should remain LIGHT and ONLY configure application wiring.
/// No business logic should ever be added here.
/// </summary>


var builder = WebApplication.CreateBuilder(args);


// =====================================================
// SERVICE REGISTRATION (Dependency Injection Container)
// =====================================================

// ------------------------------------
// Register Infrastructure Layer
// ------------------------------------
builder.Services.AddInfrastructure(builder.Configuration);

// ------------------------------------
// HttpContext Access (Required for CurrentUserService)
// ------------------------------------
builder.Services.AddHttpContextAccessor();

// ------------------------------------
// Current User Resolver (JWT → UserId)
// ------------------------------------
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// ------------------------------------
// Controllers
// ------------------------------------
builder.Services.AddControllers();

// ------------------------------------
// Swagger / OpenAPI
// ------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



// =====================================================
// JWT AUTHENTICATION CONFIGURATION
// =====================================================

//var jwtSettings = builder.Configuration.GetSection("JwtSettings");

//var secretKey = jwtSettings["SecretKey"];
//var issuer = jwtSettings["Issuer"];
//var audience = jwtSettings["Audience"];

//var key = Encoding.UTF8.GetBytes(secretKey);
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

var secretKey = jwtSettings.GetValue<string>("SecretKey")!;
var issuer = jwtSettings.GetValue<string>("Issuer")!;
var audience = jwtSettings.GetValue<string>("Audience")!;

var key = Encoding.UTF8.GetBytes(secretKey);


// ------------------------------------
// Add Authentication Middleware
// ------------------------------------
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Allow HTTP for dev
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = issuer,
        ValidAudience = audience,

        IssuerSigningKey = new SymmetricSecurityKey(key),

        ClockSkew = TimeSpan.Zero
    };
});



// =====================================================
// APP PIPELINE
// =====================================================

var app = builder.Build();



// ------------------------------------
// Swagger UI
// ------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



// ------------------------------------
// Security Middlewares
// ------------------------------------
app.UseHttpsRedirection();

app.UseAuthentication(); // MUST be before Authorization
app.UseAuthorization();



// ------------------------------------
// Map Controllers
// ------------------------------------
app.MapControllers();



// ------------------------------------
// Run Application
// ------------------------------------
app.Run();