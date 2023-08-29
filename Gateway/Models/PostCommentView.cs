namespace Gateway.Models;

public sealed class PostCommentView
{
  public required Guid Id { get; set; }
  public required string Content  { get; set; }

  public required string Author  { get; set; }
  public required DateTime CreatedDate  { get; set; }
}