using PostsContracts;

namespace PostsService.Repositories;

public interface IServiceBus
{
  Task SendPostAsync(BlogPost post);
  Task DeletePostAsync(Guid id);
}