using P2P.Domains.ValueObjects;

namespace P2P.Domains.Entities;

public class User
{
    protected User() { }
    public Guid Id { get; set; }
    public string Username { get; set; }
    
   
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    
    
    // authetication 
    public PasswordHash Password { get; private set; }
    public PinHash? Pin { get; private set; }
    
    public UserProfile Profile { get; private set; }
    public AccountState State { get; private set; }
    public EngagementMetrics EngagementMetrics { get; private set; }
    public ConsentSettings Consent { get; private set; }
    public AuditInfo Audit { get; private set; }

    public ICollection<Account> Accounts { get; private set; } = new List<Account>();

    public User(string username, string email, string phoneNumber, PasswordHash password, string firstName, string lastName, Gender? gender, DateTime? dob)
    {
        // Validate Username
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username is required.", nameof(username));

        // Validate Email
        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            throw new ArgumentException("Invalid email address.", nameof(email));

        // Validate PhoneNumber
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number is required.", nameof(phoneNumber));

        // Validate Password
        if (password == null)
            throw new ArgumentNullException(nameof(password), "Password is required.");

        Id = Guid.NewGuid();
        Username = username;
        Email = email;
        PhoneNumber = phoneNumber;
        Password = password;


        Profile = new UserProfile
        {
            FirstName = firstName,
            LastName = lastName,
            Gender = gender,
            DOB = dob,
          
            
        };
        State = new AccountState();
        Audit = new AuditInfo();
        EngagementMetrics = new EngagementMetrics();
        Consent = new ConsentSettings();
    }
    public void SetPin(PinHash pin)
    {
        if (pin == null)
            throw new ArgumentNullException(nameof(pin), "Pin is required.");

        Pin = pin;
        Audit = Audit with { UpdatedAt = DateTime.Now };
    }
    // update methods 
    // public void UpdateEmail(string newEmail)
    // {
    //     if (string.IsNullOrWhiteSpace(newEmail) || !newEmail.Contains("@"))
    //         throw new ArgumentException("Invalid email address.", nameof(newEmail));
    //
    //     Email = newEmail;
    //     Audit = Audit with { UpdatedAt = DateTime.Now };
    // }
    public void UpdatePhoneNumber(string newPhoneNumber)
    {
        if (string.IsNullOrWhiteSpace(newPhoneNumber))
            throw new ArgumentException("Phone number is required.", nameof(newPhoneNumber));

        PhoneNumber = newPhoneNumber;
        Audit = Audit with { UpdatedAt = DateTime.Now };// Update audit info
    }

    // Method to update password
    public void UpdatePassword(PasswordHash newPassword)
    {
        if (newPassword == null)
            throw new ArgumentNullException(nameof(newPassword), "Password is required.");

        Password = newPassword;
        Audit = Audit with { UpdatedAt = DateTime.Now }; // Update audit info
    }

    // Method to update profile
    public void UpdateProfile(UserProfile newProfile)
    {
        if (newProfile == null)
            throw new ArgumentNullException(nameof(newProfile), "Profile is required.");

        Profile = newProfile;
        Audit = Audit with { UpdatedAt = DateTime.Now }; // Update audit info
    }
    
    public void AddAccount(Account account)
    {
        if (account == null)
            throw new ArgumentNullException(nameof(account));
        
        Accounts.Add(account);
        Audit = Audit with { UpdatedAt = DateTime.Now };
    }
}