using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LogoMaker.DTO;

public class CreateCompanyDTO
{
    [Required(ErrorMessage = "VAT is required")]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "VAT must exactly contain 11 figures")]
    [DefaultValue("")]
    public string PartitaIVA { get; set; } = string.Empty;
    [Required(ErrorMessage = "Name is required")]
    [DefaultValue("")]
    public string RagioneSociale { get; set; } = string.Empty;
    [Required(ErrorMessage = "Logo is required")]
    [DefaultValue("")]
    public string Logo { get; set; } = string.Empty;
    [RegularExpression("^[A-Za-z]+$", ErrorMessage = "Name must only contain letters")]
    [DefaultValue("")]
    public string UsernameUtente { get; set; } = string.Empty;
}