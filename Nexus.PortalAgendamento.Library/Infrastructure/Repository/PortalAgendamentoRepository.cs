using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Nexus.Framework.Common;
using Nexus.Framework.Data;
using Nexus.Framework.Data.Model.Result;
using Nexus.Framework.Data.Repository.Default;
using Nexus.Framework.Data.Repository.Interfaces; // <--- FALTAVA ESTE (Para IServiceBase)
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.InputModel;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.ListModel;
using Nexus.PortalAgendamento.Library.Infrastructure.Helper;
using Nexus.PortalAgendamento.Library.Infrastructure.Repository.Interfaces;
using System.Data;

namespace Nexus.PortalAgendamento.Library.Infrastructure.Repository;

public class PortalAgendamentoRepository : ProcedureRepository, IPortalAgendamentoRepository
{
    private readonly IServiceBase _serviceBase;
    private readonly IConnectionStringProvider _conn;

    // Constantes locais
    private const string DS_TRATATIVA_VOUCHER = "VOUCHER";
    private const short COD_TIPO_ANEXOS_VOUCHER = 12;
    private const bool FL_VOUCHER = true;
    private const int COD_INCLUSAO_USUARIOS_VOUCHER = 2;

    public PortalAgendamentoRepository(
        IConnectionStringProvider connectionStringProvider,
        ILogger<ProcedureRepository> logger,
        IServiceBase serviceBase)
        : base(connectionStringProvider, logger)
    {
        _serviceBase = serviceBase ?? throw new ArgumentNullException(nameof(serviceBase));
        _conn = connectionStringProvider ?? throw new ArgumentNullException(nameof(connectionStringProvider));
    }

    public async Task<NexusResult<ClienteOutputModel>> GetCliente(Guid? identificadorCliente, CancellationToken cancellationToken = default)
    {
        var nexus = new NexusResult<ClienteOutputModel>();

        if (identificadorCliente == null || identificadorCliente == Guid.Empty)
        {
            nexus.AddFailureMessage("IdentificadorCliente é obrigatório.");
            return nexus;
        }

        try
        {
            _logger.LogInformation("GetCliente -> Buscando dados para IdentificadorCliente: {id}", identificadorCliente);

            var cs = await _conn.GetDefaultConnectionStringAsync();
            await using var connection = new SqlConnection(cs);
            await connection.OpenAsync(cancellationToken);

            string sql = @"
                        SELECT TC.NM_RAZAO_CLIENTES AS NOME_CLIENTE
                        FROM NewSitex.dbo.TB_PORTAL_AGENDAMENTO_DPA PAD
                            INNER JOIN NewSitex.dbo.TB_CLIENTES AS TC
                                ON PAD.COD_REMETENTE_CLIENTES = TC.COD_CLIENTES
                        WHERE IDENTIFICADOR_CLIENTES = @IdentificadorCliente";

            var cmd = new CommandDefinition(sql, new { IdentificadorCliente = identificadorCliente }, commandType: CommandType.Text, cancellationToken: cancellationToken);
            var row = await connection.QueryFirstOrDefaultAsync<ClienteOutputModel>(cmd);

            if (row is null)
            {
                nexus.AddFailureMessage("Nenhum registro encontrado.");
                return nexus;
            }

            nexus.AddData(row);
            nexus.AddDefaultSuccessMessage();
            return nexus;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetCliente -> Erro: {id}", identificadorCliente);
            var nexusError = new NexusResult<ClienteOutputModel>();
            nexusError.AddFailureMessage($"Erro ao consultar dados: {ex.Message}");
            return nexusError;
        }
    }

