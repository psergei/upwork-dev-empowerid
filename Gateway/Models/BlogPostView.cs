namespace Gateway.Models;

public sealed class BlogPostView
{
  public required Guid Id { get; set; }
  public required string Title  { get; set; }
  public required string Content  { get; set; }

  public required string Author  { get; set; }
  public required DateTime CreatedDate { get; set; }
  public DateTime? UpdatedDate  { get; set; }

  public IEnumerable<PostCommentView>? Comments  { get; set; }
}