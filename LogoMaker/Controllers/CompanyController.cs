using LogoMaker.DataAccess;
using LogoMaker.DTO;
using LogoMaker.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace LogoMaker.Controllers;

[ApiController]
[Route("api/companylist")]
public class CompanyController : ControllerBase
{
    private readonly LMContext _context;

    private static readonly Semaphore _semaphore = new Semaphore(initialCount: 1, maximumCount: 1);

    public CompanyController(LMContext ctx)
    {
        _context = ctx;
    }

    [HttpGet("getallcompanies")]
    [Authorize]
    public async Task<ActionResult<List<Società>>> GetCompanies()
    {
        if (User.IsInRole("USER"))
        {
            var usercompanies = await _context.Società.Where(s => s.UsernameUtente == User.Identity.Name).Select(s => new CreateCompanyDTO
            {
                PartitaIVA = s.PartitaIVA,
                RagioneSociale = s.RagioneSociale,
                Logo = s.Logo,
                UsernameUtente = s.UsernameUtente
            }).ToListAsync();
            return Ok(usercompanies);
        }
        var companies = await _context.Società.Select(s=> new CreateCompanyDTO
        {
            PartitaIVA = s.PartitaIVA,
            RagioneSociale = s.RagioneSociale,
            Logo = s.Logo,
            UsernameUtente = s.UsernameUtente
        }).ToListAsync();
        return Ok(companies);
    }

    [HttpGet("getcompany/{PartitaIva}")]
    [Authorize]
    public async Task<ActionResult<Utente>> GetCompany(string PartitaIva)
    {
        var società = await _context.Società.Where(s => s.PartitaIVA == PartitaIva).Select(s => new CreateCompanyDTO
        {
            PartitaIVA = s.PartitaIVA,
            RagioneSociale = s.RagioneSociale,
            UsernameUtente = s.UsernameUtente,
            Logo = s.Logo
        }).FirstOrDefaultAsync();
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

    [HttpPost("insertcompany")]
    [Authorize]
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
            return Ok("Società inserita correttamente");
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException SqlException && SqlException.Number == 2627)
        {
            return BadRequest("Company already exists");
        }
    }

    [HttpPatch("editcompany/{societa}")]
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

    [HttpDelete("deletecompany/{PartitaIva}")]
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
    
    [HttpPost("generatelogo/{societa}")]
    [Authorize]
    public async Task<ActionResult> GenerateLogo(string societa)
    {

        var company = await _context.Società.FirstOrDefaultAsync(s => s.PartitaIVA == societa);

        if (company == null)
            return NotFound();

        if (User.IsInRole("USER"))
        {
            if (User.Identity?.Name != company.UsernameUtente)
            {
                return NotFound("Non è possibile generare loghi per società di altri utenti");
            }
        }

        _semaphore.WaitOne();
        try
        {
            using HttpClient client = new HttpClient();

            var colors = await client.GetFromJsonAsync<List<string>>("https://aptitudetestapi.azurewebsites.net/api/HexColor/HexColorArray?length=100");

            if (colors == null)
                return StatusCode(502, "Errore API esterna");

            company.Logo = colors;
            await _context.SaveChangesAsync();
        }
        finally
        {_semaphore.Release();}


        return Ok(new CreateCompanyDTO
        {
            PartitaIVA = company.PartitaIVA,
            RagioneSociale = company.RagioneSociale,
            Logo = company.Logo,
            UsernameUtente = company.UsernameUtente
        });

    }

}