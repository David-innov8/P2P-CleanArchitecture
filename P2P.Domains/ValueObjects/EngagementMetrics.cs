namespace P2P.Domains.ValueObjects;

public record EngagementMetrics
{
    public int LoginCount { get; private set; }
    public bool IsActive { get; private set; } = true;
    public decimal TotalTimeSpent { get; private set; }
    public string? Tags { get; private set; }
    public int EngagementScore { get; private set; }

    public void IncrementLoginCount()
    {
        LoginCount++;
        EngagementScore += 5; // Example scoring logic
    }
};