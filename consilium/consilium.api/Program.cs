using Consilium.API;
using Consilium.API.DBServices;
using Consilium.API.Services;
using Npgsql;
using System.Data;
// hi
var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

string connString = builder.Configuration["DB_CONN"] ?? throw new Exception("No connection string was found.");
builder.Services.AddScoped<IDbConnection>(provider =>
{
    return new NpgsqlConnection(connString);
});

builder.Services.AddScoped<GoogleAuthService>();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173", 
                "https://localhost:5173",
                "https://consilium.mateo.tomaszeuski.com")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDBService, DBService>();

var feedbackUri = builder.Configuration["FEEDBACK_WEBHOOK"] ?? "";
if (!string.IsNullOrEmpty(feedbackUri)) {
    builder.Services.AddHttpClient("FeedbackWebhock", client => client.BaseAddress = new Uri(feedbackUri));
}

var app = builder.Build();

bool featureFlag = builder.Configuration["feature_flag"] == "true";

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || featureFlag) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors("AllowFrontend");
app.UseRouting();

//change
app.MapGet("", () =>
{
    app.Logger.LogInformation("Home page accessed");
    return "Welcome to the Consilium Api";
});

app.MapGet("/health", () =>
{
    return Results.Ok("healthy");
});

app.MapGet("/error", () =>
{
    throw new Exception("This is an error");
});

app.MapGet("/timecheck", () =>
{
    Task.Delay(60).Wait();
    return Results.Ok("done");
});

// Need to make a change to test formatting, test 3
if (featureFlag) {
    app.MapGet("/secret", () =>
    {
        app.Logger.LogInformation("Inside the secret feature flag");

        return "Secrets are hidden within.";
    });
}

app.MapControllers();

app.MapFallbackToFile("index.html");

app.Run();