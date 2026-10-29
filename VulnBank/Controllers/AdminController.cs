using Microsoft.AspNetCore.Mvc;
using VulnBank.Data;
using VulnBank.Models;

namespace VulnBank.Controllers;

public class AdminController : Controller
{
    public IActionResult Index()
    {
        var users = new List<User>();
        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText =
            "SELECT Id, Username, FullName, AccountNumber, Balance, IsAdmin FROM Users ORDER BY Id;";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            users.Add(new User
            {
                Id = reader.GetInt32(0),
                Username = reader.GetString(1),
                FullName = reader.GetString(2),
                AccountNumber = reader.GetString(3),
                Balance = reader.GetDouble(4),
                IsAdmin = reader.GetInt32(5) == 1
            });
        }
        return View(users);
    }
}
