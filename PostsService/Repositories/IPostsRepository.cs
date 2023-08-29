using PostsContracts;

namespace PostsService.Repositories;

public interface IPostsRepository
{
  Task<IEnumerable<BlogPost>> GetAllPostsAsync();
  Task<BlogPost?> GetPostByIdAsync(Guid id);
  void AddPost(BlogPost post);
  void UpdatePost(BlogPost post);
  void DeletePost(BlogPost post);
  Task<bool> SaveChangesAsync();
}