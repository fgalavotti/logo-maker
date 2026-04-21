using LogoMaker.DataAccess;
using LogoMaker.DTO;
using LogoMaker.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LogoMaker.Controllers;

[ApiController]
[Route("api")]
public class RegisterLoginController : ControllerBase
{
    private readonly LMContext _context;
    private readonly IConfiguration _config;
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100_000;

    public RegisterLoginController(LMContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDTO dto)
    {
        var user = await _context.Utenti
            .FirstOrDefaultAsync(u => u.Username == dto.Username);

        if (user == null)
            return Unauthorized("Username non valido");

        if (!VerifyPassword(user.Password, dto.Password))
            return Unauthorized("Password non valida");

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
            Password = HashPassword(dto.Password),
            Gender = dto.Gender,
            Role = "USER"
        };

        _context.Utenti.Add(user);
        await _context.SaveChangesAsync();

        return Ok("Registrazione completata");
    }


    public static string HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize
        );

        byte[] combined = new byte[SaltSize + HashSize];
        Buffer.BlockCopy(salt, 0, combined, 0, SaltSize);
        Buffer.BlockCopy(hash, 0, combined, SaltSize, HashSize);

        return Convert.ToBase64String(combined);
    }

    static bool VerifyPassword(string stored, string passwordInserita)
    {
        byte[] combinedBytes = Convert.FromBase64String(stored);
        byte[] salt = new byte[SaltSize];
        byte[] originalHash = new byte[HashSize];

        Buffer.BlockCopy(combinedBytes, 0, salt, 0, SaltSize);
        Buffer.BlockCopy(combinedBytes, SaltSize, originalHash, 0, HashSize);

        byte[] newHash = Rfc2898DeriveBytes.Pbkdf2(
            passwordInserita,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize
        );

        return CryptographicOperations.FixedTimeEquals(newHash, originalHash);
    }

}
