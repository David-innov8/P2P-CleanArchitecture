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
DotEnv.Load();

var appSettings = new AppSettings();
builder.Services.AddSingleton(appSettings);
// Add DbContext
builder.Services.AddDbContext<P2pContext>(options => {
    var appSettings = builder.Services.BuildServiceProvider().GetRequiredService<AppSettings>();
    options.UseSqlServer(appSettings.ConnectionString);
});

builder.Services.Configure<JsonSerializerOptions>(options => {
    options.IgnoreReadOnlyProperties = true;
    options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.Converters.Add(new JsonStringEnumConverter());
});

builder.Configuration.AddEnvironmentVariables();

//DI
builder.Services.AddScoped<IGetReciepientDetailsUSeCase , GetReceipeintDetailsUseCase>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IRegisterUserUseCase, SignUpCase>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<ILoginUserUseCase, LoginCase > ();
builder.Services.AddScoped<IUpdatePasswordUseCase, UpdatePasswordCase > ();
builder.Services.AddTransient<ISmtpService, SmtpService>();
builder.Services.AddScoped<IResetPasswordUseCase, ResetPasswordUseCase>();
builder.Services.AddScoped<IForgotPasswordCase,ForgetPasswordCase>();
builder.Services.AddScoped<ISetPinCase,SetPinCase>();
builder.Services.AddScoped<ITransactionsRepository, TransactionRepository>();
builder.Services.AddScoped<ITransferCase, TransferUseCase>();
builder.Services.AddScoped<ISendOtpCase, SendOtpCase>();
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUpdateUserUseCase,UpdateUserDetailsCase>();
builder.Services.AddScoped<ITransactionHistory, GetTransactionHistoryUseCase>();
builder.Services.AddScoped<IGetUserAccountDetails, GetUserAccountDetails>();
builder.Services.AddScoped<IAccountNumberGenerator, AccountNumberGenerator>();
builder.Services.AddScoped<IGLService,GeneralLedgerService>();
builder.Services.AddScoped<IGLRepository, GLRepository>();
builder.Services.AddScoped<IInitializeGlCase, InitializeGlUseCase>();
builder.Services.AddScoped<IGLTransactionRepository, GLTransactionRepository>();
builder.Services.AddScoped<SignUpValidator>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//redis configuration 
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = ConfigurationOptions.Parse("localhost:6379,abortConnect=false");
    return ConnectionMultiplexer.Connect(configuration);
});
// Add Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    c =>
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
    }
);

// Add CORS Policy (optional)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// Add IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Configure JWT
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
//
// var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
// var key = Encoding.UTF8.GetBytes(jwtSettings.Key);

// Configure JWT Authentication
// builder.Services.AddAuthentication(options =>
// {
//     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//     options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
// })
// .AddJwtBearer(options =>
// {
//     options.TokenValidationParameters = new TokenValidationParameters
//     {
//         ValidateIssuer = true,
//         ValidateAudience = true,
//         ValidateLifetime = true,
//         ValidateIssuerSigningKey = true,
//         ValidIssuer = jwtSettings.Issuer,
//         ValidAudience = jwtSettings.Audience,
//         IssuerSigningKey = new SymmetricSecurityKey(key)
//     };
// });

builder.Services.AddAuthentication(options => {
    // Authentication setup
}).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.JwtKey)),
        ValidateIssuer = true,
        ValidIssuer = appSettings.JwtIssuer,
        ValidateAudience = true,
        ValidAudience = appSettings.JwtAudience,
        ValidateLifetime = true
    };
});



// Add Authorization
builder.Services.AddAuthorization();

// -----------------------------
// ⏯️ Build the App
// -----------------------------

var app = builder.Build();

// Use Swagger in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    using var scope = app.Services.CreateScope();
    var initializeGlCase = scope.ServiceProvider.GetRequiredService<IInitializeGlCase>();
    try
    {
        await initializeGlCase.InitializeSystemGLs(); // Initializes all system GLs
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error initializing GLs: {ex.Message}");
    }
}

// -----------------------------
// 🚦 Configure Middleware
// -----------------------------

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
