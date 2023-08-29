using System.Text.Json;
using Azure.Messaging.ServiceBus;
using PostsContracts;

namespace PostsService.Repositories;

public class ServiceBus : IServiceBus
{
  private readonly string _connectionString;
  private readonly string _queueName;

  public ServiceBus(IConfiguration config)
  {
    _connectionString = config["ServiceBus"] ?? string.Empty;
    _queueName = config["QueueName"] ?? string.Empty;
  }

  public async Task SendPostAsync(BlogPost post)
  {
    var body = JsonSerializer.Serialize(post);
    await SendMessageAsync(body, "post-upserted");
  }

  public async Task DeletePostAsync(Guid id)
  {
    await SendMessageAsync(id.ToString(), "post-deleted");
  }

  private async Task SendMessageAsync(string body, string subject)
  {
    await using ServiceBusClient client = new (_connectionString);
    await using ServiceBusSender sender = client.CreateSender(_queueName);

    var msg = new ServiceBusMessage(body)
    {
      Subject = subject
    };

    await sender.SendMessageAsync(msg);
  }
}