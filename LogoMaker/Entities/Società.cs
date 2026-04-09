
namespace LogoMaker.Entities;

public class Società
{
    public required string RagioneSociale{ get; set; }
    [Key]
    public required string PartitaIVA{ get; set; }
    public required string Logo{ get; set; }
    public Utente? User{ get; set; }
}