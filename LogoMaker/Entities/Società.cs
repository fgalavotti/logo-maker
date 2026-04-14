
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

    [Column("Logo")]
    [Required]
    public required string Logo{ get; set; }

    [Column("Utente")]
    public string? UsernameUtente { get; set; }

    [ForeignKey("UsernameUtente")]
    public Utente? User{ get; set; }
}