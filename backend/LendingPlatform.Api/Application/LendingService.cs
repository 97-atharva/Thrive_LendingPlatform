using LendingPlatform.Api.Domain;
using LendingPlatform.Api.Infrastructure;

namespace LendingPlatform.Api.Application;

public sealed class LendingService
{
    private readonly IDecisionEngine _decisionEngine;
    private readonly IApplicationRepository _repository;

    public LendingService(
        IDecisionEngine decisionEngine,
        IApplicationRepository repository)
    {
        _decisionEngine = decisionEngine;
        _repository = repository;
    }

    public LoanDecisionResponse Submit(LoanApplicationRequest request)
    {
        decimal ltv = decimal.Round(
            request.LoanAmount / request.AssetValue * 100m,
            2,
            MidpointRounding.AwayFromZero);

        Decision decision = _decisionEngine.Evaluate(
            request.LoanAmount,
            ltv,
            request.CreditScore);

        var application = new ApplicationRecord(
            Guid.NewGuid(),
            request.LoanAmount,
            ltv,
            decision,
            DateTimeOffset.UtcNow);

        _repository.Add(application);

        return new LoanDecisionResponse(
            application.Id,
            decision.Status,
            ltv,
            decision.Reason,
            application.SubmittedAt);
    }

    public PortfolioResponse GetPortfolio()
    {
        var applications = _repository.GetAll();

        var approvedLoans = applications
            .Where(a => a.Decision.IsApproved)
            .ToList();

        decimal meanLtv = applications.Count == 0
            ? 0m
            : decimal.Round(
                applications.Average(a => a.LoanToValuePercent),
                2,
                MidpointRounding.AwayFromZero);

        return new PortfolioResponse(
            applications.Count,
            approvedLoans.Count,
            applications.Count - approvedLoans.Count,
            approvedLoans.Sum(a => a.LoanAmount),
            meanLtv);
    }
}