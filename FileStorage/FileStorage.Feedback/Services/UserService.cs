namespace FileStorage.Feedback.Services;
public interface IUserService
{
    string? GetUserId();
}



public class UserService : IUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetUserId()
    {
        var result = string.Empty;
        if (_httpContextAccessor.HttpContext is not null)
        {
            result = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "id")?.Value;
        }
        return result;
    }
}
