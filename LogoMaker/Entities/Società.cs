
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
namespace LogoMaker.Entities;

[Table("companytable")]
public class Società
{
    [Key]
    [Column("PartitaIVA")]
    [Required]
    public required string PartitaIVA { get; set; }

    [Column("RagioneSociale")]
    [Required]
    public required string RagioneSociale{ get; set; }

    [Column("Utente")]
    public string? UsernameUtente { get; set; }

    [ForeignKey("UsernameUtente")]
    public Utente? User{ get; set; }
    
    [Column("Logo", TypeName = "nvarchar(max)")]
    public string LogoJson { get; set; } = string.Empty;

    [NotMapped]
    public List<string> Logo
    {
        get => JsonSerializer.Deserialize<List<string>>(LogoJson)
               ?? new List<string>();
        set => LogoJson = JsonSerializer.Serialize(value);
    }

}