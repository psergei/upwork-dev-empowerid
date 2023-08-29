using System.Security.Claims;

namespace Gateway.Services;

/// <summary>
/// Class used to get the auth user name from the http context principal
/// </summary>
public class BlogUserService: IBlogUserService
{
  private readonly IHttpContextAccessor _httpCtxAccessor;

  public BlogUserService(IHttpContextAccessor ctxAccessor)
  {
    _httpCtxAccessor = ctxAccessor;
  }

  public string UserName
  {
    get
    {
      var user = _httpCtxAccessor.HttpContext?.User;

      return user?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    }
  }
}