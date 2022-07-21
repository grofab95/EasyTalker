namespace EasyTalker.Authentication.Configuration;

public class AuthenticationOption
{
    public const string SectionKey = "Authentication";

    public PasswordOption Password { get; set; }
}