using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using VulnBank.Data;
using VulnBank.Models;

namespace VulnBank.Controllers;

public class AccountController : Controller
{
    private int? Uid => HttpContext.Session.GetInt32("uid");

    public IActionResult Index()
    {
        if (Uid == null) return RedirectToAction("Login", "Home");

        using var conn = Db.Open();
        var user = LoadUser(conn, Uid.Value);
        if (user == null) return RedirectToAction("Login", "Home");

        var map = LoadUserMap(conn);
        var txs = LoadTransactionsFor(conn, Uid.Value, map, limit: 5);

        ViewBag.Transactions = txs;
        return View(user);
    }

    public IActionResult Statement()
    {
        if (Uid == null) return RedirectToAction("Login", "Home");

        using var conn = Db.Open();
        var user = LoadUser(conn, Uid.Value);
        if (user == null) return RedirectToAction("Login", "Home");

        var map = LoadUserMap(conn);
        var txs = LoadTransactionsFor(conn, Uid.Value, map, limit: 0);

        ViewBag.Account = user.AccountNumber;
        return View(txs);
    }

    [HttpGet]
    public IActionResult Transfer()
    {
        if (Uid == null) return RedirectToAction("Login", "Home");
        using var conn = Db.Open();
        var user = LoadUser(conn, Uid.Value);
        ViewBag.Balance = user?.Balance ?? 0;
        return View();
    }

    [HttpPost]
    public IActionResult Transfer(string toAccount, double amount, string description)
    {
        if (Uid == null) return RedirectToAction("Login", "Home");

        using var conn = Db.Open();
        var source = LoadUser(conn, Uid.Value);
        if (source == null) return RedirectToAction("Login", "Home");

        int? toId = null;
        using (var find = conn.CreateCommand())
        {
            find.CommandText = "SELECT Id FROM Users WHERE AccountNumber = $acc;";
            find.Parameters.AddWithValue("$acc", toAccount ?? "");
            using var r = find.ExecuteReader();
            if (r.Read()) toId = r.GetInt32(0);
        }

        if (toId == null)
        {
            ViewBag.Balance = source.Balance;
            ViewBag.Error = "Conta de destino não encontrada.";
            return View();
        }

        using (var debit = conn.CreateCommand())
        {
            debit.CommandText = "UPDATE Users SET Balance = Balance - $amt WHERE Id = $id;";
            debit.Parameters.AddWithValue("$amt", amount);
            debit.Parameters.AddWithValue("$id", Uid.Value);
            debit.ExecuteNonQuery();
        }
        using (var credit = conn.CreateCommand())
        {
            credit.CommandText = "UPDATE Users SET Balance = Balance + $amt WHERE Id = $id;";
            credit.Parameters.AddWithValue("$amt", amount);
            credit.Parameters.AddWithValue("$id", toId.Value);
            credit.ExecuteNonQuery();
        }

        using (var log = conn.CreateCommand())
        {
            log.CommandText =
                "INSERT INTO Transactions (FromId, ToId, Amount, Description, CreatedAt) " +
                "VALUES ($f, $t, $a, $d, $c);";
            log.Parameters.AddWithValue("$f", Uid.Value);
            log.Parameters.AddWithValue("$t", toId.Value);
            log.Parameters.AddWithValue("$a", amount);
            log.Parameters.AddWithValue("$d", (object)description ?? DBNull.Value);
            log.Parameters.AddWithValue("$c", DateTime.UtcNow.ToString("o"));
            log.ExecuteNonQuery();
        }

        long newId;
        using (var idCmd = conn.CreateCommand())
        {
            idCmd.CommandText = "SELECT last_insert_rowid();";
            newId = Convert.ToInt64(idCmd.ExecuteScalar());
        }

        return RedirectToAction("Receipt", new { id = newId });
    }

    public IActionResult Receipt(int id)
    {
        if (Uid == null) return RedirectToAction("Login", "Home");

        using var conn = Db.Open();
        Transaction tx = null;
        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText =
                "SELECT Id, FromId, ToId, Amount, Description, CreatedAt FROM Transactions WHERE Id = $id;";
            cmd.Parameters.AddWithValue("$id", id);
            using var r = cmd.ExecuteReader();
            if (r.Read())
            {
                tx = new Transaction
                {
                    Id = r.GetInt32(0),
                    FromId = r.GetInt32(1),
                    ToId = r.GetInt32(2),
                    Amount = r.GetDouble(3),
                    Description = r.IsDBNull(4) ? "" : r.GetString(4),
                    CreatedAt = r.GetString(5)
                };
            }
        }
        if (tx == null) return NotFound();

        var map = LoadUserMap(conn);
        if (map.TryGetValue(tx.FromId, out var f)) { tx.FromName = f.name; tx.FromAccount = f.acc; }
        if (map.TryGetValue(tx.ToId, out var t)) { tx.ToName = t.name; tx.ToAccount = t.acc; }

        return View(tx);
    }

    // ---------- helpers ----------

    private static User LoadUser(SqliteConnection conn, int id)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText =
            "SELECT Id, Username, FullName, AccountNumber, Balance FROM Users WHERE Id = $id;";
        cmd.Parameters.AddWithValue("$id", id);
        using var r = cmd.ExecuteReader();
        if (!r.Read()) return null;
        return new User
        {
            Id = r.GetInt32(0),
            Username = r.GetString(1),
            FullName = r.GetString(2),
            AccountNumber = r.GetString(3),
            Balance = r.GetDouble(4)
        };
    }

    private static Dictionary<int, (string name, string acc)> LoadUserMap(SqliteConnection conn)
    {
        var map = new Dictionary<int, (string, string)>();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Id, FullName, AccountNumber FROM Users;";
        using var r = cmd.ExecuteReader();
        while (r.Read())
            map[r.GetInt32(0)] = (r.GetString(1), r.GetString(2));
        return map;
    }

    private static List<Transaction> LoadTransactionsFor(
        SqliteConnection conn, int uid, Dictionary<int, (string name, string acc)> map, int limit)
    {
        var list = new List<Transaction>();
        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText =
                "SELECT Id, FromId, ToId, Amount, Description, CreatedAt FROM Transactions " +
                "WHERE FromId = $id OR ToId = $id ORDER BY Id DESC" + (limit > 0 ? " LIMIT $lim" : "") + ";";
            cmd.Parameters.AddWithValue("$id", uid);
            if (limit > 0) cmd.Parameters.AddWithValue("$lim", limit);
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new Transaction
                {
                    Id = r.GetInt32(0),
                    FromId = r.GetInt32(1),
                    ToId = r.GetInt32(2),
                    Amount = r.GetDouble(3),
                    Description = r.IsDBNull(4) ? "" : r.GetString(4),
                    CreatedAt = r.GetString(5)
                });
            }
        }
        foreach (var t in list)
        {
            if (map.TryGetValue(t.FromId, out var f)) { t.FromName = f.name; t.FromAccount = f.acc; }
            if (map.TryGetValue(t.ToId, out var to)) { t.ToName = to.name; t.ToAccount = to.acc; }
        }
        return list;
    }
}