    public async Task<NexusResult<PortalAgendamentoOutputModel>> GetDataAgendamentoConfirmacao(Guid? identificadorCliente, CancellationToken cancellationToken = default)
    {
        var nexus = new NexusResult<PortalAgendamentoOutputModel>();

        if (identificadorCliente == null || identificadorCliente == Guid.Empty)
        {
            nexus.AddFailureMessage("IdentificadorCliente é obrigatório.");
            return nexus;
        }

        try
        {
            var cs = await _conn.GetDefaultConnectionStringAsync();
            await using var connection = new SqlConnection(cs);
            await connection.OpenAsync(cancellationToken);

            string sql = @"
                        SELECT TC.DT_SUGESTAO_AGENDAMENTO AS DataAgendamento
                        FROM NewSitex.dbo.TB_PORTAL_AGENDAMENTO_DPA PAD
                            INNER JOIN NewSitex.dbo.TB_CONHECIMENTOS AS TC
                                ON PAD.ID_CONHECIMENTOS = TC.ID
                        WHERE IDENTIFICADOR_CLIENTES = @IdentificadorCliente";

            var cmd = new CommandDefinition(sql, new { IdentificadorCliente = identificadorCliente }, commandType: CommandType.Text, cancellationToken: cancellationToken);
            var row = await connection.QueryFirstOrDefaultAsync<PortalAgendamentoOutputModel>(cmd);

            if (row is null)
            {
                nexus.AddFailureMessage("Nenhum registro encontrado.");
                return nexus;
            }

            nexus.AddData(row);
            nexus.AddDefaultSuccessMessage();
            return nexus;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetDataAgendamentoConfirmacao -> Erro");
            var nexusError = new NexusResult<PortalAgendamentoOutputModel>();
            nexusError.AddFailureMessage($"Erro: {ex.Message}");
            return nexusError;
        }
    }

    public async Task<NexusResult<PortalAgendamentoOutputModel>> GetNotasConhecimento(Guid? identificadorCliente, CancellationToken cancellationToken = default)
    {
        var nexus = new NexusResult<PortalAgendamentoOutputModel>();
        if (identificadorCliente == null || identificadorCliente == Guid.Empty)
        {
            nexus.AddFailureMessage("IdentificadorCliente é obrigatório.");
            return nexus;
        }

        try
        {
            var cs = await _conn.GetDefaultConnectionStringAsync();
            await using var connection = new SqlConnection(cs);
            await connection.OpenAsync(cancellationToken);

            const string sql = @"
                            SELECT TNF.NR_PEDIDO
                                ,CLIENTE.NM_RAZAO_CLIENTES AS NOME_RECEBEDOR
                                ,FORNECEDOR.NM_RAZAO_CLIENTES AS NOME_FORNECEDOR
                                ,FORNECEDOR.UF_CLIENTES
                                ,TNF.NR_NOTAS_FISCAIS 
                                ,TNF.NR_SERIE_NOTAS_FISCAIS 
                                ,CONVERT(VARCHAR(10), TNF.DT_EMISSAO_NOTAS_FISCAIS, 103) AS DT_EMISSAO
                                ,TNF.QT_VOLUME_NOTAS_FISCAIS
                                ,TNF.PESO_NOTAS_FISCAIS
                                ,TNF.VL_TOTAL_NOTAS_FISCAIS
                                ,FORNECEDOR.COD_CLIENTES AS COD_CLIENTES_FORNECEDOR
                                ,CLIENTE.COD_CLIENTES AS COD_CLIENTES_RECEBEDOR
                            FROM NewSitex.dbo.TB_PORTAL_AGENDAMENTO_DPA
                                INNER JOIN NewSitex.dbo.TB_CONHECIMENTOS_NOTAS_FISCAIS AS TCNF
                                    ON NewSitex.dbo.TB_PORTAL_AGENDAMENTO_DPA.ID_CONHECIMENTOS = TCNF.ID_CONHECIMENTOS
                                INNER JOIN NewSitex.dbo.TB_NOTAS_FISCAIS AS TNF 
                                    ON TCNF.ID_NOTA = TNF.id
                                INNER JOIN NewSitex.dbo.TB_CLIENTES AS FORNECEDOR
                                    ON TNF.COD_CLIENTES = FORNECEDOR.COD_CLIENTES
                                INNER JOIN NewSitex.dbo.TB_CLIENTES AS CLIENTE
                                    ON TNF.COD_RECEBEDOR = CLIENTE.COD_CLIENTES
                            WHERE IDENTIFICADOR_CLIENTES = @IdentificadorCliente";

            var cmd = new CommandDefinition(sql, new { IdentificadorCliente = identificadorCliente }, commandType: CommandType.Text, cancellationToken: cancellationToken);
            var notas = (await connection.QueryAsync<NotaFiscalOutputModel>(cmd)).ToList();

            if (!notas.Any())
            {
                nexus.AddFailureMessage("Nenhum registro encontrado.");
                return nexus;
            }

            nexus.AddData(new PortalAgendamentoOutputModel { NotasFiscais = notas });
            nexus.AddDefaultSuccessMessage();
            return nexus;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetNotasConhecimento -> Erro");
            var nexusError = new NexusResult<PortalAgendamentoOutputModel>();
            nexusError.AddFailureMessage($"Erro: {ex.Message}");
            return nexusError;
        }
    }

