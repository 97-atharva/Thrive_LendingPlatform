using LendingPlatform.Api.Application;
using LendingPlatform.Api.Domain;
using LendingPlatform.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IDecisionEngine, LendingDecisionEngine>();
builder.Services.AddSingleton<IApplicationRepository, InMemoryApplicationRepository>();
builder.Services.AddSingleton<LendingService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();
app.UseCors("AllowFrontend");
app.MapPost("/api/applications", (LoanApplicationRequest request, LendingService service) =>
{
    try
    {
        var validation = RequestValidator.Validate(request);

        if (validation.Count > 0)
        {
            return Results.ValidationProblem(validation);
        }

        var result = service.Submit(request);

        Console.WriteLine("Application processed successfully");

        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ERROR: {ex}");
        return Results.Problem(ex.Message);
    }
});

app.MapGet("/api/portfolio",
    (LendingService service) => Results.Ok(service.GetPortfolio()));

app.Run();

public partial class Program;