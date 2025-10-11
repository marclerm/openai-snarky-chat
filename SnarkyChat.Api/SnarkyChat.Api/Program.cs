using Microsoft.Extensions.Options;
using SnarkyChat.Api.Models;
using SnarkyChat.Api.Services;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add the User Secrets in development to get access to the API key
builder.Configuration.AddUserSecrets<Program>();

// Configure OpenAI options from config section to get access to model name and base URL
builder.Services.Configure<OpenAIOptions>(
    builder.Configuration.GetSection("OpenAI"));

// Named HttpClient with auth header
builder.Services.AddHttpClient("openai", (sp, client) =>
{
    var cfg = sp.GetRequiredService<IOptions<OpenAIOptions>>().Value;
    client.BaseAddress = new Uri(cfg.BaseUrl);

    // Prefer env var, fallback to config section
    // Make sure to set this in your environment or user secrets
    // OPENAI_API_KEY is the standard name used by OpenAI SDKs
    var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
             ?? builder.Configuration["OPENAI_API_KEY"]
             ?? builder.Configuration["OpenAI:ApiKey"];


    if (string.IsNullOrWhiteSpace(apiKey))
        throw new InvalidOperationException("OPENAI_API_KEY not configured.");

    // Set the auth header for all requests made with this client instance to the OpenAI API
    // This is a Bearer token as per OpenAI spec (https://platform.openai.com/docs/api-reference/authentication)
    // We could also use client.DefaultRequestHeaders.Add("Authorization ...") but this is cleaner and more explicit
    client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", apiKey);
});

// CORS for React dev server
const string CorsPolicy = "ReactDev";
builder.Services.AddCors(o =>
{
    o.AddPolicy(CorsPolicy, p =>
        p.WithOrigins("http://localhost:5173") // Vite dev server
         .AllowAnyHeader()
         .AllowAnyMethod()
    );
});

// Application services
builder.Services.AddScoped<LlmClientService>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();
// Enable CORS
app.UseCors(CorsPolicy);

// Some default configuration created by VS
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.Run();
