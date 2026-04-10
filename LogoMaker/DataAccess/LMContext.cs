using LogoMaker.Entities;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace LogoMaker.DataAccess;

public class LMContext : DbContext
{
    private string _connectionString = "Data Source=PC-STAGER\\SQLEXPRESS2025; Persist Security Info=True; User ID = sa; Password = 1111; DataBase = TESTDBGLV; Pooling = False; MultipleActiveResultSets = False; Encrypt = False; TrustServerCertificate = True; Application Name = \"SQL Server Management Studio\"; Command Timeout = 0";
    public DbSet<Utente> Utenti { get; set; }
    public DbSet<Società> Società { get; set; }
    public LMContext()
    {}
    public LMContext(string connectionString)
    {
        _connectionString = connectionString;
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_connectionString);
    }
}
