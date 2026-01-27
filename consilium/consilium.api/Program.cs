using Consilium.API;
using Consilium.API.DBServices;
using EmailAuthenticator;
using Npgsql;
using System.Data;
// hi
var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

string connString = builder.Configuration["DB_CONN"] ?? throw new Exception("No connection string was found.");
builder.Services.AddSingleton<IDbConnection>(provider =>
{
    return new NpgsqlConnection(connString);
});

builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddSingleton<IIDMiddlewareConfig, MiddlewareConfig>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IDBService, DBService>();

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
app.UseMiddleware<IdentityMiddleware>();
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