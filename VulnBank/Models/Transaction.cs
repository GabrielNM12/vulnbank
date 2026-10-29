namespace VulnBank.Models;

public class Transaction
{
    public int Id { get; set; }
    public int FromId { get; set; }
    public int ToId { get; set; }
    public double Amount { get; set; }
    public string Description { get; set; }
    public string CreatedAt { get; set; }

    // Campos preenchidos para exibição (nomes/contas das pontas).
    public string FromName { get; set; }
    public string FromAccount { get; set; }
    public string ToName { get; set; }
    public string ToAccount { get; set; }
}
