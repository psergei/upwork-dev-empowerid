using Microsoft.AspNetCore.Http.HttpResults;
using CommentsContracts;
using CommentsService.Repositories;

namespace CommentsService.Controllers;

public static class CommentsController
{
  // Here we define all possible routes for the API
  public static void MapCommentsControllerEndpoints(this IEndpointRouteBuilder routes)
  {
    var group = routes.MapGroup("/api/comments");

    group.MapGet("/{id}", GetPostComments)
      .WithName("GetPostComments");

    group.MapPost("/", CreateComment)
      .WithName("CreateComment");
  }

  public async static Task<Ok<IEnumerable<PostComment>>> GetPostComments(Guid id,
    ICommentsRepository repo)
  {
    return TypedResults.Ok(await repo.GetPostComments(id));
  }

  public async static Task<Ok> CreateComment(PostComment comment,
    ICommentsRepository repo)
  {
    repo.AddComment(comment);
    await repo.SaveChangesAsync();

    return TypedResults.Ok();
  }
}