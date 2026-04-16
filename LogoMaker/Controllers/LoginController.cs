using LogoMaker.DataAccess;
using LogoMaker.DTO;
using LogoMaker.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace LogoMaker.Controllers;

[ApiController]
[Route("api/registerlogin")]
public class RegisterLoginController : ControllerBase
{
    private readonly LMContext _context;
    private readonly IConfiguration _config;

    public RegisterLoginController(LMContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDTO dto)
    {
        var user = await _context.Utenti
            .FirstOrDefaultAsync(u =>
                u.Username == dto.Username &&
                u.Password == dto.Password);

        if (user == null)
            return Unauthorized("Credenziali non valide");

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
        );

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256)
        );

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token)
        });
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterUserDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (await _context.Utenti.AnyAsync(u => u.Username == dto.Username))
            return Conflict("Username già esistente");

        var user = new Utente
        {
            Username = dto.Username,
            Password = dto.Password,
            Gender = dto.Gender,
            Role = "USER"
        };

        _context.Utenti.Add(user);
        await _context.SaveChangesAsync();

        return Ok("Registrazione completata");
    }

}
