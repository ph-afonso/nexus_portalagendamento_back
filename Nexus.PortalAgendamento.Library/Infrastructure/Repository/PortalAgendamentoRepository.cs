using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Nexus.Framework.Common;
using Nexus.Framework.Data;
using Nexus.Framework.Data.Model.Result;
using Nexus.Framework.Data.Repository.Default;
using Nexus.Framework.Data.Repository.Interfaces;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.InputModel;
using Nexus.PortalAgendamento.Library.Infrastructure.Repository.Interfaces;
using System.Data;

namespace Nexus.PortalAgendamento.Library.Infrastructure.Repository;

public class PortalAgendamentoRepository : ProcedureRepository, IPortalAgendamentoRepository
{
    private readonly IConnectionStringProvider _conn;

    public PortalAgendamentoRepository(
        IConnectionStringProvider connectionStringProvider,
        ILogger<ProcedureRepository> logger)
        : base(connectionStringProvider, logger)
    {
        _conn = connectionStringProvider;
    }

    public async Task<NexusResult<dynamic>> GetDadosValidacaoToken(Guid identificadorCliente, CancellationToken ct)
    {
        var nexus = new NexusResult<dynamic>();
        try
        {
            var cs = await _conn.GetDefaultConnectionStringAsync();
            await using var connection = new SqlConnection(cs);
            await connection.OpenAsync(ct);

            // Busca datas para validar regra de 48h
            const string sql = @"
                SELECT DT_INCLUSAO, DT_ALTERACAO 
                FROM TB_PORTAL_AGENDAMENTO_DPA 
                WHERE IDENTIFICADOR_CLIENTES = @Id";

            var row = await connection.QueryFirstOrDefaultAsync(sql, new { Id = identificadorCliente });

            if (row == null)
            {
                nexus.AddFailureMessage("Token não encontrado.");
                return nexus;
            }

            nexus.AddData(row);
            nexus.AddDefaultSuccessMessage();
            return nexus;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao validar token");
            // Em caso de erro de banco, retornamos falha segura
            var err = new NexusResult<dynamic>();
            err.AddFailureMessage("Erro interno ao validar token.");
            return err;
        }
    }

    public async Task<NexusResult<bool>> ConfirmarAgendamento(ConfirmarAgendamentoInputModel model, CancellationToken ct)
    {
        // TODO: Ajustar update real da tabela TB_PORTAL_AGENDAMENTO_DPA
        // Exemplo: UPDATE TB_PORTAL... SET FL_CONFIRMADO = 1 WHERE IDENTIFICADOR_CLIENTES = ...
        return await Task.FromResult(new NexusResult<bool>().AddData(true));
    }

    public async Task<NexusResult<bool>> SolicitarNovaData(SolicitarNovaDataInputModel model, CancellationToken ct)
    {
        // TODO: Ajustar insert/update real
        return await Task.FromResult(new NexusResult<bool>().AddData(true));
    }

    public async Task<NexusResult<bool>> RegistrarAnexo(Guid identificadorCliente, string caminhoArquivo, CancellationToken ct)
    {
        // TODO: Inserir na tabela de anexos
        return await Task.FromResult(new NexusResult<bool>().AddData(true));
    }
}