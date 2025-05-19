namespace P2p_Clean_Architecture________b;

public class AppSettings
{
    // JWT Settings
    public string JwtKey { get; }
    public string JwtIssuer { get; }
    public string JwtAudience { get; }
    public int JwtDurationMinutes { get; }

    // SMTP Settings
    public string SmtpServer { get; }
    public int SmtpPort { get; }
    public string SmtpUsername { get; }
    public string SmtpSenderEmail { get; }
    public string SmtpPassword { get; }
    public bool SmtpEnableSsl { get; }

    // Database Connection
    public string ConnectionString { get; }

    public AppSettings()
    {
        // JWT Settings
        JwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? throw new InvalidOperationException("JWT_KEY environment variable is not set");
        JwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? throw new InvalidOperationException("JWT_ISSUER environment variable is not set");
        JwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? throw new InvalidOperationException("JWT_AUDIENCE environment variable is not set");
        JwtDurationMinutes = int.Parse(Environment.GetEnvironmentVariable("JWT_DURATION_MINUTES") ?? "60");

        // SMTP Settings
        SmtpServer = Environment.GetEnvironmentVariable("SMTP_SERVER")!;
        SmtpPort = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "587");
        SmtpUsername = Environment.GetEnvironmentVariable("SMTP_USERNAME")!;
        SmtpSenderEmail = Environment.GetEnvironmentVariable("SMTP_SENDER_EMAIL")!;
        SmtpPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD")!;
        SmtpEnableSsl = bool.Parse(Environment.GetEnvironmentVariable("SMTP_ENABLE_SSL") ?? "false");

        // Database Connection
        ConnectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? throw new InvalidOperationException("CONNECTION_STRING environment variable is not set");

    }
}