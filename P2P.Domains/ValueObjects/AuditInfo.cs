namespace P2P.Domains.ValueObjects;

public record AuditInfo
{
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; init; }
    public DateTime? LastLoginDate { get; init; }
    public string? IPAddress { get; init; }
};