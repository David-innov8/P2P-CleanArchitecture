using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using dotenv.net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using P2p_Clean_Architecture________b;
using P2P.Application.Interfaces.Repositories;
using P2P.Application.UseCases;
using P2P.Application.UseCases.AccountCases;
using P2P.Application.UseCases.Interfaces;
using P2P.Application.UseCases.Interfaces.GeneralLedgers;
using P2P.Application.UseCases.Interfaces.Paystack;
using P2P.Application.UseCases.Interfaces.Transfer;
using P2P.Application.UseCases.Interfaces.UserAccounts;
using P2P.Application.Validators;
using P2P.Infrastructure.Context;
using P2P.Infrastructure.Repositories;
using P2P.Infrastructure.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------
// 🔧 Configure Services
// -----------------------------

// Load environment variables regardless of environment
DotEnv.Load();
builder.Configuration.AddEnvironmentVariables();

// Configure AppSettings properly with configuration binding
builder.Services.Configure<AppSettings>(builder.Configuration);
builder.Services.AddSingleton<AppSettings>(provider => 
{
    var config = provider.GetRequiredService<IConfiguration>();
    var appSettings = new AppSettings();
    config.Bind(appSettings);
    return appSettings;
});

// Add DbContext with proper dependency injection
builder.Services.AddDbContext<P2pContext>((serviceProvider, options) => {
    var appSettings = serviceProvider.GetRequiredService<AppSettings>();
    options.UseSqlServer(appSettings.ConnectionString);
});

builder.Services.Configure<JsonSerializerOptions>(options => {
    options.IgnoreReadOnlyProperties = true;
    options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.Converters.Add(new JsonStringEnumConverter());
});

//DI Registration
builder.Services.AddScoped<IGetReciepientDetailsUSeCase, GetReceipeintDetailsUseCase>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IRegisterUserUseCase, SignUpCase>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<ILoginUserUseCase, LoginCase>();
builder.Services.AddScoped<IUpdatePasswordUseCase, UpdatePasswordCase>();
builder.Services.AddTransient<ISmtpService, SmtpService>();
builder.Services.AddScoped<IResetPasswordUseCase, ResetPasswordUseCase>();
builder.Services.AddScoped<IForgotPasswordCase, ForgetPasswordCase>();
builder.Services.AddScoped<ISetPinCase, SetPinCase>();
builder.Services.AddScoped<ITransactionsRepository, TransactionRepository>();
builder.Services.AddScoped<ITransferCase, TransferUseCase>();
builder.Services.AddScoped<ISendOtpCase, SendOtpCase>();
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUpdateUserUseCase, UpdateUserDetailsCase>();
builder.Services.AddScoped<ITransactionHistory, GetTransactionHistoryUseCase>();
builder.Services.AddScoped<IGetUserAccountDetails, GetUserAccountDetails>();
builder.Services.AddScoped<IAccountNumberGenerator, AccountNumberGenerator>();
builder.Services.AddScoped<IGLService, GeneralLedgerService>();
builder.Services.AddScoped<IGLRepository, GLRepository>();
builder.Services.AddScoped<IInitializeGlCase, InitializeGlUseCase>();
builder.Services.AddScoped<IGLTransactionRepository, GLTransactionRepository>();
builder.Services.AddScoped<SignUpValidator>();
builder.Services.AddScoped<IPaystackService, PayStackService>();
builder.Services.AddHttpClient();

// Note: IUnitOfWork is registered twice in your original code - keeping only one
// builder.Services.AddScoped<IUnitOfWork, UnitOfWork>(); // Duplicate removed

// Redis configuration 
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = ConfigurationOptions.Parse("localhost:6379,abortConnect=false");
    return ConnectionMultiplexer.Connect(configuration);
});

// Add Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Jwt Authorization header using the bearer scheme. Enter `Bearer` [space] and your token in the text input below",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement{
        {
            new OpenApiSecurityScheme(){
                Reference = new OpenApiReference{
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});

// Add CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// Add IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Configure JWT and SMTP settings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var serviceProvider = builder.Services.BuildServiceProvider();
    var appSettings = serviceProvider.GetRequiredService<AppSettings>();
    
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.JwtKey)),
        ValidateIssuer = true,
        ValidIssuer = appSettings.JwtIssuer,
        ValidateAudience = true,
        ValidAudience = appSettings.JwtAudience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Add Authorization
builder.Services.AddAuthorization();

// Add logging
builder.Services.AddLogging();

// -----------------------------
// ⏯️ Build the App
// -----------------------------

var app = builder.Build();

// Add error handling and logging
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.Logger.LogInformation("Application starting...");

// Use Swagger in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "P2P API V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "P2P API V1");
    c.RoutePrefix = "swagger";
});
// Initialize GLs (moved to try-catch and added logging)
try
{
    using var scope = app.Services.CreateScope();
    var initializeGlCase = scope.ServiceProvider.GetRequiredService<IInitializeGlCase>();
    await initializeGlCase.InitializeSystemGLs(); // Initializes all system GLs
    app.Logger.LogInformation("System GLs initialized successfully");
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "Error initializing GLs: {Message}", ex.Message);
    // Don't throw here - let the app continue to start
}

// -----------------------------
// 🚦 Configure Middleware
// -----------------------------

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapControllers();

app.Logger.LogInformation("Application started successfully");

app.Run();