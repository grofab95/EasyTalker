namespace EasyTalker.Api.Requests;

public class ChangePasswordResponse
{
    public string[] Errors { get; }
    public bool IsSuccess => Errors == null || Errors.Length == 0;

    public ChangePasswordResponse(string[] errors)
    {
        Errors = errors;
    }
}