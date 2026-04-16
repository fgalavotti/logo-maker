using LogoMaker.DataAccess;
using LogoMaker.DTO;
using LogoMaker.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace LogoMaker.Controllers;

[ApiController]
[Route("api/companylist")]
public class CompanyController : ControllerBase
{
    private readonly LMContext _context;

    public CompanyController(LMContext ctx)
    {
        _context = ctx;
    }

    [HttpGet]
    [Authorize(Roles = "ADMINISTRATOR")]
    public async Task<ActionResult<List<Società>>> GetCompanies()
    {
        var companies = await _context.Società.ToListAsync();
        return Ok(companies);
    }

    [HttpGet("{PartitaIva}")]
    [Authorize]
    public async Task<ActionResult<Utente>> GetCompany(string PartitaIva)
    {
        Società? società = await _context.Società.FirstOrDefaultAsync(s => s.PartitaIVA == PartitaIva);
        if (società is null)
        {
            return NotFound();
        }
        if (User.IsInRole("USER"))
        {
            if (User.Identity?.Name != società.UsernameUtente)
            {
                return NotFound("Non è possibile visualizzare i dati di una società altrui utente");
            }
        }
        return Ok(società);
    }

    [HttpPost]
    //[Authorize]
    public async Task<ActionResult> CompanyInsert([FromBody] CreateCompanyDTO companyDTO)
    {
        if (!ModelState.IsValid)
        { return BadRequest(ModelState); }

        if (User.IsInRole("USER"))
        {
            if (User.Identity?.Name != companyDTO.UsernameUtente)
            {
                return BadRequest("Non è possibile inserie società a nome di altri utenti");
            }
        }

        var societa = new Società
        {
            PartitaIVA = companyDTO.PartitaIVA,
            RagioneSociale = companyDTO.RagioneSociale,
            Logo = companyDTO.Logo,
            UsernameUtente = companyDTO.UsernameUtente
        };

        societa.UsernameUtente = char.ToUpper(societa.UsernameUtente[0]) + societa.UsernameUtente.Substring(1).ToLower();
        if (societa.UsernameUtente == "") { societa.UsernameUtente = null; }

        _context.Società.Add(societa);
        try
        {
            await _context.SaveChangesAsync();
            return Ok(societa);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException SqlException && SqlException.Number == 2627)
        {
            return BadRequest("Company already exists");
        }
    }

    [HttpPatch("{societa}")]
    [Authorize]
    public async Task<ActionResult> CompanyEdit(string societa, [FromQuery] string? newiva, [FromQuery] string? newrag, [FromQuery] List<string>? newlog, [FromQuery] string? newuse)
    {
        var updatedcompany = await _context.Società.FirstOrDefaultAsync(s => s.PartitaIVA == societa);
        if (updatedcompany == null)
            return NotFound();

        if (User.IsInRole("USER"))
        {
            if (User.Identity?.Name != updatedcompany.UsernameUtente)
            {
                return BadRequest("Non è possibile modificare la società di un altro utente");
            }
            if (User.Identity?.Name != newuse)
            {
                return BadRequest("Non è possibile assegnare un nome altrui nella modifica di una società");
            }
        }

        if (newiva != null)
            updatedcompany.PartitaIVA = newiva;
        if (newrag != null)
            updatedcompany.RagioneSociale = newrag;
        if (newlog != null)
            updatedcompany.Logo = newlog;
        if (newuse != null)
            updatedcompany.UsernameUtente = newuse;
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("{PartitaIva}")]
    [Authorize]
    public async Task<ActionResult> DeleteCompany(string PartitaIva)
    {
        Società? societa = await _context.Società.FindAsync(PartitaIva);
        if (societa is null)
        {
            return NotFound("Società non trovata");
        }
        if (User.IsInRole("USER"))
        {
            if (User.Identity?.Name != societa.UsernameUtente)
            {
                return NotFound("Non è possibile eliminare società di altri utenti");
            }
        }
        _context.Società.Remove(societa);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    
    [HttpPost("{societa}")]
    //[Authorize]
    public async Task<ActionResult> GenerateLogo(string societa)
    {

        var company = await _context.Società.FirstOrDefaultAsync(s => s.PartitaIVA == societa);

        if (company == null)
            return NotFound();

        using HttpClient client = new HttpClient();

        var colors = await client
            .GetFromJsonAsync<List<string>>(
                "https://aptitudetestapi.azurewebsites.net/api/HexColor/HexColorArray?length=5"
            );

        if (colors == null)
            return StatusCode(502, "Errore API esterna");

        company.Logo = colors;

        await _context.SaveChangesAsync();

        return Ok(company);

    }

}