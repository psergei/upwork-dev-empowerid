using Microsoft.EntityFrameworkCore;
using CommentsContracts;

namespace CommentsService.Repositories;

public class ApiDbContext: DbContext
{
  public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
  {}

  public DbSet<PostComment> PostComments { get; set; } = default!;
}