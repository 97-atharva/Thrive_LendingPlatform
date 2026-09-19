using LendingPlatform.Api.Application;
using LendingPlatform.Api.Domain;
using LendingPlatform.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddSingleton<IDecisionEngine, LendingDecisionEngine>();
builder.Services.AddSingleton<IApplicationRepository, InMemoryApplicationRepository>();
builder.Services.AddSingleton<LendingService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "http://localhost:5174",
                "http://localhost:5175"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Enable CORS BEFORE endpoints
app.UseCors("AllowFrontend");

// POST: Submit Loan Application
app.MapPost("/api/applications",
    (LoanApplicationRequest request, LendingService service) =>
{
    try
    {
        var validation = RequestValidator.Validate(request);

        if (validation.Count > 0)
        {
            return Results.ValidationProblem(validation);
        }

        var result = service.Submit(request);

        Console.WriteLine($"Application processed: {result.ApplicationId}");

        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ERROR: {ex}");

        return Results.Problem(
            title: "Application Processing Failed",
            detail: ex.Message,
            statusCode: 500
        );
    }
});

// GET: Portfolio Metrics
app.MapGet("/api/portfolio",
    (LendingService service) =>
{
        return Results.Ok(service.GetPortfolio());
});

// Health Check Endpoint
app.MapGet("/api/health",
    () => Results.Ok(new
    {
        Status = "Running",
        Timestamp = DateTime.UtcNow
    }));

app.Run();

public partial class Program;