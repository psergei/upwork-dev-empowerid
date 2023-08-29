namespace Gateway.Models;

public sealed class PostCommentInput
{
  public required Guid PostId  { get; set; }
  public required string Content  { get; set; }
}