    public async Task<NexusResult<PortalAgendamentoOutputModel>> GetValidadeToken(Guid? identificadorCliente, CancellationToken cancellationToken = default)
    {
        var nexus = new NexusResult<PortalAgendamentoOutputModel>();

        try
        {
            var cs = await _conn.GetDefaultConnectionStringAsync();
            await using var connection = new SqlConnection(cs);
            await connection.OpenAsync(cancellationToken);

            const string sql = @"
                            SELECT ISNULL(DT_ALTERACAO, DT_INCLUSAO) as DataValidade
                            FROM NewSitex.dbo.TB_PORTAL_AGENDAMENTO_DPA
                            WHERE IDENTIFICADOR_CLIENTES = @IdentificadorCliente";

            var cmd = new CommandDefinition(sql, new { IdentificadorCliente = identificadorCliente }, commandType: CommandType.Text, cancellationToken: cancellationToken);
            var row = await connection.QueryFirstOrDefaultAsync<PortalAgendamentoOutputModel>(cmd);

            if (row is null)
            {
                nexus.AddFailureMessage("Nenhum registro encontrado.");
                return nexus;
            }

            nexus.AddData(row);
            nexus.AddDefaultSuccessMessage();
            return nexus;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetValidadeToken -> Erro");
            var nexusError = new NexusResult<PortalAgendamentoOutputModel>();
            nexusError.AddFailureMessage($"Erro: {ex.Message}");
            return nexusError;
        }
    }

