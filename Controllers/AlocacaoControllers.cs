using CadeirasAPI.Data;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using CadeirasAPI.Models;

namespace CadeirasApi.Controllers;

[ApiController]
[Route("alocacao")]
public class AlocacaoController : ControllerBase
{
    private readonly DBConnection _db;

    public AlocacaoController(DBConnection db)
    {
        _db = db;
    }

    // POST: /alocacao/alocar
    [HttpPost("alocar")]
    public async Task<IActionResult> Alocar([FromBody] AlocacaoRequest request)
    {
        if (request.DataHoraFim <= request.DataHoraInicio)
            return BadRequest("Data/hora final deve ser maior que a inicial.");

        await using var connection = await _db.GetConnectionAsync();

        const string queryCadeiras = @"
            SELECT *
            FROM cadeira
            WHERE id NOT IN (
                SELECT cadeira_id
                FROM alocacao
                WHERE @data_inicio BETWEEN data_hora_init AND data_hora_fim
                   OR @data_fim BETWEEN data_hora_init AND data_hora_fim
            )
            ORDER BY numero;";

        var commandCadeiras = new MySqlCommand(queryCadeiras, connection);
        commandCadeiras.Parameters.AddWithValue("@data_inicio", request.DataHoraInicio);
        commandCadeiras.Parameters.AddWithValue("@data_fim", request.DataHoraFim);

        var reader = await commandCadeiras.ExecuteReaderAsync();

        var cadeiras = new List<(Guid Id, int Numero)>();

        while (await reader.ReadAsync())
        {
            var id = reader.GetGuid(reader.GetOrdinal("id"));
            var numero = reader.GetInt32(reader.GetOrdinal("numero"));
            cadeiras.Add((id, numero));
        }

        await reader.CloseAsync();

        if (!cadeiras.Any())
            return BadRequest("Não há cadeiras disponíveis no período selecionado.");

        // Calcula blocos de 60 minutos
        TimeSpan duracaoBloco = TimeSpan.FromMinutes(60);
        var totalBlocos = (int)((request.DataHoraFim - request.DataHoraInicio).TotalMinutes / duracaoBloco.TotalMinutes);

        if (totalBlocos == 0)
            return BadRequest("O período é muito curto para alocação.");

        var alocacoes = new List<(Guid Id, Guid CadeiraId, DateTime Inicio, DateTime Fim)>();

        for (int i = 0; i < totalBlocos; i++)
        {
            var cadeira = cadeiras[i % cadeiras.Count];
            var blocoInicio = request.DataHoraInicio.AddMinutes(i * duracaoBloco.TotalMinutes);
            var blocoFim = blocoInicio.Add(duracaoBloco);
            alocacoes.Add((Guid.NewGuid(), cadeira.Id, blocoInicio, blocoFim));
        }

        // Inserir no banco
        const string insertQuery = @"
            INSERT INTO alocacao (id, cadeira_id, data_hora_init, data_hora_fim)
            VALUES (@id, @cadeira_id, @data_hora_init, @data_hora_fim);";

        foreach (var aloc in alocacoes)
        {
            var insertCmd = new MySqlCommand(insertQuery, connection);
            insertCmd.Parameters.AddWithValue("@id", aloc.Id);
            insertCmd.Parameters.AddWithValue("@cadeira_id", aloc.CadeiraId);
            insertCmd.Parameters.AddWithValue("@data_hora_init", aloc.Inicio);
            insertCmd.Parameters.AddWithValue("@data_hora_fim", aloc.Fim);
            await insertCmd.ExecuteNonQueryAsync();
        }

        return Ok(new
        {
            Message = "Alocação realizada com sucesso.",
            TotalBlocos = totalBlocos,
            CadeirasAlocadas = alocacoes.Select(a => new
            {
                Id = a.Id,
                CadeiraId = a.CadeiraId,
                Inicio = a.Inicio,
                Fim = a.Fim
            }),
        });
    }

    // GET: /alocacao
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Guid? cadeiraId)
    {
        await using var connection = await _db.GetConnectionAsync();

        string query = @"
            SELECT a.id,
                   a.data_hora_init,
                   a.data_hora_fim,
                   c.numero,
                   c.descricao
            FROM alocacao a
            INNER JOIN cadeira c ON c.id = a.cadeira_id";

        if (cadeiraId.HasValue)
            query += " WHERE a.cadeira_id = @cadeiraId";

        query += " ORDER BY a.data_hora_init;";

        var command = new MySqlCommand(query, connection);

        if (cadeiraId.HasValue)
            command.Parameters.AddWithValue("@cadeiraId", cadeiraId);

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
                DataHoraInicio = reader.IsDBNull(reader.GetOrdinal("data_hora_init"))
                    ? (DateTime?)null
                    : reader.GetDateTime(reader.GetOrdinal("data_hora_init")),
                DataHoraFim = reader.IsDBNull(reader.GetOrdinal("data_hora_fim"))
                    ? (DateTime?)null
                    : reader.GetDateTime(reader.GetOrdinal("data_hora_fim"))
            });
        }

        return Ok(lista);
    }
}
