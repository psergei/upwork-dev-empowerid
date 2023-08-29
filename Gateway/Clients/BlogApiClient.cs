using System.Runtime.Versioning;
using System.Text.Json;
using CommentsContracts;
using Gateway.Models;
using Microsoft.Extensions.Caching.Distributed;
using PostsContracts;

namespace Gateway.Clients;

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

  public async Task<BlogPost?> GetPostById(Guid id)
  {
    var token = $"{PostContentToken}-{id}";
    var resultContent = await _cache.GetStringAsync(token);

    if (resultContent == null)
    {
      var response = await _http.GetAsync($"{_postsApiUrl}/{id}");
      response.EnsureSuccessStatusCode();

      resultContent = await response.Content.ReadAsStringAsync();

      await _cache.SetStringAsync(token, resultContent);
    }

    BlogPost? res = JsonSerializer.Deserialize<BlogPost>(resultContent,
      new JsonSerializerOptions(JsonSerializerDefaults.Web));

    return res;
  }

  public async Task CreatePost(BlogPost post)
  {
    var response = await _http.PostAsJsonAsync(_postsApiUrl, post, new JsonSerializerOptions(JsonSerializerDefaults.Web));
    response.EnsureSuccessStatusCode();

    await _cache.RemoveAsync(AllPostsToken);
  }

  public async Task UpdatePost(Guid id, BlogPost post)
  {
    var response = await _http.PutAsJsonAsync($"{_postsApiUrl}/{id}", post, new JsonSerializerOptions(JsonSerializerDefaults.Web));
    response.EnsureSuccessStatusCode();

    var token = $"{PostContentToken}-{id}";
    await _cache.RemoveAsync(AllPostsToken);
    await _cache.RemoveAsync(token);
  }

  public async Task DeletePost(Guid id)
  {
    var response = await _http.DeleteAsync($"{_postsApiUrl}/{id}");
    response.EnsureSuccessStatusCode();

    var token = $"{PostContentToken}-{id}";
    await _cache.RemoveAsync(AllPostsToken);
    await _cache.RemoveAsync(token);
  }

  public async Task CreateComment(PostComment comment)
  {
    var response = await _http.PostAsJsonAsync(_commentsApiUrl, comment, new JsonSerializerOptions(JsonSerializerDefaults.Web));
    response.EnsureSuccessStatusCode();

    var token = $"{PostCommentsToken}-{comment.PostId}";
    await _cache.RemoveAsync(token);
  }

  public async Task<IEnumerable<PostComment>?> GetPostComments(Guid postId)
  {
    var token = $"{PostCommentsToken}-{postId}";
    var resultContent = await _cache.GetStringAsync(token);

    if (resultContent == null)
    {
      var response = await _http.GetAsync($"{_commentsApiUrl}/{postId}");
      response.EnsureSuccessStatusCode();

      resultContent = await response.Content.ReadAsStringAsync();

      await _cache.SetStringAsync(token, resultContent);
    }

    IEnumerable<PostComment>? res = JsonSerializer.Deserialize<IEnumerable<PostComment>>(resultContent,
      new JsonSerializerOptions(JsonSerializerDefaults.Web));

    return res;
  }
}
