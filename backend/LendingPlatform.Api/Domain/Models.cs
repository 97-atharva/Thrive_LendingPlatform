namespace LendingPlatform.Api.Domain;

public sealed record LoanApplicationRequest(decimal LoanAmount, decimal AssetValue, int CreditScore);

public sealed record LoanDecisionResponse(
    Guid ApplicationId,
    string Status,
    decimal LoanToValuePercent,
    string Reason,
    DateTimeOffset SubmittedAt);

public sealed record PortfolioResponse(
    int TotalApplications,
    int SuccessfulApplications,
    int DeclinedApplications,
    decimal TotalLoansWritten,
    decimal MeanLoanToValuePercent);

public sealed record ApplicationRecord(
    Guid Id,
    decimal LoanAmount,
    decimal LoanToValuePercent,
    Decision Decision,
    DateTimeOffset SubmittedAt);

public sealed record Decision(bool IsApproved, string Reason)
{
    public string Status => IsApproved ? "Successful" : "Declined";
}
