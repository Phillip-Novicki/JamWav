namespace JamWav.Web.Models;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expires { get; set; }
}