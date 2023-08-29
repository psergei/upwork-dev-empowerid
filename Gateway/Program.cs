using System.Threading.RateLimiting;
using Gateway.Clients;
using Gateway.Controllers;
using Gateway.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Identity.Web;
using Polly;
using Polly.Extensions.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add authentication configuration, will be using JWT and Azure AD as requested
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAD"));
builder.Services.AddAuthorization();
builder.Services.AddCors();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IBlogUserService, BlogUserService>();

builder.Services.AddStackExchangeRedisCache(options =>
{
  options.Configuration = builder.Configuration["RedisConnectionString"];
});

// Create rate limit policies
var readDataPolicy = "read-data-policy";
var writeDataPolicy = "write-data-policy";

builder.Services.AddRateLimiter(options => options
  .AddFixedWindowLimiter(readDataPolicy, limiter =>
  {
    limiter.PermitLimit = 4;
    limiter.Window = TimeSpan.FromSeconds(5);
    limiter.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    limiter.QueueLimit = 10;
  })
  .AddFixedWindowLimiter(writeDataPolicy, limiter =>
  {
    limiter.PermitLimit = 2;
    limiter.Window = TimeSpan.FromSeconds(10);
    limiter.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    limiter.QueueLimit = 2;
  }));

// Add retry/circuit breaker for BlogApi and also handler for certificate errors
builder.Services.AddHttpClient<IBlogApiClient, BlogApiClient>()
  .ConfigurePrimaryHttpMessageHandler(configHandler => GetMessageHandler())
  .AddPolicyHandler(GetRetryPolicy())
  .AddPolicyHandler(GetCircuitBreakerPolicy());

var app = builder.Build();

app.UseCors(builder =>
{
  builder
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowAnyOrigin();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseHttpsRedirection();

app.MapGatewayControllerEndpoints(readDataPolicy, writeDataPolicy);

app.Run();

/// <summary>
/// Define simple retry policy
/// </summary>
/// <returns></returns>
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
  return HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(2, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}

/// <summary>
/// Define simple circuit breaker policy
/// </summary>
/// <returns></returns>
static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
  return HttpPolicyExtensions
    .HandleTransientHttpError()
    .CircuitBreakerAsync(3, TimeSpan.FromSeconds(10));
}

/// <summary>
/// Workaround for non-truested development certificates, still having issues under Ubuntu
/// </summary>
/// <returns></returns>
static HttpClientHandler GetMessageHandler()
{
  return new HttpClientHandler
  {
    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
    {
      return true;
    }
  };
}