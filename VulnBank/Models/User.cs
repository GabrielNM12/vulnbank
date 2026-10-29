namespace VulnBank.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string FullName { get; set; }
    public string AccountNumber { get; set; }
    public double Balance { get; set; }
    public bool IsAdmin { get; set; }
}
