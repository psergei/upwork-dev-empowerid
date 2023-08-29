using CommentsContracts;
using Microsoft.EntityFrameworkCore;

namespace CommentsService.Repositories;

public class CommentsRepository : ICommentsRepository
{
  private readonly ApiDbContext _context;
  private readonly DbSet<PostComment> _comments;

  public CommentsRepository(ApiDbContext context)
  {
    _context = context;
    _comments = context.Set<PostComment>();
  }

  public async Task<IEnumerable<PostComment>> GetPostComments(Guid postId)
  {
    return await _comments
      .AsNoTracking()
      .Where(c => c.PostId == postId).ToListAsync();
  }

  public void AddComment(PostComment comment)
  {
    _comments.Add(comment);
  }

  public void DeletePostComments(Guid postId)
  {
    // NB: acoid use of EF for such things, consider direct SQL to delete records
    // other may hit peformance issues on large posts
    var postComments = _comments
      .AsNoTracking()
      .Where(c => c.PostId == postId);
    _comments.RemoveRange(postComments);
  }

  public async Task<bool> SaveChangesAsync()
  {
    return await _context.SaveChangesAsync() > 0;
  }
}