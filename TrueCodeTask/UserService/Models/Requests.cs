namespace UserService.Models
{
    public record RegisterRequest(string Name, string Password);
    public record LoginRequest(string Name, string Password);
}
