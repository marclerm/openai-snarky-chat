using Microsoft.Extensions.Options;
using SnarkyChat.Api.Models;
using SnarkyChat.Api.Services;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();
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
    client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", apiKey);
});

const string CorsPolicy = "ReactDev";
builder.Services.AddCors(o =>
{
    o.AddPolicy(CorsPolicy, p =>
        p.WithOrigins("http://localhost:5173") // Vite dev server
         .AllowAnyHeader()
         .AllowAnyMethod()
    );
});

// App services
builder.Services.AddScoped<LlmClientService>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseCors(CorsPolicy);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.Run();
