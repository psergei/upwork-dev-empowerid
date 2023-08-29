using Microsoft.EntityFrameworkCore;
using PostsContracts;

namespace PostsService.Repositories;

/// <summary>
/// Blog posts repository
/// </summary>
public class PostsRepository: IPostsRepository
{
  private readonly ApiDbContext _context;
  private readonly DbSet<BlogPost> _posts;

  public PostsRepository(ApiDbContext context)
  {
    _context = context;
    _posts = context.Set<BlogPost>();
  }

  /// <summary>
  /// Returns all blog posts
  /// </summary>
  /// <returns></returns>
  public async Task<IEnumerable<BlogPost>> GetAllPostsAsync()
  {
    return await _posts
      .AsNoTracking()
      .ToListAsync();
  }

  /// <summary>
  /// Returns specific post by Id
  /// </summary>
  /// <param name="id">Post Id</param>
  /// <returns></returns>
  public async Task<BlogPost?> GetPostByIdAsync(Guid id)
  {
    return await _posts
      .AsNoTracking()
      .FirstOrDefaultAsync(p => p.Id == id);
  }

  /// <summary>
  /// Adds a new post
  /// </summary>
  /// <param name="post"></param>
  public void AddPost(BlogPost post)
  {
    _posts.Add(post);
  }

  /// <summary>
  /// Update existing post
  /// </summary>
  /// <param name="post"></param>
  public void UpdatePost(BlogPost post)
  {
    _posts.Update(post);
  }

  /// <summary>
  /// Delete post
  /// </summary>
  /// <param name="post"></param>
  public void DeletePost(BlogPost post)
  {
    _posts.Remove(post);
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