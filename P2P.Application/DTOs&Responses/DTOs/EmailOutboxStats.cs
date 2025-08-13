namespace P2P.Application.DTOs;

public class EmailOutboxStats
{
    public int PendingCount { get; set; }
    public int ProcessedTodayCount { get; set; }
    public int FailedCount { get; set; }
    public int DeadLetterCount { get; set; }
}