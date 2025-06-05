using CadeirasAPI.Data;
using CadeirasAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace CadeirasAPI.Routes;

public static class AlocacaoRoute
{
    public static void AlocacaoRoutes(this WebApplication app)
    {
        var routes = app.MapGroup("alocacao");

        // Alocar cadeiras
        routes.MapPost("alocar", async (
            [FromBody] AlocacaoRequest request,
            [FromServices] IConfiguration config) =>
        {
            if (request.DataHoraFim <= request.DataHoraInicio)
                return Results.BadRequest("Data/hora final deve ser maior que a inicial.");

            var connectionString = config.GetConnectionString("DefaultConnection");
            await using var connection = new MySql.Data.MySqlClient.MySqlConnection(connectionString);
            await connection.OpenAsync();

            // Buscar cadeiras que podem ser alocadas
            const string queryCadeiras = @"
                SELECT *
                FROM cadeirasdb.cadeira
                WHERE id NOT IN (
                    SELECT  cadeira_id
                    FROM alocacao
                    WHERE '@data_inicio' BETWEEN data_hora_init AND data_hora_fim
                        OR '@data_fim' BETWEEN data_hora_init AND data_hora_fim   
                )
                ORDER BY numero;";

            var commandCadeiras = new MySql.Data.MySqlClient.MySqlCommand(queryCadeiras, connection);
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
                return Results.BadRequest("Não há cadeiras disponíveis no periodo selecionado!!");

            // Alocação 
            TimeSpan duracaoBloco = TimeSpan.FromMinutes(60); // Blocos de 60 minutos
            var totalBlocos = (int)((request.DataHoraFim - request.DataHoraInicio).TotalMinutes / duracaoBloco.TotalMinutes);

            if (totalBlocos == 0)
                return Results.BadRequest("O período é muito curto para alocação.");

            var alocacoes = new List<(Guid Id, Guid CadeiraId, DateTime Inicio, DateTime Fim)>();

            for (int i = 0; i < totalBlocos; i++)
            {
                var cadeira = cadeiras[i % cadeiras.Count];
                var blocoInicio = request.DataHoraInicio.AddMinutes(i * duracaoBloco.TotalMinutes);
                var blocoFim = blocoInicio.Add(duracaoBloco);
                // Armazena as alocacoes 
                alocacoes.Add((Guid.NewGuid(), cadeira.Id, blocoInicio, blocoFim));
            }

            // Inserir alocações no banco
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


            return Results.Ok(new
            {
                Message = "Alocação realizada com sucesso.",
                TotalBlocos = totalBlocos,
                CadeirasAlocadas = alocacoes.Select(a => new
                {
                    id = a.Id,
                    CadeiraId = a.CadeiraId,
                    Inicio = a.Inicio,
                    Fim = a.Fim
                }),
            });
        });

        // Read 
        routes.MapGet("", async (
            [FromServices] IConfiguration config,
            Guid? cadeiraId
        ) =>
        {
            var connectionString = config.GetConnectionString("DefaultConnection");
            await using var connection = new MySql.Data.MySqlClient.MySqlConnection(connectionString);

            string query = @"SELECT a.id 
                                , a.data_hora_init 
                                , a.data_hora_fim    
                                , c.numero
                                , c.descricao
                            FROM alocacao a
                            INNER JOIN cadeira c ON (c.id = a.cadeira_id)";

            // Montagem dinâmica do filtro
            if (cadeiraId.HasValue)
                query += " WHERE a.cadeira_id = @cadeiraId ";


            query += " ORDER BY 2 ";

            await connection.OpenAsync();
            var command = new MySql.Data.MySqlClient.MySqlCommand(query, connection);

            // Adiciona parâmetros apenas se houver filtro
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
                    Descricao = reader.IsDBNull(reader.GetOrdinal("descricao")) ? null : reader.GetString(reader.GetOrdinal("descricao")),
                    DataHoraInicio = reader.IsDBNull(reader.GetOrdinal("data_hora_init")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("data_hora_init")),
                    DataHoraFim = reader.IsDBNull(reader.GetOrdinal("data_hora_fim")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("data_hora_fim"))
                });
            }

            return Results.Ok(lista);
        });

    }
}