🧭 FEATURE-BUILDING GUIDE (Clean Architecture Style)
Whenever you're creating a new feature (like “Transfer Money”, “Fund Wallet”, “Add Beneficiary”, etc), follow this step-by-step thinking pattern:

🪪 1. What’s the Feature Doing?
Ask yourself:

Who is using this?

What action are they performing?

What should happen in the system?

👉 Example:

A user wants to send money from Account A to Account B.

🧠 2. What’s the Business Logic? (→ Domain Layer)
Does this feature have rules that must always be true, no matter where or how it's called?

✅ Then:

Create or update your Entities (Account, Transaction)

Add any Value Objects (Money, AccountNumber, etc)

Add Domain Events if anything should happen "later" (e.g., notify receiver)

Add Domain Exceptions if things can go wrong (e.g., InsufficientBalanceException)

✅ Example:

Create a method on Account:

csharp
Copy
Edit
public void TransferTo(Account receiver, Money amount) { ... }
🧩 3. Who Coordinates the Action? (→ Application Layer)
This is your "use case".

✅ Create:

A Service or Handler (e.g., TransferService)

Define an Input DTO (e.g., TransferRequestDto)

Return an Output DTO (e.g., TransferResultDto)

Inject domain services or repositories

✅ Example:

csharp
Copy
Edit
public interface ITransferService
{
Task<TransferResultDto> SendAsync(TransferRequestDto dto);
}
💾 4. What Data Do We Need? (→ Repositories in Domain, Implementation in Infrastructure)
Where do you need to fetch/save this data from/to?

✅ Define in Domain:

IAccountRepository

ITransactionRepository

✅ Implement in Infrastructure:

AccountRepository : IAccountRepository using EF Core

🧪 5. What Should the User Interact With? (→ Presentation Layer)
How will a user trigger this action? API? Razor page?

✅ Create:

TransferController

Add endpoint:

csharp
Copy
Edit
[HttpPost("send")]
public async Task<IActionResult> Send([FromBody] TransferRequestDto dto)
✅ Validate input using:

DataAnnotations or FluentValidation

✅ Secure endpoint:

[Authorize]

🔌 6. Do We Need to Talk to the Outside World? (→ Infrastructure Layer)
✅ Use for:

Sending SignalR notifications (INotificationService)

Sending emails (IEmailService)

Logging to files or 3rd party

Calling APIs (e.g., exchange rates, fraud check)

📦 7. Organize the Folders
cpp
Copy
Edit
/Domain
/Entities
/ValueObjects
/Events
/Interfaces
/Exceptions

/Application
/DTOs
/Interfaces
/Services
/UseCases (optional)

/Infrastructure
/Repositories
/Services
/EF (DbContext, Configs)
/SignalR

/Presentation
/Controllers
/RequestModels
/ResponseModels
✅ 8. Final Checklist Before Moving On

Item	✅
Domain rules are in Entities/ValueObjects	✅
Application coordinates things via interfaces	✅
Infrastructure implements DB/API access	✅
Controller accepts and returns clean models	✅
Errors & validations handled clearly	✅
Domain events used if needed (for notifications, emails)	✅
Feature can be tested independently