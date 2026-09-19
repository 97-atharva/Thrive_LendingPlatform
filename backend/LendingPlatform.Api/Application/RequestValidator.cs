namespace LendingPlatform.Api.Application;
using LendingPlatform.Api.Domain;
public static class RequestValidator
{
    public static Dictionary<string, string[]> Validate(
        LoanApplicationRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (request.LoanAmount <= 0)
            errors["loanAmount"] =
                ["Loan amount must be greater than zero."];

        if (request.AssetValue <= 0)
            errors["assetValue"] =
                ["Asset value must be greater than zero."];

        if (request.CreditScore is < 1 or > 999)
            errors["creditScore"] =
                ["Credit score must be between 1 and 999."];

        return errors;
    }
}