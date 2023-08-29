using PostsContracts;

namespace PostsService.Services;

public interface IServiceBus
{
  Task SendPostAsync(BlogPost post);
  Task DeletePostAsync(Guid id);
}