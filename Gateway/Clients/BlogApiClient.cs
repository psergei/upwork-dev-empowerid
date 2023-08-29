using System.Net;
using System.Runtime.Versioning;
using System.Text.Json;
using CommentsContracts;
using Gateway.Models;
using Microsoft.Extensions.Caching.Distributed;
using PostsContracts;

namespace Gateway.Clients;

/// <summary>
/// Define API client for Posts/Comments microservices
/// Supports Redis cache
/// </summary>
public class BlogApiClient : IBlogApiClient
{
  const string AllPostsToken = "AllPosts";
  const string PostContentToken = "PostContent";
  const string PostCommentsToken = "PostComments";

  private readonly HttpClient _http;
  private readonly IDistributedCache _cache;
  private readonly string _postsApiUrl;
  private readonly string _commentsApiUrl;

  public BlogApiClient(IConfiguration config, HttpClient http, IDistributedCache cache)
  {
    _http = http;
    _cache = cache;

    _postsApiUrl = config["PostsApi"] ?? string.Empty;
    _commentsApiUrl = config["CommentsApi"] ?? string.Empty;
  }

  /// <summary>
  /// Returns all posts from the Posts service or from the cache if available
  /// </summary>
  /// <returns></returns>
  public async Task<IEnumerable<BlogPost>?> GetAllPosts()
  {
    var resultContent = await _cache.GetStringAsync(AllPostsToken);
    if (resultContent == null)
    {
      var response = await _http.GetAsync(_postsApiUrl);
      response.EnsureSuccessStatusCode();

      resultContent = await response.Content.ReadAsStringAsync();

      await _cache.SetStringAsync(AllPostsToken, resultContent);
    }

    IEnumerable<BlogPost>? res = JsonSerializer.Deserialize<IEnumerable<BlogPost>>(resultContent,
      new JsonSerializerOptions(JsonSerializerDefaults.Web));

    return res;
  }

  /// <summary>
  /// Returns specific post from the service or from the cache if available
  /// </summary>
  /// <param name="id"></param>
  /// <returns></returns>
  public async Task<BlogPost?> GetPostById(Guid id)
  {
    var token = $"{PostContentToken}-{id}";
    var resultContent = await _cache.GetStringAsync(token);

    if (resultContent == null)
    {
      var response = await _http.GetAsync($"{_postsApiUrl}/{id}");
      if (response.StatusCode == HttpStatusCode.NotFound)
      {
        return null;
      }
      response.EnsureSuccessStatusCode();

      resultContent = await response.Content.ReadAsStringAsync();

      await _cache.SetStringAsync(token, resultContent);
    }

    BlogPost? res = JsonSerializer.Deserialize<BlogPost>(resultContent,
      new JsonSerializerOptions(JsonSerializerDefaults.Web));

    return res;
  }

  /// <summary>
  /// Create a new post, prune the cache
  /// </summary>
  /// <param name="post"></param>
  /// <returns></returns>
  public async Task CreatePost(BlogPost post)
  {
    var response = await _http.PostAsJsonAsync(_postsApiUrl, post, new JsonSerializerOptions(JsonSerializerDefaults.Web));
    response.EnsureSuccessStatusCode();

    await _cache.RemoveAsync(AllPostsToken);
  }

  /// <summary>
  /// Update the post, prune the cache
  /// </summary>
  /// <param name="id"></param>
  /// <param name="post"></param>
  /// <returns></returns>
  public async Task<bool> UpdatePost(Guid id, BlogPost post)
  {
    var response = await _http.PutAsJsonAsync($"{_postsApiUrl}/{id}", post, new JsonSerializerOptions(JsonSerializerDefaults.Web));
    if (response.StatusCode == HttpStatusCode.NotFound)
    {
      return false;
    }
    response.EnsureSuccessStatusCode();

    var token = $"{PostContentToken}-{id}";
    await _cache.RemoveAsync(AllPostsToken);
    await _cache.RemoveAsync(token);

    return true;
  }

  /// <summary>
  /// Delete existing post, prune the cache
  /// </summary>
  /// <param name="id"></param>
  /// <returns></returns>
  public async Task<bool> DeletePost(Guid id)
  {
    var response = await _http.DeleteAsync($"{_postsApiUrl}/{id}");
    if (response.StatusCode == HttpStatusCode.NotFound)
    {
      return false;
    }
    response.EnsureSuccessStatusCode();

    var token = $"{PostContentToken}-{id}";
    await _cache.RemoveAsync(AllPostsToken);
    await _cache.RemoveAsync(token);

    return true;
  }

  /// <summary>
  /// Adds a new comment to the post
  /// </summary>
  /// <param name="comment"></param>
  /// <returns></returns>
  public async Task<bool> CreateComment(PostComment comment)
  {
    var response = await _http.PostAsJsonAsync(_commentsApiUrl, comment, new JsonSerializerOptions(JsonSerializerDefaults.Web));
    if (response.StatusCode == HttpStatusCode.NotFound)
    {
      return false;
    }
    response.EnsureSuccessStatusCode();

    var token = $"{PostCommentsToken}-{comment.PostId}";
    await _cache.RemoveAsync(token);

    return true;
  }

  /// <summary>
  /// Get post comments from the server or from the cache if available
  /// </summary>
  /// <param name="postId"></param>
  /// <returns></returns>
  public async Task<IEnumerable<PostComment>?> GetPostComments(Guid postId)
  {
    var token = $"{PostCommentsToken}-{postId}";
    var resultContent = await _cache.GetStringAsync(token);

    if (resultContent == null)
    {
      var response = await _http.GetAsync($"{_commentsApiUrl}/{postId}");
      if (response.StatusCode == HttpStatusCode.NotFound)
      {
        return null;
      }
      response.EnsureSuccessStatusCode();

      resultContent = await response.Content.ReadAsStringAsync();

      await _cache.SetStringAsync(token, resultContent);
    }

    IEnumerable<PostComment>? res = JsonSerializer.Deserialize<IEnumerable<PostComment>>(resultContent,
      new JsonSerializerOptions(JsonSerializerDefaults.Web));

    return res;
  }
}
