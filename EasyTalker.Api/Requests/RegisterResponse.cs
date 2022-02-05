using EasyTalker.Core.Dto.User;

namespace EasyTalker.Api.Requests;

public class RegisterResponse
{
    public UserDto User { get; }
    public string Error { get; }
    public string[] ValidationErrors { get; }

    public RegisterResponse(UserDto user)
    {
        User = user;
    }

    public RegisterResponse(string[] errors)
    {
        ValidationErrors = errors;
    }
    
    public RegisterResponse(string error)
    {
        Error = error;
    }
}