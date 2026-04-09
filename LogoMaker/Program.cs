using Microsoft.Data.SqlClient;
using System.Globalization;

/*
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
*/

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
        var user = reader.GetString(0);
        var passw = reader.GetString(1);
        var gen = reader.GetString(2);
        var ruo = reader.GetString(3);
        Console.WriteLine($"{user}, {passw}, {gen}, {ruo}");
    }
}
catch(Exception e)
{
    Console.WriteLine("ECCEZIONE");
    Console.WriteLine(e.Message);
}