using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LogoMaker.DTO;

public class CreateUserDTO
{
    [Required(ErrorMessage = "Username is required")]
    [RegularExpression("^[A-Za-z]+$", ErrorMessage = "Name must only contain letters")]
    [DefaultValue("")]
    public string Username { get; set; } = string.Empty;
    [Required(ErrorMessage = "Password is required")]
    [RegularExpression("^(?=.*[A-Z])(?=.*[a-z])(?=.*[^A-Za-z0-9]).{8,}$", ErrorMessage = "Password must contain at least 8 characters, one uppercase, one lowercase and one special character")]
    [DefaultValue("")]
    public string Password { get; set; } = string.Empty;
    [Required(ErrorMessage = "Gender is required")]
    [RegularExpression("^[MFXmfx]$", ErrorMessage = "Gender must be M, F or X")]
    [DefaultValue("")]
    public string Gender { get; set; } = string.Empty;
    [Required(ErrorMessage = "Role is required")]
    [RegularExpression("^(?i)(user|administrator)$", ErrorMessage = "Role must either be USER or ADMINISTRATOR")]
    [DefaultValue("USER")]
    public string Role { get; set; } = string.Empty;
    [DefaultValue("[]")]
    public List<CreateCompanyDTO>? SocietàUtente { get; set; } = [];
}
