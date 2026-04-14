using LogoMaker.Entities;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net.Sockets;

namespace LogoMaker.DataAccess;

public class LMContext : DbContext
{
    private string _connectionString = String.Empty;
    public DbSet<Utente> Utenti { get; set; }
    public DbSet<Società> Società { get; set; }
    public LMContext()
    {}
    public LMContext(string connectionString)
    {
        _connectionString = connectionString;
    }
    public LMContext(DbContextOptions<LMContext> options)
    : base(options)
    {}
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (_connectionString != String.Empty)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}
