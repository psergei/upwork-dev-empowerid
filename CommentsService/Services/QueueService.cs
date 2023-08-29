using Azure.Messaging.ServiceBus;
using CommentsService.Repositories;

namespace CommentsService.Services;

/// <summary>
/// Hosted service for Azure ServiceBus listening
/// </summary>
public class QueueService : IHostedService, IDisposable
{
  private readonly string _connectionString;
  private readonly string _queueName;

  private readonly ServiceBusClient _client;
  private readonly ServiceBusProcessor _processor;

  private readonly IServiceProvider _serviceProvider;

  public QueueService(IConfiguration config, IServiceProvider serviceProvider)
  {
    _connectionString = config["ServiceBus"] ?? string.Empty;
    _queueName = config["QueueName"] ?? string.Empty;

    _client = new (_connectionString);
    _processor = _client.CreateProcessor(_queueName);

    _serviceProvider = serviceProvider;
  }

  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }

  public async Task StartAsync(CancellationToken cancellationToken)
  {
    _processor.ProcessMessageAsync += MessageHandler;
    _processor.ProcessErrorAsync += ErrorHandler;

    await _processor.StartProcessingAsync(cancellationToken);
  }

  public async Task StopAsync(CancellationToken cancellationToken)
  {
    await _processor.StopProcessingAsync(cancellationToken);
  }

  protected virtual async void Dispose(bool disposing)
  {
    if (disposing){
      await _processor.DisposeAsync();
      await _client.DisposeAsync();
    }
  }

  /// <summary>
  /// Handle message from service bus queue
  /// </summary>
  /// <param name="args"></param>
  /// <returns></returns>
  private async Task MessageHandler(ProcessMessageEventArgs args)
  {
    string body = args.Message.Body.ToString();
    string subject = args.Message.Subject.ToString();

    Console.WriteLine($"Got message subject: {subject}");

    if (subject == "post-deleted")
    {
      using (IServiceScope scope = _serviceProvider.CreateScope())
      {
        ICommentsRepository repo = scope.ServiceProvider.GetRequiredService<ICommentsRepository>();
        repo.DeletePostComments(Guid.Parse(body));
        await repo.SaveChangesAsync();
      }
    }
  }

  /// <summary>
  /// Handle error, just do nothing yet
  /// </summary>
  /// <param name="args"></param>
  /// <returns></returns>
  private Task ErrorHandler(ProcessErrorEventArgs args)
  {
    Console.WriteLine(args.Exception.ToString());
    return Task.CompletedTask;
  }
}