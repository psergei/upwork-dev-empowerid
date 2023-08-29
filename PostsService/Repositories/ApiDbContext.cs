using Microsoft.EntityFrameworkCore;
using PostsContracts;

namespace PostsService.Repositories;

public class ApiDbContext: DbContext
{
  public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
  {}

  public DbSet<BlogPost> BlogPosts { get; set; } = default!;
}