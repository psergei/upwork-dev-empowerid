using CommentsContracts;
using Gateway.Models;
using PostsContracts;

namespace Gateway.Clients;

public interface IBlogApiClient
{
  Task<IEnumerable<BlogPost>?> GetAllPosts();
  Task<BlogPost?> GetPostById(Guid id);
  Task CreatePost(BlogPost post);
  Task<bool> UpdatePost(Guid id, BlogPost post);
  Task<bool> DeletePost(Guid id);
  Task<bool> CreateComment(PostComment comment);
  Task<IEnumerable<PostComment>?> GetPostComments(Guid postId);
}