using Microsoft.AspNetCore.Mvc;
using VulnBank.Data;

namespace VulnBank.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        if (HttpContext.Session.GetInt32("uid") != null)
            return RedirectToAction("Index", "Account");
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (HttpContext.Session.GetInt32("uid") != null)
            return RedirectToAction("Index", "Account");
        return View();
    }

    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText =
            "SELECT Id, Username FROM Users " +
            "WHERE Username = '" + username + "' AND Password = '" + password + "';";

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            HttpContext.Session.SetInt32("uid", reader.GetInt32(0));
            HttpContext.Session.SetString("uname", reader.GetString(1));
            return RedirectToAction("Index", "Account");
        }

        ViewBag.Error = "Credenciais inválidas. Verifique usuário e senha.";
        return View();
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
