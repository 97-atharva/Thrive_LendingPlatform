namespace LendingPlatform.Api.Domain;

public interface IDecisionEngine
{
    Decision Evaluate(decimal loanAmount, decimal loanToValuePercent, int creditScore);
}

public sealed class LendingDecisionEngine : IDecisionEngine
{
    private const decimal Million = 1_000_000m;

    public Decision Evaluate(decimal loanAmount, decimal loanToValuePercent, int creditScore)
    {
        if (loanAmount < 100_000m || loanAmount > 1_500_000m)
            return new(false, "Loan amount must be between GBP 100,000 and GBP 1,500,000.");

        if (loanAmount >= Million)
        {
            if (loanToValuePercent > 60m)
                return new(false, "Loans of GBP 1,000,000 or more require an LTV of 60% or less.");
            return creditScore >= 950
                ? new(true, "Meets the high-value loan LTV and credit-score requirements.")
                : new(false, "Loans of GBP 1,000,000 or more require a credit score of at least 950.");
        }

        if (loanToValuePercent >= 90m)
            return new(false, "Loans below GBP 1,000,000 are declined when LTV is 90% or more.");

        var requiredScore = loanToValuePercent < 60m ? 750 : loanToValuePercent < 80m ? 800 : 900;
        return creditScore >= requiredScore
            ? new(true, $"Meets the {requiredScore}+ credit-score requirement for this LTV band.")
            : new(false, $"This LTV band requires a credit score of at least {requiredScore}.");
    }
}
