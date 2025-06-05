using CadeirasAPI.Data;
using CadeirasAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace CadeirasAPI.Routes;

public static class CadeirasRoute
{
    public static void CadeirasRoutes(this WebApplication app)
    {
        var routes = app.MapGroup("cadeiras");

        // Insert 
        routes.MapPost("", async (
            [FromBody] CadeiraRequest request,
            [FromServices] IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySql.Data.MySqlClient.MySqlConnection(connectionString);
            await connection.OpenAsync();

            // Valida duplicidade do número da cadeira
            const string queryCadeiras = @"SELECT numero FROM cadeira WHERE numero = @numero;";

            await using (var commandCadeiras = new MySql.Data.MySqlClient.MySqlCommand(queryCadeiras, connection))
            {
                commandCadeiras.Parameters.AddWithValue("@numero", request.Numero);

                await using var reader = await commandCadeiras.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                    return Results.BadRequest(new { Message = "Cadeira com esse número já existe." });
            }

            // Query para insert
            const string queryInsert = @"
                INSERT INTO cadeira (id, numero, descricao)
                VALUES (@id, @numero, @descricao);";

            var id = Guid.NewGuid();

            await using (var commandInsert = new MySql.Data.MySqlClient.MySqlCommand(queryInsert, connection))
            {
                commandInsert.Parameters.AddWithValue("@id", id);
                commandInsert.Parameters.AddWithValue("@numero", request.Numero);
                commandInsert.Parameters.AddWithValue("@descricao", request.Descricao ?? (object)DBNull.Value); ;

                await commandInsert.ExecuteNonQueryAsync();
            }

            return Results.Created($"/cadeiras/{id}", new
            {
                id,
                request.Numero,
                request.Descricao
            });
        });


        // Update 
        routes.MapPut("{id:guid}", async (
            Guid id,
            [FromBody] CadeiraRequest request,
            [FromServices] IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            await using var connection = new MySql.Data.MySqlClient.MySqlConnection(connectionString);
            await connection.OpenAsync();

            // Valida duplicidade do número da cadeira
            const string queryCadeiras = @"SELECT numero FROM cadeira WHERE numero = @numero AND id != @id;";

            await using (var commandCadeiras = new MySql.Data.MySqlClient.MySqlCommand(queryCadeiras, connection))
            {
                commandCadeiras.Parameters.AddWithValue("@id", id);
                commandCadeiras.Parameters.AddWithValue("@numero", request.Numero);

                await using var reader = await commandCadeiras.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                    return Results.BadRequest(new { Message = "Cadeira com esse número já existe." });
            }

            // Query para update
            const string query = @"
                UPDATE cadeira 
                SET numero = @numero, descricao = @descricao
                WHERE id = @id";

            var command = new MySql.Data.MySqlClient.MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@numero", request.Numero);
            command.Parameters.AddWithValue("@descricao", request.Descricao ?? (object)DBNull.Value);

            var rowsAffected = await command.ExecuteNonQueryAsync();

            if (rowsAffected == 0)
                return Results.NotFound(new { Message = "Cadeira não encontrada." });

            return Results.Ok(new { Message = "Cadeira atualizada com sucesso." });
        });


        // Delete 
        routes.MapDelete("{id:guid}", async (
            Guid id,
            [FromServices] IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");
            await using var connection = new MySql.Data.MySqlClient.MySqlConnection(connectionString);

            const string query = "DELETE FROM cadeira WHERE id = @id";

            await connection.OpenAsync();

            var command = new MySql.Data.MySqlClient.MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);

            var rowsAffected = await command.ExecuteNonQueryAsync();

            if (rowsAffected == 0)
                return Results.NotFound(new { Message = "Cadeira não encontrada." });

            return Results.Ok(new { Message = "Cadeira deletada com sucesso." });
        });


        // Read 
        routes.MapGet("", async (
            [FromServices] IConfiguration config) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");
            await using var connection = new MySql.Data.MySqlClient.MySqlConnection(connectionString);

            const string query = "SELECT id, numero, descricao FROM cadeira ORDER BY numero;";

            await connection.OpenAsync();

            var command = new MySql.Data.MySqlClient.MySqlCommand(query, connection);
            var reader = await command.ExecuteReaderAsync();

            var lista = new List<object>();

            while (await reader.ReadAsync())
            {
                lista.Add(new
                {
                    Id = (Guid)reader["id"],
                    Numero = reader.GetInt32(reader.GetOrdinal("numero")),
                    Descricao = reader.IsDBNull(reader.GetOrdinal("descricao")) ? null : reader.GetString(reader.GetOrdinal("descricao")),
                });
            }

            return Results.Ok(lista);
        });
    }
}