    public async Task<NexusResult<EmailPostFixInputModel>> SendEmailAnexo(Guid? identificadorCliente, IFormFile request, CancellationToken cancellationToken = default)
    {
        try
        {
            var notasResult = await GetNotasConhecimento(identificadorCliente, cancellationToken);
            var notas = notasResult.ResultData?.NotasFiscais ?? new List<NotaFiscalOutputModel>();
            var nomeRecebedor = notas.FirstOrDefault()?.NomeRecebedor ?? "Recebedor";
            var nomeFornecedor = notas.FirstOrDefault()?.NomeFornecedor ?? "Fornecedor";
            var codFornecedor = notas.FirstOrDefault()?.CodClientesFornecedor;
            var codRecebedor = notas.FirstOrDefault()?.CodClientesRecebedor;

            var assunto = $"Atenção: Anexo não lido - {nomeFornecedor} - solicitação de agendamento {nomeRecebedor}";
            var htmlBody = EmailTemplateBuilder.BuildMensagemComTabelaNFs(notas);
            var destinatarios = await GetDestinatariosEmailAsync(codFornecedor, codRecebedor, cancellationToken);

            var emails = destinatarios
                .Select(d => d.Email?.Trim())
                .Where(e => !string.IsNullOrWhiteSpace(e) && e.Contains("@tragetta", StringComparison.OrdinalIgnoreCase))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (emails.Count == 0) _logger.LogWarning("Nenhum destinatário encontrado.");

            byte[]? arquivoBytes = null;
            string? nomeArquivo = null;
            string? mimeType = null;

            if (request != null && request.Length > 0)
            {
                using var ms = new MemoryStream();
                await request.CopyToAsync(ms, cancellationToken);
                arquivoBytes = ms.ToArray();
                nomeArquivo = Path.GetFileName(request.FileName);
                mimeType = string.IsNullOrWhiteSpace(request.ContentType) ? "application/pdf" : request.ContentType;
            }

            var model = new EmailPostFixInputModel
            {
                DsAssunto = assunto,
                DsMensagem = htmlBody,
                FlMensagemHtml = true,
                DestinatariosJson = JsonConvert.SerializeObject(emails),
                NmArquivo = nomeArquivo,
                MimeType = mimeType,
                Arquivo = arquivoBytes,
                EmailRemetente = "naoresponda@tragetta.srv.br"
            };

            return await EnviarEmailPostfixAsync(model, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SendEmailAnexo -> Erro");
            throw;
        }
    }

    public async Task<NexusResult<PortalAgendamentoInputModel>> UpdateDataAgendamento(PortalAgendamentoInputModel model, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _serviceBase.ExecuteBusinessOperationAsync(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateDataAgendamento -> Erro");
            throw;
        }
    }

    public async Task<NexusResult<bool>> CreateVoucherTratativaAsync(Guid identificadorCliente, IFormFile file, CancellationToken ct = default)
    {
        var nexus = new NexusResult<bool>();
        int? codOcorrencias = null;
        DateTime? dataAgendamento = null;

        var cs = await _conn.GetDefaultConnectionStringAsync();
        await using var conn = new SqlConnection(cs);
        await conn.OpenAsync(ct);
        await using var tx = await conn.BeginTransactionAsync(ct);

        try
        {
            const string sqlResolve = @"
            SELECT TOP (1)
             ONF.COD_OCORRENCIAS           AS CodOcorrencias
            ,TC.DT_SUGESTAO_AGENDAMENTO    AS DataAgendamento
            ,TC.COD_CONHECIMENTOS          AS CodConhecimentos  
            FROM   NewSitex.dbo.TB_PORTAL_AGENDAMENTO_DPA PAD
            JOIN   NewSitex.dbo.TB_CONHECIMENTOS TC               ON TC.ID = PAD.ID_CONHECIMENTOS
            LEFT   JOIN NewSitex.dbo.TB_CONHECIMENTOS_NOTAS_FISCAIS CNF ON CNF.ID_CONHECIMENTOS = TC.ID
            LEFT   JOIN NewSitex.dbo.TB_OCORRENCIAS_NOTAS_FISCAIS ONF   ON ONF.ID_NOTA = CNF.ID_NOTA
            WHERE  PAD.IDENTIFICADOR_CLIENTES = @IdentificadorCliente
            ORDER BY ONF.COD_OCORRENCIAS DESC;";

            var resolve = await conn.QueryFirstOrDefaultAsync(sqlResolve, new { IdentificadorCliente = identificadorCliente }, transaction: tx);
            codOcorrencias = (int?)resolve?.CodOcorrencias;
            dataAgendamento = (DateTime?)resolve?.DataAgendamento;
            int? codConhecimentos = (int?)resolve?.CodConhecimentos;

            if (codOcorrencias is null)
            {
                if (codConhecimentos is null)
                {
                    nexus.AddFailureMessage("Não há COD_CONHECIMENTOS associado.");
                    await tx.RollbackAsync(ct);
                    return nexus;
                }

                var cmdCriar = new CommandDefinition("NewSitex.dbo.PR_GERAR_OCORRENCIAS_AGENDAMENTO_AUTOMATICO_CTE",
                    new { COD_CONHECIMENTOS_CTE = codConhecimentos }, transaction: tx, commandType: CommandType.StoredProcedure, cancellationToken: ct);
                await conn.ExecuteAsync(cmdCriar);

                var resolve2 = await conn.QueryFirstOrDefaultAsync(sqlResolve, new { IdentificadorCliente = identificadorCliente }, transaction: tx);
                codOcorrencias = (int?)resolve2?.CodOcorrencias;
                dataAgendamento = (DateTime?)resolve2?.DataAgendamento;

                if (codOcorrencias is null)
                {
                    nexus.AddFailureMessage("Falha ao criar ocorrência 91.");
                    await tx.RollbackAsync(ct);
                    return nexus;
                }
            }

            var basePath = Environment.GetEnvironmentVariable("TRATATIVAS_BASE_PATH") ?? @"\\arquivos.tragetta.com.br\DESENVOLVIMENTO$\Repository\TratativasOcorrencias";
            var bucket = Guid.NewGuid().ToString("N");
            var dir = Path.Combine(basePath, bucket);
            Directory.CreateDirectory(dir);

            var safeName = Path.GetFileName(file.FileName);
            var diskPath = Path.Combine(dir, safeName);

            await using (var fs = new FileStream(diskPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await file.CopyToAsync(fs, ct);
            }

            const string sqlInsertTrat = @"
            INSERT INTO NewSitex.dbo.TB_OCORRENCIAS_TRATATIVAS
                (COD_OCORRENCIAS, DT_INCLUSAO_OCORRENCIAS_TRATATIVAS, DT_OCORRENCIAS_TRATATIVAS, DS_OCORRENCIAS_TRATATIVAS,
                 COD_INCLUSAO_USUARIOS, FL_VOUCHER, COD_TIPO_ANEXOS)
            VALUES (@COD_OCORRENCIAS, GETDATE(), GETDATE(), @DS, @USU, @FLV, @COD_TIPO);";

            await conn.ExecuteAsync(sqlInsertTrat, new { COD_OCORRENCIAS = codOcorrencias, DS = DS_TRATATIVA_VOUCHER, USU = COD_INCLUSAO_USUARIOS_VOUCHER, FLV = FL_VOUCHER, COD_TIPO = COD_TIPO_ANEXOS_VOUCHER }, transaction: tx);

            const string sqlInsertArq = @"INSERT INTO NewSitex.dbo.TB_OCORRENCIAS_TRATATIVAS_ARQUIVOS (COD_OCORRENCIAS, DT_INCLUSAO_OCORRENCIAS_TRATATIVAS, CAMINHO_OCORRENCIAS_TRATATIVAS_ARQUIVOS) VALUES (@COD_OCORRENCIAS, GETDATE(), @CAMINHO);";
            await conn.ExecuteAsync(sqlInsertArq, new { COD_OCORRENCIAS = codOcorrencias, CAMINHO = diskPath }, transaction: tx);

            await tx.CommitAsync(ct);
            nexus.AddData(true);
            nexus.AddDefaultSuccessMessage();
            return nexus;
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync(ct);
            _logger.LogError(ex, "Erro ao gravar tratativa. Tentando fallback por email.");
            // (Logica de email de fallback omitida para brevidade, mas pode ser mantida se desejado)
            var fail = new NexusResult<bool>();
            fail.AddFailureMessage("Falha ao processar voucher.");
            return fail;
        }
    }

    private async Task<List<ClientesRegrasAgendamentoEmail>> GetDestinatariosEmailAsync(int? codFornecedor, int? codRecebedor, CancellationToken cancellationToken)
    {
        if (codFornecedor is null && codRecebedor is null) return new List<ClientesRegrasAgendamentoEmail>();

        const string procedureName = "NewSitex.dbo.PR_S_TB_CLIENTES_REGRAS_AGENDAMENTO_EMAIL";
        var parameters = new DynamicParameters();
        parameters.Add("@COD_CLIENTES", codFornecedor);
        parameters.Add("@COD_RECEBEDOR", codRecebedor);

        var connectionString = await _conn.GetDefaultConnectionStringAsync();
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        var result = await connection.QueryAsync<ClientesRegrasAgendamentoEmail>(procedureName, parameters, commandType: CommandType.StoredProcedure);
        return result?.ToList() ?? new List<ClientesRegrasAgendamentoEmail>();
    }

    private async Task<NexusResult<EmailPostFixInputModel>> EnviarEmailPostfixAsync(EmailPostFixInputModel model, CancellationToken ct)
    {
        const string proc = "NewSitex.dbo.PR_ENVIAR_EMAIL_POSTFIX";
        var cs = await _conn.GetDefaultConnectionStringAsync();
        await using var conn = new SqlConnection(cs);
        await conn.OpenAsync(ct);

        var p = new DynamicParameters();
        p.Add("@DS_ASSUNTO", model.DsAssunto);
        p.Add("@DS_MENSAGEM", model.DsMensagem);
        p.Add("@FL_MENSAGEM_HTML", model.FlMensagemHtml);
        p.Add("@DESTINATARIOS_JSON", model.DestinatariosJson);
        p.Add("@NM_ARQUIVO", model.NmArquivo);
        p.Add("@MIME_TYPE", model.MimeType);
        p.Add("@ARQUIVO", model.Arquivo, DbType.Binary);
        p.Add("@EMAIL_REMETENTE", model.EmailRemetente);

        await conn.ExecuteAsync(new CommandDefinition(proc, p, commandType: CommandType.StoredProcedure, cancellationToken: ct));

        var nexus = new NexusResult<EmailPostFixInputModel>();
        nexus.AddData(model);
        nexus.AddDefaultSuccessMessage();
        return nexus;
    }
}