using CommentsContracts;
using Gateway.Models;
using PostsContracts;

namespace Gateway.Clients;

public interface IBlogApiClient
{
  Task<IEnumerable<BlogPost>?> GetAllPosts();
  Task<BlogPost?> GetPostById(Guid id);
  Task CreatePost(BlogPost post);
  Task UpdatePost(Guid id, BlogPost post);
  Task DeletePost(Guid id);
  Task CreateComment(PostComment comment);
  Task<IEnumerable<PostComment>?> GetPostComments(Guid postId);
}