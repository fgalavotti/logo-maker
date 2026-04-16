using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
namespace LogoMaker.Entities;

[Table("roletable")]
public class Ruolo : IdentityRole
{

    // public int Id { get; set; }
    // public string Nome { get; set; } = null!;

    public virtual ICollection<Utente> ListaUtenti { get; set; } = new List<Utente>{};
}