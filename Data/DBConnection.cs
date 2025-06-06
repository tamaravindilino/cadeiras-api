using MySql.Data.MySqlClient;
namespace CadeirasAPI.Data;

public class DBConnection
{
    private readonly string _connectionString;

    public DBConnection(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")
            ?? throw new ArgumentNullException("DefaultConnection", "Connection string 'DefaultConnection' not found.");
    }

    public async Task<MySqlConnection> GetConnectionAsync()
    {
        var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }
}