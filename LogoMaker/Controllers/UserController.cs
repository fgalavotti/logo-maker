using LogoMaker.DataAccess;
using LogoMaker.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using LogoMaker.DTO;

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

    [HttpGet]
    public async Task<ActionResult<List<Utente>>> GetUsers()
    {
        var users= await _context.Utenti.ToListAsync();
        return Ok(users);
    }

    [HttpGet("{Username}")]
    public async Task<ActionResult<Utente>> GetUser(string Username)
    {
        Utente? user = await _context.Utenti.FirstOrDefaultAsync(u => u.Username == Username);
        if(user is null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [HttpPost]
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

    [HttpPatch("{Username}")]
    public async Task<ActionResult> UserEdit(string Username, [FromQuery] string? newnam, [FromQuery] string? newpas, [FromQuery] string? newgen, [FromQuery] string? newrol)
    {
        var updateduser = await _context.Utenti.FirstOrDefaultAsync(u => u.Username == Username);
        if (updateduser == null)
            return NotFound();
        if (newnam != null)
            updateduser.Username = newnam;
        if (newpas != null)
            updateduser.Password = newpas;
        if (newgen != null)
            updateduser.Gender = newgen;
        if (newrol != null)
            updateduser.Role = newrol;
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("{Username}")]
    public async Task<ActionResult> DeleteUser(string Username)
    {
        Utente? user = await _context.Utenti.FindAsync(Username);
        if (user is null)
        {
            return NotFound("Utente non trovato");
        }

        _context.Utenti.Remove(user);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
