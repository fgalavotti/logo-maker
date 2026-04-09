
namespace LogoMaker.Entities;

public class Utente
{
    [Key]
    public required string Username{ get; set; }
    public required string Password{ get; set; }
    public required string Gender{ get; set; }
    public required string Role{ get; set; }
    public List<Società>? SocietàUtente{ get; set; }
}