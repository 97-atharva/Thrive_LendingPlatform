using LendingPlatform.Api.Domain;
using LendingPlatform.Api.Infrastructure;

namespace LendingPlatform.Api.Application;

public sealed class LendingService(IDecisionEngine decisionEngine, IApplicationRepository repository)
{
    public LoanDecisionResponse Submit(LoanApplicationRequest request)
    {
        var ltv = decimal.Round(request.LoanAmount / request.AssetValue * 100m, 2, MidpointRounding.AwayFromZero);
        var decision = decisionEngine.Evaluate(request.LoanAmount, ltv, request.CreditScore);
        var record = new ApplicationRecord(Guid.NewGuid(), request.LoanAmount, ltv, decision, DateTimeOffset.UtcNow);
        repository.Add(record);
        return new(record.Id, decision.Status, ltv, decision.Reason, record.SubmittedAt);
    }

    public PortfolioResponse GetPortfolio()
    {
        var applications = repository.GetAll();
        var successful = applications.Where(application => application.Decision.IsApproved).ToList();
        return new(
            applications.Count,
            successful.Count,
            applications.Count - successful.Count,
            successful.Sum(application => application.LoanAmount),
            applications.Count == 0 ? 0m : decimal.Round(applications.Average(application => application.LoanToValuePercent), 2, MidpointRounding.AwayFromZero));
    }
}

public static class RequestValidator
{
    public static Dictionary<string, string[]> Validate(LoanApplicationRequest request)
    {
        var errors = new Dictionary<string, string[]>();
        if (request.LoanAmount <= 0) errors["loanAmount"] = ["Loan amount must be greater than zero."];
        if (request.AssetValue <= 0) errors["assetValue"] = ["Asset value must be greater than zero."];
        if (request.CreditScore is < 1 or > 999) errors["creditScore"] = ["Credit score must be between 1 and 999."];
        return errors;
    }
}
