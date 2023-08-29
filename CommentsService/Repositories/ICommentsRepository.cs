using CommentsContracts;

namespace CommentsService.Repositories;

public interface ICommentsRepository
{
  Task<IEnumerable<PostComment>> GetPostComments(Guid postId);
  void AddComment(PostComment comment);
  void DeletePostComments(Guid postId);
  Task<bool> SaveChangesAsync();
}