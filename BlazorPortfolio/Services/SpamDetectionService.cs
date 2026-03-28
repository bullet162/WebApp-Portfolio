using BlazorPortfolio.Models;

namespace BlazorPortfolio.Services;

public static class SpamDetectionService
{
    private static readonly string[] SpamKeywords =
    [
        "crypto", "bitcoin", "earn money", "make money", "investment", "forex",
        "casino", "lottery", "prize", "winner", "click here", "free offer",
        "buy now", "limited time", "act now", "guaranteed", "100% free",
        "work from home", "passive income", "mlm", "affiliate"
    ];

    public static CollaborationStatus Evaluate(CollaborationRequest req)
    {
        var combined = $"{req.FirstName} {req.LastName} {req.Message} {req.PortfolioUrl} {req.Email}"
                       .ToLowerInvariant();

        // Contains spam keywords
        if (SpamKeywords.Any(kw => combined.Contains(kw)))
            return CollaborationStatus.Flagged;

        // Message contains links
        if (req.Message is not null &&
            (req.Message.Contains("http://", StringComparison.OrdinalIgnoreCase) ||
             req.Message.Contains("https://", StringComparison.OrdinalIgnoreCase) ||
             req.Message.Contains("www.", StringComparison.OrdinalIgnoreCase)))
            return CollaborationStatus.Flagged;

        // Repeated characters (e.g. "aaaaaaa")
        if (req.Message is not null && HasRepeatedChars(req.Message, 5))
            return CollaborationStatus.Flagged;

        // Very long random-looking strings in name fields
        if (HasRepeatedChars(req.FirstName, 4) || HasRepeatedChars(req.LastName, 4))
            return CollaborationStatus.Flagged;

        return CollaborationStatus.Pending;
    }

    private static bool HasRepeatedChars(string input, int threshold)
    {
        int count = 1;
        for (int i = 1; i < input.Length; i++)
        {
            if (char.ToLower(input[i]) == char.ToLower(input[i - 1])) count++;
            else count = 1;
            if (count >= threshold) return true;
        }
        return false;
    }
}
