using Microsoft.AspNetCore.Mvc;
using VulnBank.Data;

namespace VulnBank.Controllers;

public class ApiController : Controller
{
    [HttpGet]
    public IActionResult Profile(int id)
    {
        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText =
            "SELECT Id, Username, Password, FullName, AccountNumber, Balance, IsAdmin FROM Users WHERE Id = $id;";
        cmd.Parameters.AddWithValue("$id", id);
        using var r = cmd.ExecuteReader();
        if (!r.Read()) return NotFound();

        return Json(new
        {
            id = r.GetInt32(0),
            username = r.GetString(1),
            password = r.GetString(2),
            fullName = r.GetString(3),
            accountNumber = r.GetString(4),
            balance = r.GetDouble(5),
            isAdmin = r.GetInt32(6) == 1
        });
    }
}
