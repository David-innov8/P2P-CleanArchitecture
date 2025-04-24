namespace P2P.Domains.Entities;

public class Fees
{
    public Guid Id { get; private set; } // Use GUID for better uniqueness
    public string Key { get; private set; }
    public string Value { get; private set; }
    public string Description { get; private set; }

    // Constructor
    public Fees(string key, string value, string description)
    {
        Id = Guid.NewGuid();
        Key = key ?? throw new ArgumentNullException(nameof(key));
        Value = value ?? throw new ArgumentNullException(nameof(value));
        Description = description ?? throw new ArgumentNullException(nameof(description));
    }

    // Method to update the value
    public void UpdateValue(string newValue)
    {
        if (string.IsNullOrWhiteSpace(newValue))
            throw new ArgumentException("Value cannot be null or empty.");

        Value = newValue;
    }
}