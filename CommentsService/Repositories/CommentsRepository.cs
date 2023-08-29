using CommentsContracts;
using Microsoft.EntityFrameworkCore;

namespace CommentsService.Repositories;

/// <summary>
/// Blog post comments repository 
/// </summary>
public class CommentsRepository : ICommentsRepository
{
  private readonly ApiDbContext _context;
  private readonly DbSet<PostComment> _comments;

  public CommentsRepository(ApiDbContext context)
  {
    _context = context;
    _comments = context.Set<PostComment>();
  }

  /// <summary>
  /// Returns specific post comments
  /// </summary>
  /// <param name="postId">Post Id</param>
  /// <returns>comments list</returns>
  public async Task<IEnumerable<PostComment>> GetPostComments(Guid postId)
  {
    return await _comments
      .AsNoTracking()
      .Where(c => c.PostId == postId).ToListAsync();
  }

  /// <summary>
  /// Adds a new comment
  /// </summary>
  /// <param name="comment"></param>
  public void AddComment(PostComment comment)
  {
    _comments.Add(comment);
  }

  /// <summary>
  /// Delete all post comments
  /// </summary>
  /// <param name="postId">Post Id</param>
  public void DeletePostComments(Guid postId)
  {
    // NB: avoid use of EF for such things, consider direct SQL to delete records
    // other may hit peformance issues on large posts
    var postComments = _comments
      .AsNoTracking()
      .Where(c => c.PostId == postId);
    _comments.RemoveRange(postComments);
  }

  /// <summary>
  /// Save pending changes to the database
  /// </summary>
  /// <returns></returns>
  public async Task<bool> SaveChangesAsync()
  {
    return await _context.SaveChangesAsync() > 0;
  }
}