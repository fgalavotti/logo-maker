using LogoMaker.DataAccess;
using LogoMaker.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using LogoMaker.Controllers;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("LMContext");
builder.Services.AddDbContext<LMContext>(opt => opt.UseSqlServer(connectionString));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


/*

try //test funzionamento connessione database
{
    string connectionString = """Data Source=PC-STAGER\SQLEXPRESS2025; Persist Security Info=True; User ID = sa; Password = 1111; DataBase = TESTDBGLV; Pooling = False; MultipleActiveResultSets = False; Encrypt = False; TrustServerCertificate = True; Application Name = "SQL Server Management Studio"; Command Timeout = 0""";
    using SqlConnection conn = new(connectionString);
    conn.Open();
    string query = "SELECT * FROM usertable";
    SqlCommand cmd = new(query, conn);
    SqlDataReader reader = cmd.ExecuteReader();
    while (reader.Read())
    {
        var user = reader.IsDBNull(0) ? null : reader.GetString(0);
        var passw = reader.IsDBNull(1) ? null : reader.GetString(1);
        var gen = reader.IsDBNull(2) ? null : reader.GetString(2);
        var ruo = reader.IsDBNull(3) ? null : reader.GetString(3);
        Console.WriteLine($"{user}, {passw}, {gen}, {ruo}");
    }
    reader.Close();
    query = "SELECT * FROM companytable";
    cmd = new(query, conn);
    reader = cmd.ExecuteReader();
    while (reader.Read())
    {
        var par = reader.GetString(0);
        var rag = reader.GetString(1);
        var log = reader.GetString(2);
        var ut = reader.IsDBNull(3) ? null : reader.GetString(3);
        Console.WriteLine($"{par}, {rag}, {log}, {ut}");
    }
}
catch(Exception e)
{
    Console.WriteLine("ECCEZIONE");
    Console.WriteLine(e.Message);
}

//test funzionamento LMContext

string connstr = """Data Source=PC-STAGER\SQLEXPRESS2025; Persist Security Info=True; User ID = sa; Password = 1111; DataBase = TESTDBGLV; Pooling = False; MultipleActiveResultSets = False; Encrypt = False; TrustServerCertificate = True; Application Name = "SQL Server Management Studio"; Command Timeout = 0""";
using var ctx = new LMContext(connstr);

var nuovasoc = new Società
{
     PartitaIVA = "10101010101",
     RagioneSociale = "nomesoc",
     Logo = "log"
};
ctx.Società.Add(nuovasoc);
ctx.SaveChanges();

/*
foreach (var user in ctx.Utenti.Include(u => u.SocietàUtente))
{
    Console.WriteLine(user.Username);
    if(user.SocietàUtente != null)
    {
        foreach(var soc in user.SocietàUtente)
        {
            Console.WriteLine("XXXXXXX");
            Console.WriteLine(soc.PartitaIVA);
        }
    }
}*/