namespace P2P.Domains.Entities;

public class EmailOutbox
{
    public int Id { get; private set; }
        public string To { get; private set; }
        public string Subject { get; private set; }
        public string TemplateName { get; private set; }
        public string PlaceholdersJson { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? ProcessedAt { get; private set; }
        public bool IsProcessed { get; private set; }
        public string? ErrorMessage { get; private set; }
        public int RetryCount { get; private set; }
        public DateTime? NextRetryAt { get; private set; }

        // Private constructor for EF Core
        private EmailOutbox() { }

        public EmailOutbox(string to, string subject, string templateName, Dictionary<string, string> placeholders)
        {
            To = to ?? throw new ArgumentNullException(nameof(to));
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
            TemplateName = templateName ?? throw new ArgumentNullException(nameof(templateName));
            PlaceholdersJson = System.Text.Json.JsonSerializer.Serialize(placeholders);
            CreatedAt = DateTime.UtcNow;
            IsProcessed = false;
            RetryCount = 0;
        }

        public void MarkAsProcessed()
        {
            IsProcessed = true;
            ProcessedAt = DateTime.UtcNow;
            ErrorMessage = null;
            NextRetryAt = null;
        }

        public void MarkAsFailed(string errorMessage)
        {
            RetryCount++;
            ErrorMessage = errorMessage;
            
            // Exponential backoff: 1min, 5min, 30min
            var delayMinutes = RetryCount switch
            {
                1 => 1,
                2 => 5,
                3 => 30,
                _ => 60
            };
            
            NextRetryAt = DateTime.UtcNow.AddMinutes(delayMinutes);
        }

        public bool ShouldRetry(int maxRetries = 3)
        {
            return !IsProcessed && RetryCount < maxRetries && 
                   (NextRetryAt == null || NextRetryAt <= DateTime.UtcNow);
        }

        public Dictionary<string, string> GetPlaceholders()
        {
            return System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(PlaceholdersJson) 
                   ?? new Dictionary<string, string>();
        }
    
}