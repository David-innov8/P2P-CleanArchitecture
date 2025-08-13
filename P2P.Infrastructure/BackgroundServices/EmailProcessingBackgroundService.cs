using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using P2P.Application.UseCases.Interfaces.EmailService;

namespace P2P.Infrastructure.BackgroundServices;

public class EmailProcessingBackgroundService:BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EmailProcessingBackgroundService> _logger;
    private readonly TimeSpan _processingInterval = TimeSpan.FromSeconds(30);
    
    public EmailProcessingBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<EmailProcessingBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Email Processing Background Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessEmailBatch(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing email batch");
            }

            await Task.Delay(_processingInterval, stoppingToken);
        }

        _logger.LogInformation("Email Processing Background Service stopped");
    }

    
    private async Task ProcessEmailBatch(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var emailOutboxService = scope.ServiceProvider.GetRequiredService<IEmailOutboxService>();

        var processedCount = await emailOutboxService.ProcessPendingEmailsAsync(batchSize: 20);

        if (processedCount > 0)
        {
            _logger.LogInformation("Processed {Count} emails in this batch", processedCount);
        }
    }
    
    
    
    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Email Processing Background Service is stopping");
        await base.StopAsync(stoppingToken);
    }
}