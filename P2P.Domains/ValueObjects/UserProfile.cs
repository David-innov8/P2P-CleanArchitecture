namespace P2P.Domains.ValueObjects;

public record UserProfile
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public Gender? Gender { get; init; }
    public DateTime? DOB { get; init; }
    public byte[]? ProfileImage { get; init; }
    public string? Address { get; init; }
}