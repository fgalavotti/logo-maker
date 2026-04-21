using LogoMaker.DataAccess;
using LogoMaker.DTO;
using LogoMaker.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogoMaker.Controllers;

[ApiController]
[Route("api/userlist")]
public class UserController : ControllerBase
{
    private readonly LMContext _context;

    public UserController(LMContext ctx)
    {
        _context = ctx;
    }

    [HttpGet("getallusers")]
    [Authorize(Roles = "ADMINISTRATOR")]
    public async Task<ActionResult<List<Utente>>> GetUsers([FromQuery] bool showcompanies=true)
    {
        var users = await _context.Utenti.Select(u => new CreateUserDTO
        {
            Username = u.Username,
            Password = u.Password,
            Gender = u.Gender,
            Role = u.Role,
            SocietàUtente = showcompanies == true ? u.SocietàUtente.Select(s => new CreateCompanyDTO
            {
                PartitaIVA = s.PartitaIVA,
                RagioneSociale = s.RagioneSociale,
                UsernameUtente = s.UsernameUtente,
                Logo = s.Logo
            }).ToList() ?? new List<CreateCompanyDTO>() : new List<CreateCompanyDTO>()

        }).ToListAsync();
        return Ok(users);
    }

    [HttpGet("getuser/{Username}")]
    [Authorize]
    public async Task<ActionResult<Utente>> GetUser(string Username)
    {
        var user = await _context.Utenti.Where(u => u.Username == Username).Select(u=>new CreateUserDTO
        {
            Username = u.Username,
            Password = u.Password,
            Gender = u.Gender,
            Role = u.Role,
            SocietàUtente = u.SocietàUtente.Select(s => new CreateCompanyDTO
            {
                PartitaIVA = s.PartitaIVA,
                RagioneSociale = s.RagioneSociale,
                UsernameUtente = s.UsernameUtente,
                Logo = s.Logo
            }).ToList() ?? new List<CreateCompanyDTO>()
        }).FirstOrDefaultAsync();
        if(user is null)
        {
            return NotFound();
        }
        if (User.IsInRole("USER"))
        {
            if (User.Identity?.Name != user.Username)
            {
                return NotFound("Non è possibile visualizzare i dati di un altro utente");
            }
        }
        return Ok(user);
    }

    /* INUTILE DAL MOMENTO IN CUI ESISTE LA REGISTER PER INSERIRE UTENTI
     * 
    [HttpPost("insertuser")]
    [Authorize(Roles = "ADMINISTRATOR")]
    public async Task<ActionResult> UserInsert([FromBody] CreateUserDTO userDTO)
    {
        if (!ModelState.IsValid)
        { return BadRequest(ModelState); }
        
        var user = new Utente
        {
            Username = userDTO.Username,
            Password = userDTO.Password,
            Gender = userDTO.Gender,
            Role = userDTO.Role
        };

        user.Username = char.ToUpper(user.Username[0]) + user.Username.Substring(1).ToLower();
        user.Gender = user.Gender.ToUpper();
        user.Role = user.Role.ToUpper();

        _context.Utenti.Add(user);
        try
        {
            await _context.SaveChangesAsync();
            return Ok(user);
        }
        catch(DbUpdateException ex) when (ex.InnerException is SqlException SqlException && SqlException.Number == 2627)
        {
            return BadRequest("User already exists");
        }
    }
    */

    [HttpPatch("edituser/{Username}")]
    [Authorize]
    public async Task<ActionResult> UserEdit(string Username, [FromQuery] string? newnam, [FromQuery] string? newpas, [FromQuery] string? newgen, [FromQuery] string? newrol)
    {
        if (User.IsInRole("USER"))
        {
            if (User.Identity?.Name != Username)
            {
                return BadRequest("Non è possibile modificare un altro utente");
            }
            if(newrol != "USER")
            {
                return BadRequest("Non è possibile modificare il proprio ruolo");
            }
        }

        var updateduser = await _context.Utenti.FirstOrDefaultAsync(u => u.Username == Username);
        if (updateduser == null)
            return NotFound();
        if (newnam != null)
            updateduser.Username = newnam;
        if (newpas != null)
            updateduser.Password = RegisterLoginController.HashPassword(newpas);
        if (newgen != null)
            updateduser.Gender = newgen;
        if (newrol != null)
            updateduser.Role = newrol;
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("deleteuser/{Username}")]
    [Authorize]
    public async Task<ActionResult> DeleteUser(string Username)
    {
        Utente? user = await _context.Utenti.FindAsync(Username);
        if (user is null)
        {
            return NotFound("Utente non trovato");
        }

        if (User.IsInRole("USER"))
        {
            if(User.Identity?.Name != user.Username)
            {
                return NotFound("Non è possibile eliminare un altro utente");
            }
        }

        _context.Utenti.Remove(user);
        await _context.SaveChangesAsync();
        return Ok("Utente eliminato correttamente");
    }
}