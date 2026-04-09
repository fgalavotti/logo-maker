
namespace LogoMaker.Entities;

public class Società
{
    public string RagioneSociale{ get; set; }
    public string PartitaIVA{ get; set; }
    public string Logo{ get; set; }
    public Utente? User{ get; set; }
}