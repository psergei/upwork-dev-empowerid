using Microsoft.AspNetCore.Http.HttpResults;
using PostsContracts;
using CommentsContracts;
using Gateway.Models;
using Gateway.Clients;
using AutoMapper;
using Gateway.Services;

namespace Gateway.Controllers;

/// <summary>
/// Public API facade controller
/// </summary>
public static class GatewayController
{
  private readonly static Mapper _mapper;

  static GatewayController()
  {
    // Define automatic mappers for DTO, input/output models
    _mapper = new Mapper(new MapperConfiguration(cfg => 
    {
      cfg.CreateMap<BlogPost, BlogPostView>();
      cfg.CreateMap<PostComment, PostCommentView>();
      cfg.CreateMap<BlogPostInput, BlogPost>();
      cfg.CreateMap<PostCommentInput, PostComment>();
    }));
  }

  /// <summary>
  /// Here we define all possible routes for the API
  /// </summary>
  /// <param name="routes"></param>
  /// <param name="readDataPolicy"></param>
  /// <param name="writeDataPolicy"></param>
  public static void MapGatewayControllerEndpoints(this IEndpointRouteBuilder routes, 
    string readDataPolicy, string writeDataPolicy)
  {
    ConfigureRoutes(routes, readDataPolicy, writeDataPolicy);
  }

  public async static Task<Ok<IEnumerable<BlogPostView>>> GetAllPosts(IBlogApiClient api)
  {
    var posts = await api.GetAllPosts();
    var views = _mapper.Map<IEnumerable<BlogPostView>>(posts);

    return TypedResults.Ok(views);
  }

  public async static Task<Results<Ok<BlogPostView>, NotFound>> GetPostById(Guid id,
    IBlogApiClient api)
  {
    var post = await api.GetPostById(id);
    if(post == null) 
    {
      return TypedResults.NotFound();
    }

    var view = _mapper.Map<BlogPostView>(post);

    var comments = await api.GetPostComments(id);
    view.Comments = _mapper.Map<IEnumerable<PostCommentView>>(comments);

    return TypedResults.Ok(view);
  }

  public async static Task<Ok> CreatePost(BlogPostInput inputPost,
    IBlogApiClient api, IBlogUserService user)
  {
    var post = _mapper.Map<BlogPost>(inputPost);
    post.Id = Guid.NewGuid();
    post.CreatedDate = DateTime.UtcNow;
    post.Author = user.UserName;

    await api.CreatePost(post);

    return TypedResults.Ok();
  }

  public async static Task<Results<Ok, NotFound>> UpdatePost(Guid id, BlogPostInput inputPost,
    IBlogApiClient api)
  {
    var post = await api.GetPostById(id);
    if (post == null)
    {
      return TypedResults.NotFound();
    }

    post.Title = inputPost.Title;
    post.Content = inputPost.Content;
    post.UpdatedDate = DateTime.UtcNow;

    await api.UpdatePost(id, post);

    return TypedResults.Ok();
  }

  public async static Task<Results<Ok, NotFound>> DeletePost(Guid id,
    IBlogApiClient api)
  {
    var post = await api.GetPostById(id);
    if (post == null)
    {
      return TypedResults.NotFound();
    }

    await api.DeletePost(id);

    return TypedResults.Ok();
  }

  public async static Task<Results<Ok, NotFound>> CreateComment(PostCommentInput inputComment,
    IBlogApiClient api, IBlogUserService user)
  {
    var post = await api.GetPostById(inputComment.PostId);
    if (post == null)
    {
      return TypedResults.NotFound();
    }

    var comment = _mapper.Map<PostComment>(inputComment);
    comment.Id = Guid.NewGuid();
    comment.CreatedDate = DateTime.UtcNow;
    comment.Author = user.UserName;

    await api.CreateComment(comment);

    return TypedResults.Ok();
  }

  /// <summary>
  /// Define routes configuration for gateway
  /// </summary>
  /// <param name="routes"></param>
  /// <param name="readDataPolicy">rate-limit policy for read operations</param>
  /// <param name="writeDataPolicy">rate0limit policy for write operations</param>
  private static void ConfigureRoutes(IEndpointRouteBuilder routes, string readDataPolicy, string writeDataPolicy)
  {
    var groupPosts = routes.MapGroup("/api/posts");
    var groupComments = routes.MapGroup("/api/comments");

    groupPosts.MapGet("/", GetAllPosts)
      .WithName("GetAllPosts")
      .RequireAuthorization()
      .RequireRateLimiting(readDataPolicy);

    groupPosts.MapGet("/{id}", GetPostById)
      .WithName("GetPostById")
      .RequireAuthorization()
      .RequireRateLimiting(readDataPolicy);

    groupPosts.MapPost("/", CreatePost)
      .WithName("CreatePost")
      .RequireAuthorization()
      .RequireRateLimiting(writeDataPolicy);

    groupPosts.MapPut("/{id}", UpdatePost)
      .WithName("UpdatePost")
      .RequireAuthorization()
      .RequireRateLimiting(writeDataPolicy);

    groupPosts.MapDelete("/{id}", DeletePost)
      .WithName("DeletePost")
      .RequireAuthorization()
      .RequireRateLimiting(writeDataPolicy);

    groupComments.MapPost("/", CreateComment)
      .WithName("CreateComment")
      .RequireAuthorization()
      .RequireRateLimiting(writeDataPolicy);
  }
}