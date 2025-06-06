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
    
    // PayStack Settings
    public string PaystackSecretKey { get; }
    public string PaystackPublicKey { get; }
    public string PaystackBaseUrl { get; }
    public List<string> PaystackAllowedIPs { get; }


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
        
        // PayStack Settings
        PaystackSecretKey = Environment.GetEnvironmentVariable("PAYSTACK_SECRET_KEY") ?? throw new InvalidOperationException("PAYSTACK_SECRET_KEY environment variable is not set");
        PaystackPublicKey = Environment.GetEnvironmentVariable("PAYSTACK_PUBLIC_KEY") ?? throw new InvalidOperationException("PAYSTACK_PUBLIC_KEY environment variable is not set");
        PaystackBaseUrl = Environment.GetEnvironmentVariable("PAYSTACK_BASE_URL") ?? "https://api.paystack.co";
        
        // Get PayStack allowed IPs
        PaystackAllowedIPs = new List<string>();
        
        // Add IPs from environment variables
        string? ip1 = Environment.GetEnvironmentVariable("PAYSTACK_ALLOWED_IP_1");
        string? ip2 = Environment.GetEnvironmentVariable("PAYSTACK_ALLOWED_IP_2");
        string? ip3 = Environment.GetEnvironmentVariable("PAYSTACK_ALLOWED_IP_3");
        
        if (!string.IsNullOrEmpty(ip1)) PaystackAllowedIPs.Add(ip1);
        if (!string.IsNullOrEmpty(ip2)) PaystackAllowedIPs.Add(ip2);
        if (!string.IsNullOrEmpty(ip3)) PaystackAllowedIPs.Add(ip3);
        
        // Set default PayStack IPs if none provided
        if (PaystackAllowedIPs.Count == 0)
        {
            PaystackAllowedIPs.Add("52.31.139.75");
            PaystackAllowedIPs.Add("52.49.173.169");
            PaystackAllowedIPs.Add("52.214.14.220");
        }
    }
}