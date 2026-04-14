using LogoMaker.DataAccess;
using LogoMaker.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using LogoMaker.DTO;

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
    public async Task<ActionResult<List<Società>>> GetCompanies()
    {
        var companies = await _context.Società.ToListAsync();
        return Ok(companies);
    }

    [HttpGet("{PartitaIva}")]
    public async Task<ActionResult<Utente>> GetCompany(string PartitaIva)
    {
        Società? società = await _context.Società.FirstOrDefaultAsync(s => s.PartitaIVA == PartitaIva);
        if (società is null)
        {
            return NotFound();
        }
        return Ok(società);
    }

    [HttpPost]
    public async Task<ActionResult> CompanyInsert([FromBody] CreateCompanyDTO companyDTO)
    {
        if (!ModelState.IsValid)
        { return BadRequest(ModelState); }

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
    public async Task<ActionResult> CompanyEdit(string societa, [FromQuery] string? newiva, [FromQuery] string? newrag, [FromQuery] string? newlog, [FromQuery] string? newuse)
    {
        var updatedcompany = await _context.Società.FirstOrDefaultAsync(s => s.PartitaIVA == societa);
        if (updatedcompany == null)
            return NotFound();
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
    public async Task<ActionResult> DeleteCompany(string PartitaIva)
    {
        Società? societa = await _context.Società.FindAsync(PartitaIva);
        if (societa is null)
        {
            return NotFound("Società non trovata");
        }

        _context.Società.Remove(societa);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
