using Microsoft.AspNetCore.Http.HttpResults;
using PostsContracts;
using PostsService.Repositories;
using PostsService.Services;

namespace PostsService.Controllers;

/// <summary>
/// Minimal-api-style controller
/// </summary>
public static class PostsController
{
  /// <summary>
  /// Here we define all possible routes for the API
  /// </summary>
  /// <param name="routes"></param>
  public static void MapPostsControllerEndpoints(this IEndpointRouteBuilder routes)
  {
    var group = routes.MapGroup("/api/posts");

    group.MapGet("/", GetAllPosts)
      .WithName("GetAllPosts");

    group.MapGet("/{id}", GetPostById)
      .WithName("GetPostById");

    group.MapPost("/", CreatePost)
      .WithName("CreatePost");

    group.MapPut("/{id}", UpdatePost)
      .WithName("UpdatePost");

    group.MapDelete("/{id}", DeletePost)
      .WithName("DeletePost");
  }

  public async static Task<Ok<IEnumerable<BlogPost>>> GetAllPosts(IPostsRepository repo)
  {
    return TypedResults.Ok(await repo.GetAllPostsAsync());
  }

  public async static Task<Results<Ok<BlogPost>, NotFound>> GetPostById(Guid id,
    IPostsRepository repo)
  {
    var post = await repo.GetPostByIdAsync(id);

    return post != null ? TypedResults.Ok(post) : TypedResults.NotFound();
  }

  public async static Task<Ok> CreatePost(BlogPost post,
    IPostsRepository repo, IServiceBus sbus)
  {
    repo.AddPost(post);
    await repo.SaveChangesAsync();

    // Notify somebody that a post was created, could result in e.g. WebSocket message to the UI
    // Or a chance for moderator to approve/reject the post, in theory
    await sbus.SendPostAsync(post);

    return TypedResults.Ok();
  }

  public async static Task<Results<Ok, NotFound>> UpdatePost(Guid id, BlogPost post, 
    IPostsRepository repo, IServiceBus sbus)
  {
    var res = await repo.GetPostByIdAsync(id);

    if (res != null && res.Id == post.Id) 
    {
      repo.UpdatePost(post);
      await repo.SaveChangesAsync();

      // Notify somebody that a post was updated, could result in e.g. WebSocket message to the UI
      await sbus.SendPostAsync(post);

      return TypedResults.Ok();
    }

    return TypedResults.NotFound();
  }

  public async static Task<Results<Ok, NotFound>> DeletePost(Guid id,
    IPostsRepository repo, IServiceBus sbus)
  {
    var post = await repo.GetPostByIdAsync(id);

    if (post != null) 
    {
      // Notify somebody that we deleting the post, perform additional cleanup maybe (yes!) somewhere else.
      // E.g. delete or archive comments
      await sbus.DeletePostAsync(id);

      repo.DeletePost(post);
      await repo.SaveChangesAsync();

      return TypedResults.Ok();
    }

    return TypedResults.NotFound();
  }
}