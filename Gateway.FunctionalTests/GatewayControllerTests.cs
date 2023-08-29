using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Gateway.Models;

namespace Gateway.FunctionalTests;

[TestClass]
public class GatewayControllerTests
{
  [TestMethod]
  public async Task GetAllPosts_ShouldReturn_200()
  {
    var client = TestClient.GetTestClient();
    var response = await client.GetAsync("api/posts");

    Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
  }

  [TestMethod]
  public async Task CreatePost_ShouldReturn_200()
  {
    var client = TestClient.GetTestClient();
    var response = await client.PostAsJsonAsync("api/posts", new BlogPostInput()
    {
      Content = "test content",
      Title = "test title"
    });

    Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
  }

  [TestMethod]
  public async Task GetPostById_ShouldReturn_404()
  {
    var client = TestClient.GetTestClient();
    var response = await client.GetAsync($"api/posts/{Guid.Empty}");

    Assert.IsTrue(response.StatusCode == HttpStatusCode.NotFound);
  }

  [TestMethod]
  public async Task GetPostById_ShouldReturn_200()
  {
    var testTitle = Guid.NewGuid().ToString();
    var client = TestClient.GetTestClient();
    await client.PostAsJsonAsync("api/posts", new BlogPostInput()
    {
      Content = "test content",
      Title = testTitle
    });

    var posts = await GetIntegrationPosts();
    Assert.IsNotNull(posts);
    Assert.IsTrue(posts?.Any());

    var lastPost = posts?.First();
    Assert.AreEqual(testTitle, lastPost?.Title);

    var response = await client.GetAsync($"api/posts/{lastPost?.Id}");

    Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
  }

  [TestMethod]
  public async Task UpdatePost_ShouldReturn_404()
  {
    var client = TestClient.GetTestClient();
    var response = await client.PutAsJsonAsync($"api/posts/{Guid.Empty}", new BlogPostInput()
    {
      Content = "",
      Title = ""
    });

    Assert.IsTrue(response.StatusCode == HttpStatusCode.NotFound);
  }

  [TestMethod]
  public async Task DeletePost_ShouldReturn_404()
  {
    var client = TestClient.GetTestClient();
    var response = await client.DeleteAsync($"api/posts/{Guid.Empty}");

    Assert.IsTrue(response.StatusCode == HttpStatusCode.NotFound);
  }

  [TestMethod]
  public async Task CreateComment_ShouldReturn_404()
  {
    var client = TestClient.GetTestClient();
    var response = await client.PostAsJsonAsync("api/comments", new PostCommentInput()
    {
      PostId = Guid.Empty,
      Content = ""
    });

    Assert.IsTrue(response.StatusCode == HttpStatusCode.NotFound);
  }

  private static async Task<IEnumerable<BlogPostView>?> GetIntegrationPosts()
  {
    var client = TestClient.GetTestClient();
    var response = await client.GetAsync("api/posts");
    var content = await response.Content.ReadAsStringAsync();
   
    var posts = JsonSerializer.Deserialize<IEnumerable<BlogPostView>>(content,
      new JsonSerializerOptions(JsonSerializerDefaults.Web));

    return posts?
      .Where(p => p.Author == "integration")
      .OrderByDescending(p => p.CreatedDate);
  }
}