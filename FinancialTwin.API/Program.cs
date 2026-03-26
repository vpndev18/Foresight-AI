using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MediatR;
using FinancialTwin.API.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",  // Vite dev server
                "http://localhost:5174",  // Vite dev server (alternate)
                "http://localhost:3000"   // Docker frontend (Nginx)
              )
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Register the Database context with the connection string from appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Register Financial Math Engine Service
builder.Services.AddScoped<FinancialTwin.API.Services.IFinancialMathEngine, FinancialTwin.API.Services.FinancialMathEngine>();

// Register AI Service
builder.Services.AddScoped<FinancialTwin.API.Services.IAiService, FinancialTwin.API.Services.MockAiService>();

// builder.Services.AddScoped<FinancialTwin.API.Services.IAiService, FinancialTwin.API.Services.GeminiAiService>();
// builder.Services.AddHttpClient<FinancialTwin.API.Services.GeminiAiService>();

// ──────────────────────────────────────────────
// 1. Register TokenService in Dependency Injection
// ──────────────────────────────────────────────
builder.Services.AddScoped<FinancialTwin.API.Services.ITokenService, FinancialTwin.API.Services.TokenService>();

// ──────────────────────────────────────────────
// 2. Configure JWT Bearer Authentication
// ──────────────────────────────────────────────
var jwtKey = builder.Configuration["Jwt:Key"]!;
var jwtIssuer = builder.Configuration["Jwt:Issuer"]!;
var jwtAudience = builder.Configuration["Jwt:Audience"]!;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();

// ──────────────────────────────────────────────
// 3. Authentication & Authorization Middleware
//    (must be BEFORE endpoint mapping)
// ──────────────────────────────────────────────
app.UseAuthentication();
app.UseAuthorization();


// ═══════════════════════════════════════════════
// PUBLIC Endpoints (no token required)
// ═══════════════════════════════════════════════

app.MapGet("/ping", () => "Pong! Financial Twin is alive.");

app.MapGet("/hello/{name}", (string name) => $"Hello {name}, welcome to your financial future.");

// ──────────────────────────────────────────────
// 4. Auth Endpoints (public — they issue tokens)
// ──────────────────────────────────────────────
app.MapPost("/api/auth/register", async (IMediator mediator, FinancialTwin.API.Features.Auth.Commands.RegisterUserCommand command) =>
{
    try
    {
        var token = await mediator.Send(command);
        return Results.Ok(new { Token = token });
    }
    catch (Exception ex)
    {
        return Results.Conflict(new { Error = ex.Message });
    }
});

app.MapPost("/api/auth/login", async (IMediator mediator, FinancialTwin.API.Features.Auth.Queries.LoginUserQuery query) =>
{
    var token = await mediator.Send(query);
    if (token is null)
    {
        return Results.Unauthorized();
    }
    return Results.Ok(new { Token = token });
});


// ═══════════════════════════════════════════════
// PROTECTED Endpoints (valid JWT required)
// ═══════════════════════════════════════════════

// ──────────────────────────────────────────────
// GET /api/users/me — fetch the logged-in user's profile
// ──────────────────────────────────────────────
app.MapGet("/api/users/me", async (IMediator mediator, HttpContext httpContext) =>
{
    var userIdClaim = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                   ?? httpContext.User.FindFirst("sub");

    if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
        return Results.Unauthorized();

    var user = await mediator.Send(new FinancialTwin.API.Features.Users.Queries.GetCurrentUserQuery(userId));

    if (user is null)
        return Results.NotFound(new { Error = "User not found." });

    return Results.Ok(new
    {
        user.Id,
        user.Name,
        user.Email,
        user.CurrentSavings,
        user.Currency
    });
}).RequireAuthorization();

// ──────────────────────────────────────────────
// GET /api/simulations — list the logged-in user's simulations
// ──────────────────────────────────────────────
app.MapGet("/api/simulations", async (IMediator mediator, HttpContext httpContext) =>
{
    var userIdClaim = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                   ?? httpContext.User.FindFirst("sub");

    if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
        return Results.Unauthorized();

    var simulations = await mediator.Send(new FinancialTwin.API.Features.Simulations.Queries.GetUserSimulationsQuery(userId));
    return Results.Ok(simulations);
}).RequireAuthorization();

app.MapPost("/api/users", async (IMediator mediator, FinancialTwin.API.Features.Users.Commands.CreateUserCommand command) =>
{
    var userId = await mediator.Send(command);
    return Results.Created($"/api/users/{userId}", new { Id = userId });
}).RequireAuthorization();

app.MapPost("/api/simulations", async (IMediator mediator, FinancialTwin.API.Features.Simulations.Commands.RunSimulationCommand command) =>
{
    var simulation = await mediator.Send(command);
    return Results.Ok(simulation);
}).RequireAuthorization();

app.MapPost("/api/ai/test", async (FinancialTwin.API.Services.IAiService aiService, string prompt) =>
{
    var advice = await aiService.GetAdviceAsync(prompt);
    return Results.Ok(new { Response = advice });
}).RequireAuthorization();

app.Run();
