namespace CommentsContracts;

public sealed class PostComment
{
  public required Guid Id { get; set; }
  public required Guid PostId  { get; set; }
  public required string Content  { get; set; }

  public required string Author  { get; set; }
  public required DateTime CreatedDate  { get; set; }
}
