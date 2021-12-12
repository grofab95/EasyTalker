using System.ComponentModel.DataAnnotations;

namespace EasyTalker.Api.Requests;

public class LoginRequest
{
    [Required(ErrorMessage = "User name is required")]
    public string Username { get; set; }
        
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }
}