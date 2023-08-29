using Microsoft.EntityFrameworkCore;
using PostsContracts;

namespace PostsService.Repositories;

public class PostsRepository: IPostsRepository
{
  private readonly ApiDbContext _context;
  private readonly DbSet<BlogPost> _posts;

  public PostsRepository(ApiDbContext context)
  {
    _context = context;
    _posts = context.Set<BlogPost>();
  }

  public async Task<IEnumerable<BlogPost>> GetAllPostsAsync()
  {
    return await _posts
      .AsNoTracking()
      .ToListAsync();
  }

  public async Task<BlogPost?> GetPostByIdAsync(Guid id)
  {
    return await _posts
      .AsNoTracking()
      .FirstOrDefaultAsync(p => p.Id == id);
  }

  public void AddPost(BlogPost post)
  {
    _posts.Add(post);
  }

  public void UpdatePost(BlogPost post)
  {
    _posts.Update(post);
  }

  public void DeletePost(BlogPost post)
  {
    _posts.Remove(post);
  }

  public async Task<bool> SaveChangesAsync()
  {
    return await _context.SaveChangesAsync() > 0;
  }
}