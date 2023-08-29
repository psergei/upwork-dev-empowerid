using System.Text.Json;
using CommentsContracts;
using Gateway.Models;
using PostsContracts;

namespace Gateway.Clients;

public class BlogApiClient : IBlogApiClient
{
  private readonly HttpClient _http;
  private readonly string _postsApiUrl;
  private readonly string _commentsApiUrl;

  public BlogApiClient(IConfiguration config, HttpClient http)
  {
    _http = http;

    _postsApiUrl = config["PostsApi"] ?? string.Empty;
    _commentsApiUrl = config["CommentsApi"] ?? string.Empty;
  }

  public async Task<IEnumerable<BlogPost>?> GetAllPosts()
  {
    var response = await _http.GetAsync(_postsApiUrl);
    response.EnsureSuccessStatusCode();

    string resultContent = await response.Content.ReadAsStringAsync();

    IEnumerable<BlogPost>? res = JsonSerializer.Deserialize<IEnumerable<BlogPost>>(resultContent,
      new JsonSerializerOptions(JsonSerializerDefaults.Web));

    return res;
  }

  public async Task<BlogPost?> GetPostById(Guid id)
  {
    var response = await _http.GetAsync($"{_postsApiUrl}/{id}");
    response.EnsureSuccessStatusCode();

    string resultContent = await response.Content.ReadAsStringAsync();

    BlogPost? res = JsonSerializer.Deserialize<BlogPost>(resultContent,
      new JsonSerializerOptions(JsonSerializerDefaults.Web));

    return res;
  }

  public async Task CreatePost(BlogPost post)
  {
    var response = await _http.PostAsJsonAsync(_postsApiUrl, post, new JsonSerializerOptions(JsonSerializerDefaults.Web));
    response.EnsureSuccessStatusCode();
  }

  public async Task UpdatePost(Guid id, BlogPost post)
  {
    var response = await _http.PutAsJsonAsync($"{_postsApiUrl}/{id}", post, new JsonSerializerOptions(JsonSerializerDefaults.Web));
    response.EnsureSuccessStatusCode();
  }

  public async Task DeletePost(Guid id)
  {
    var response = await _http.DeleteAsync($"{_postsApiUrl}/{id}");
    response.EnsureSuccessStatusCode();
  }

  public async Task CreateComment(PostComment comment)
  {
    var response = await _http.PostAsJsonAsync(_commentsApiUrl, comment, new JsonSerializerOptions(JsonSerializerDefaults.Web));
    response.EnsureSuccessStatusCode();
  }

  public async Task<IEnumerable<PostComment>?> GetPostComments(Guid postId)
  {
    var response = await _http.GetAsync($"{_commentsApiUrl}/{postId}");
    response.EnsureSuccessStatusCode();

    string resultContent = await response.Content.ReadAsStringAsync();

    IEnumerable<PostComment>? res = JsonSerializer.Deserialize<IEnumerable<PostComment>>(resultContent,
      new JsonSerializerOptions(JsonSerializerDefaults.Web));

    return res;
  }
}
