using CadeirasAPI.Data;
using CadeirasAPI.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace CadeirasApi.Controllers;

[ApiController]
[Route("cadeiras")]
public class CadeirasController : ControllerBase
{
    private readonly DBConnection _db;

    public CadeirasController(DBConnection db)
    {
        _db = db;
    }

    // GET: /cadeiras
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        await using var connection = await _db.GetConnectionAsync();

        const string query = "SELECT id, numero, descricao FROM cadeira ORDER BY numero;";

        var command = new MySqlCommand(query, connection);
        var reader = await command.ExecuteReaderAsync();

        var lista = new List<object>();

        while (await reader.ReadAsync())
        {
            lista.Add(new
            {
                Id = (Guid)reader["id"],
                Numero = reader.GetInt32(reader.GetOrdinal("numero")),
                Descricao = reader.IsDBNull(reader.GetOrdinal("descricao"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("descricao")),
            });
        }

        return Ok(lista);
    }

    // POST: /cadeiras
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CadeiraRequest request)
    {
        await using var connection = await _db.GetConnectionAsync();

        const string queryCadeiras = @"SELECT numero FROM cadeira WHERE numero = @numero;";

        await using (var commandCadeiras = new MySqlCommand(queryCadeiras, connection))
        {
            commandCadeiras.Parameters.AddWithValue("@numero", request.Numero);

            await using var reader = await commandCadeiras.ExecuteReaderAsync();

            if (await reader.ReadAsync())
                return BadRequest(new { Message = "Cadeira com esse número já existe." });
        }

        const string queryInsert = @"
            INSERT INTO cadeira (id, numero, descricao)
            VALUES (@id, @numero, @descricao);";

        var id = Guid.NewGuid();

        await using (var commandInsert = new MySqlCommand(queryInsert, connection))
        {
            commandInsert.Parameters.AddWithValue("@id", id);
            commandInsert.Parameters.AddWithValue("@numero", request.Numero);
            commandInsert.Parameters.AddWithValue("@descricao", request.Descricao ?? (object)DBNull.Value);

            await commandInsert.ExecuteNonQueryAsync();
        }

        return Created($"/cadeiras/{id}", new
        {
            id,
            request.Numero,
            request.Descricao
        });
    }

    // PUT: /cadeiras/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Put(Guid id, [FromBody] CadeiraRequest request)
    {
        await using var connection = await _db.GetConnectionAsync();

        const string queryCadeiras = @"SELECT numero FROM cadeira WHERE numero = @numero AND id != @id;";

        await using (var commandCadeiras = new MySqlCommand(queryCadeiras, connection))
        {
            commandCadeiras.Parameters.AddWithValue("@id", id);
            commandCadeiras.Parameters.AddWithValue("@numero", request.Numero);

            await using var reader = await commandCadeiras.ExecuteReaderAsync();

            if (await reader.ReadAsync())
                return BadRequest(new { Message = "Cadeira com esse número já existe." });
        }

        const string query = @"
            UPDATE cadeira 
            SET numero = @numero, descricao = @descricao
            WHERE id = @id";

        var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@numero", request.Numero);
        command.Parameters.AddWithValue("@descricao", request.Descricao ?? (object)DBNull.Value);

        var rowsAffected = await command.ExecuteNonQueryAsync();

        if (rowsAffected == 0)
            return NotFound(new { Message = "Cadeira não encontrada." });

        return Ok(new { Message = "Cadeira atualizada com sucesso." });
    }

    // DELETE: /cadeiras/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await using var connection = await _db.GetConnectionAsync();

        const string query = "DELETE FROM cadeira WHERE id = @id";

        var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);

        var rowsAffected = await command.ExecuteNonQueryAsync();

        if (rowsAffected == 0)
            return NotFound(new { Message = "Cadeira não encontrada." });

        return Ok(new { Message = "Cadeira deletada com sucesso." });
    }
}
