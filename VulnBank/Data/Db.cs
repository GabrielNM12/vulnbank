using Microsoft.Data.Sqlite;

namespace VulnBank.Data;

public static class Db
{
    public const string ConnectionString = "Data Source=nimbus.db";

    public static SqliteConnection Open()
    {
        var conn = new SqliteConnection(ConnectionString);
        conn.Open();
        return conn;
    }

    public static void Initialise()
    {
        using var conn = Open();

        using (var create = conn.CreateCommand())
        {
            create.CommandText = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id            INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username      TEXT    NOT NULL,
                    Password      TEXT    NOT NULL,
                    FullName      TEXT    NOT NULL,
                    AccountNumber TEXT    NOT NULL,
                    Balance       REAL    NOT NULL,
                    IsAdmin       INTEGER NOT NULL DEFAULT 0
                );
                CREATE TABLE IF NOT EXISTS Transactions (
                    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
                    FromId      INTEGER NOT NULL,
                    ToId        INTEGER NOT NULL,
                    Amount      REAL    NOT NULL,
                    Description TEXT,
                    CreatedAt   TEXT    NOT NULL
                );";
            create.ExecuteNonQuery();
        }

        long count;
        using (var check = conn.CreateCommand())
        {
            check.CommandText = "SELECT COUNT(*) FROM Users;";
            count = Convert.ToInt64(check.ExecuteScalar());
        }
        if (count > 0) return;

        void AddUser(string username, string password, string fullName, string account, double balance, int isAdmin)
        {
            using var ins = conn.CreateCommand();
            ins.CommandText =
                "INSERT INTO Users (Username, Password, FullName, AccountNumber, Balance, IsAdmin) " +
                "VALUES ($u, $p, $n, $acc, $b, $a);";
            ins.Parameters.AddWithValue("$u", username);
            ins.Parameters.AddWithValue("$p", password);
            ins.Parameters.AddWithValue("$n", fullName);
            ins.Parameters.AddWithValue("$acc", account);
            ins.Parameters.AddWithValue("$b", balance);
            ins.Parameters.AddWithValue("$a", isAdmin);
            ins.ExecuteNonQuery();
        }

        // Id 1, 2, 3
        AddUser("wiener", "peter",        "Peter Wiener",          "40010001", 1500.00, 0);
        AddUser("carlos", "montoya",      "Carlos Montoya",        "40010002", 8200.50, 0);
        AddUser("admin",  "N1mbus@2026!", "Nimbus Bank - Suporte", "40010000",    0.00, 1);

        void AddTx(int from, int to, double amount, string desc)
        {
            using var tx = conn.CreateCommand();
            tx.CommandText =
                "INSERT INTO Transactions (FromId, ToId, Amount, Description, CreatedAt) " +
                "VALUES ($f, $t, $a, $d, $c);";
            tx.Parameters.AddWithValue("$f", from);
            tx.Parameters.AddWithValue("$t", to);
            tx.Parameters.AddWithValue("$a", amount);
            tx.Parameters.AddWithValue("$d", desc);
            tx.Parameters.AddWithValue("$c", DateTime.UtcNow.AddDays(-3).ToString("o"));
            tx.ExecuteNonQuery();
        }

        AddTx(2, 1, 200.00, "Reembolso");
        AddTx(1, 2, 75.50, "Jantar");
        AddTx(2, 1, 1200.00, "Freelance");
    }
}
