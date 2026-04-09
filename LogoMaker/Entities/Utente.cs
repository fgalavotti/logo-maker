
namespace LogoMaker.Entities;

public class Utente
{
    public string Username{ get; set; }
    public string Password{ get; set; }
    public string Gender{ get; set; }
    public string Role{ get; set; }
    public List<Società>? SocietàUtente{ get; set; }
}