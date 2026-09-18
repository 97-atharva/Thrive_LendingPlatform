using LendingPlatform.Api.Domain;

namespace LendingPlatform.Tests;

public sealed class LendingDecisionEngineTests
{
    private readonly LendingDecisionEngine _engine = new();

    [Theory]
    [InlineData(99_999, 50, 999, false)]
    [InlineData(1_500_001, 50, 999, false)]
    [InlineData(1_000_000, 60, 950, true)]
    [InlineData(1_000_000, 60.01, 999, false)]
    [InlineData(1_000_000, 50, 949, false)]
    [InlineData(999_999, 59.99, 750, true)]
    [InlineData(999_999, 60, 799, false)]
    [InlineData(999_999, 60, 800, true)]
    [InlineData(999_999, 80, 899, false)]
    [InlineData(999_999, 80, 900, true)]
    [InlineData(999_999, 90, 999, false)]
    public void Evaluates_business_rules(decimal amount, decimal ltv, int score, bool expectedApproval)
    {
        var result = _engine.Evaluate(amount, ltv, score);
        Assert.Equal(expectedApproval, result.IsApproved);
    }
}
