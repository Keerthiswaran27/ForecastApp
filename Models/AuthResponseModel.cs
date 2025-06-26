namespace SigninService.Models;

public class AuthResponseModel
{
    public string? access_token { get; set; }
    public string? refresh_token { get; set; }
    public string? token_type { get; set; }
    public int? expires_in { get; set; }
    public User? user { get; set; }
}

public class User
{
    public string? id { get; set; }
    public string? email { get; set; }
    // Add more if needed
}

