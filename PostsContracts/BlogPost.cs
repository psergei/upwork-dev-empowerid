using System.ComponentModel.DataAnnotations;

namespace PostsContracts;

public sealed class BlogPost
{
  public required Guid Id { get; set; }
  [StringLength(200)]
  public required string Title  { get; set; }
  public required string Content  { get; set; }

  public required string Author  { get; set; }
  public required DateTime CreatedDate { get; set; }
  public DateTime? UpdatedDate  { get; set; }
}
