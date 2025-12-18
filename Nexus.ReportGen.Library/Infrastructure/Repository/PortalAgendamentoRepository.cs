using Microsoft.Extensions.Logging;
using Nexus.Framework.Data;
using Nexus.Framework.Data.Model.Input;
using Nexus.Framework.Data.Model.Result;
using Nexus.Framework.Data.Repository.Default;
using Nexus.Framework.Data.Repository.Interfaces;
using Nexus.Framework.Common;
using Nexus.Sample.Library.Infrastructure.Domain.InputModel;
using Nexus.Sample.Library.Infrastructure.Domain.ListModel;
using Nexus.Sample.Library.Infrastructure.Repository.Interfaces;
using Nexus.Sample.Library.Infrastructure.Constants;
using Nexus.ReportGen.Library.Infrastructure.Repository.Interfaces;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Data;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nexus.Sample.Library.Infrastructure.Repository;

/// <summary>
/// Implementação do repositório de Portal Agendamento usando ProcedureRepository
/// </summary>
public class PortalAgendamentoRepository : ProcedureRepository, IPortalAgendamentoRepository
{
    private readonly IServiceBase _serviceBase;
    private readonly IConnectionStringProvider _conn;
    private const string DS_TRATATIVA_VOUCHER = "VOUCHER";
    private const short COD_TIPO_ANEXOS_VOUCHER = 12;
    private const bool FL_VOUCHER = true;
    private const int COD_INCLUSAO_USUARIOS_VOUCHER = 2;


    /// <summary>
    /// Construtor
    /// </summary>
    public PortalAgendamentoRepository(
        IConnectionStringProvider connectionStringProvider,
        ILogger<ProcedureRepository> logger,
        IServiceBase serviceBase)
        : base(connectionStringProvider, logger)
    {
        _serviceBase = serviceBase ?? throw new ArgumentNullException(nameof(serviceBase));
        _conn = connectionStringProvider ?? throw new ArgumentNullException(nameof(connectionStringProvider));
    }

    /// <inheritdoc />
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
            _logger.LogInformation(
                "GetCliente -> Buscando dados para IdentificadorCliente: {id}",
                identificadorCliente);

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
                nexus.AddFailureMessage("Nenhum registro encontrado para o Identificador informado.");
                return nexus;
            }

            nexus.AddData(row);
            nexus.AddDefaultSuccessMessage();
            _logger.LogInformation(
                "GetCliente -> Sucesso ao buscar dados para IdentificadorCliente: {id}",
                identificadorCliente);

            return nexus;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "GetCliente -> Erro ao buscar dados para IdentificadorCliente: {id}",
                identificadorCliente);
            var nexusError = new NexusResult<ClienteOutputModel>();
            nexusError.AddFailureMessage($"Erro ao consultar dados: {ex.Message}");
            return nexusError;
        }
    }

    /// <inheritdoc />
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
            _logger.LogInformation(
                "GetDataAgendamentoConfirmacao -> Buscando dados para IdentificadorCliente: {id}",
                identificadorCliente);

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
                nexus.AddFailureMessage("Nenhum registro encontrado para o Identificador informado.");
                return nexus;
            }

            nexus.AddData(row);
            nexus.AddDefaultSuccessMessage();
            _logger.LogInformation(
                "GetDataAgendamentoConfirmacao -> Sucesso ao buscar dados para IdentificadorCliente: {id}",
                identificadorCliente);

            return nexus;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "GetDataAgendamentoConfirmacao -> Erro ao buscar dados para IdentificadorCliente: {id}",
                identificadorCliente);
            var nexusError = new NexusResult<PortalAgendamentoOutputModel>();
            nexusError.AddFailureMessage($"Erro ao consultar dados: {ex.Message}");
            return nexusError;
        }
    }

    /// <inheritdoc />
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
            _logger.LogInformation(
                "GetNotasConhecimento -> Buscando dados para IdentificadorCliente: {id}",
                identificadorCliente);

            var cs = await _conn.GetDefaultConnectionStringAsync();
            await using var connection = new SqlConnection(cs);
            await connection.OpenAsync(cancellationToken);

            const string sql = @"
	                            SELECT	 TNF.NR_PEDIDO
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
                                WHERE IDENTIFICADOR_CLIENTES = @IdentificadorCliente
                             ";

            var cmd = new CommandDefinition(sql, new { IdentificadorCliente = identificadorCliente }, commandType: CommandType.Text, cancellationToken: cancellationToken);

            var notas = (await connection.QueryAsync<NotaFiscalOutputModel>(cmd)).ToList();
            if (!notas.Any())
            {
                nexus.AddFailureMessage("Nenhum registro encontrado para o Identificador informado.");
                return nexus;
            }
            var row = new PortalAgendamentoOutputModel
            {
                NotasFiscais = notas
            };

            if (row is null)
            {
                nexus.AddFailureMessage("Nenhum registro encontrado para o Identificador informado.");
                return nexus;
            }

            nexus.AddData(row);
            nexus.AddDefaultSuccessMessage();
            _logger.LogInformation(
                "GetNotasConhecimento -> Sucesso ao buscar dados para IdentificadorCliente: {id}",
                identificadorCliente);

            return nexus;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "GetNotasConhecimento -> Erro ao buscar dados para IdentificadorCliente: {id}",
                identificadorCliente);
            var nexusError = new NexusResult<PortalAgendamentoOutputModel>();
            nexusError.AddFailureMessage($"Erro ao consultar dados: {ex.Message}");
            return nexusError;
        }
    }


    /// <inheritdoc />
    public async Task<NexusResult<PortalAgendamentoOutputModel>> GetValidadeToken(Guid? identificadorCliente, CancellationToken cancellationToken = default)
    {
        var nexus = new NexusResult<PortalAgendamentoOutputModel>();

        if (identificadorCliente == null || identificadorCliente == Guid.Empty)
        {
            nexus.AddFailureMessage("IdentificadorCliente é obrigatório.");
            return nexus;
        }

        try
        {
            _logger.LogInformation(
                "GetValidadeToken -> Buscando dados para IdentificadorCliente: {id}",
                identificadorCliente);

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
                nexus.AddFailureMessage("Nenhum registro encontrado para o Identificador informado.");
                return nexus;
            }

            nexus.AddData(row);
            nexus.AddDefaultSuccessMessage();
            _logger.LogInformation(
                "GetValidadeToken -> Sucesso ao buscar dados para IdentificadorCliente: {id}",
                identificadorCliente);

            return nexus;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "GetValidadeToken -> Erro ao buscar dados para IdentificadorCliente: {id}",
                identificadorCliente);
            var nexusError = new NexusResult<PortalAgendamentoOutputModel>();
            nexusError.AddFailureMessage($"Erro ao consultar dados: {ex.Message}");
            return nexusError;
        }
    }

    /// <inheritdoc />
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
            var destinatarios = await GetDestinatariosEmailAsync(codFornecedor, codRecebedor, cancellationToken) ?? new List<ClientesRegrasAgendamentoEmail>();

            var emails = destinatarios
                .Select(d => d.Email?.Trim())
                .Where(e =>
                    !string.IsNullOrWhiteSpace(e) &&
                    e.Contains("@tragetta", StringComparison.OrdinalIgnoreCase)
                )
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (emails.Count == 0)
            {
                _logger.LogWarning("Nenhum destinatário de e-mail encontrado para fornecedor {codFornecedor} / recebedor {codRecebedor}.", codFornecedor, codRecebedor);
            }

            var destinatariosJson = JsonConvert.SerializeObject(emails);

            // >>> ALTERAÇÃO: anexar o arquivo recebido no endpoint
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
            // <<< ALTERAÇÃO

            var model = new EmailPostFixInputModel
            {
                CodRemetentePessoa = null,
                CodEventos = null,
                DsAssunto = assunto,
                DsMensagem = htmlBody,
                FlMensagemHtml = true,
                LoginRemetenteUsuarios = null,
                DestinatariosJson = destinatariosJson,
                NmArquivo = nomeArquivo,      // <<< ALTERAÇÃO
                MimeType = mimeType,          // <<< ALTERAÇÃO
                Arquivo = arquivoBytes,       // <<< ALTERAÇÃO
                EmailRemetente = "naoresponda@tragetta.srv.br",
                CaminhoArquivo = null
            };

            _logger.LogInformation("SendEmailAnexo -> Processando envio de email {@model}", model);

            if (model == null)
            {
                _logger.LogWarning("SendEmailAnexo -> Modelo não pode ser nulo");
                throw new ArgumentNullException(nameof(model), "Modelo não pode ser nulo");
            }

            var result = await EnviarEmailPostfixAsync(model, cancellationToken);

            _logger.LogInformation("SendEmailAnexo -> Email enviado com sucesso");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SendEmailAnexo -> Erro ao enviar email");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<NexusResult<PortalAgendamentoInputModel>> UpdateDataAgendamento(PortalAgendamentoInputModel model, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Salvar -> Processando salvamento de bancos: {@model}", model);

            if (model == null)
            {
                _logger.LogWarning("Salvar -> Modelo não pode ser nulo");
                throw new ArgumentNullException(nameof(model), "Modelo não pode ser nulo");
            }

            var result = await _serviceBase.ExecuteBusinessOperationAsync(model);

            _logger.LogInformation("Salvar -> Bancos salvo com sucesso");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Salvar -> Erro ao salvar bancos: {@model}", model);
            throw;
        }
    }

    private async Task<List<ClientesRegrasAgendamentoEmail>> GetDestinatariosEmailAsync(int? codFornecedor, int? codRecebedor, CancellationToken cancellationToken)
    {
        if (codFornecedor is null && codRecebedor is null)
            return new List<ClientesRegrasAgendamentoEmail>();

        const string procedureName = "NewSitex.dbo.PR_S_TB_CLIENTES_REGRAS_AGENDAMENTO_EMAIL";

        var parameters = new DynamicParameters();
        parameters.Add("@COD_CLIENTES", codFornecedor, DbType.Int32, ParameterDirection.Input);
        parameters.Add("@COD_RECEBEDOR", codRecebedor, DbType.Int32, ParameterDirection.Input);

        var connectionString = await _conn.GetDefaultConnectionStringAsync();
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        var command = new CommandDefinition(
            commandText: procedureName,
            parameters: parameters,
            commandType: CommandType.StoredProcedure,
            commandTimeout: 30,
            cancellationToken: cancellationToken
        );

        var result = await connection.QueryAsync<ClientesRegrasAgendamentoEmail>(command);
        return result?.ToList() ?? new List<ClientesRegrasAgendamentoEmail>();
    }

    private async Task<NexusResult<EmailPostFixInputModel>> EnviarEmailPostfixAsync(EmailPostFixInputModel model, CancellationToken ct)
    {
        const string proc = "NewSitex.dbo.PR_ENVIAR_EMAIL_POSTFIX";

        var cs = await _conn.GetDefaultConnectionStringAsync();
        await using var conn = new SqlConnection(cs);
        await conn.OpenAsync(ct);

        var p = new DynamicParameters();

        p.Add("@COD_REMETENTE_PESSOA", model.CodRemetentePessoa, DbType.Int32);
        p.Add("@COD_EVENTOS", model.CodEventos, DbType.Int16);
        p.Add("@DS_ASSUNTO", model.DsAssunto, DbType.String);
        p.Add("@DS_MENSAGEM", model.DsMensagem, DbType.String);
        p.Add("@FL_MENSAGEM_HTML", model.FlMensagemHtml ?? false, DbType.Boolean);
        p.Add("@LOGIN_REMETENTE_USUARIOS", model.LoginRemetenteUsuarios, DbType.String);
        p.Add("@DESTINATARIOS_JSON", model.DestinatariosJson, DbType.String);
        p.Add("@NM_ARQUIVO", model.NmArquivo, DbType.String);
        p.Add("@MIME_TYPE", model.MimeType, DbType.String);
        p.Add("@ARQUIVO", model.Arquivo, DbType.Binary);
        p.Add("@EMAIL_REMETENTE", model.EmailRemetente, DbType.String);
        p.Add("@CAMINHO_ARQUIVO", model.CaminhoArquivo, DbType.String);

        var cmd = new CommandDefinition(
            proc, p,
            commandType: CommandType.StoredProcedure,
            commandTimeout: 60,
            cancellationToken: ct
        );

        await conn.ExecuteAsync(cmd);


        var nexus = new NexusResult<EmailPostFixInputModel>();
        nexus.AddData(model);
        nexus.AddDefaultSuccessMessage();
        return nexus;
    }

    public async Task<NexusResult<bool>> CreateVoucherTratativaAsync(Guid identificadorCliente, IFormFile file, CancellationToken ct = default)
    {
        var nexus = new NexusResult<bool>();

        int? codOcorrencias = null;
        long? idConhecimentos = null;
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
            ,PAD.ID_CONHECIMENTOS          AS IdConhecimentos
            ,TC.COD_CONHECIMENTOS          AS CodConhecimentos  
            FROM   NewSitex.dbo.TB_PORTAL_AGENDAMENTO_DPA PAD
            JOIN   NewSitex.dbo.TB_CONHECIMENTOS TC               ON TC.ID = PAD.ID_CONHECIMENTOS
            LEFT   JOIN NewSitex.dbo.TB_CONHECIMENTOS_NOTAS_FISCAIS CNF ON CNF.ID_CONHECIMENTOS = TC.ID
            LEFT   JOIN NewSitex.dbo.TB_OCORRENCIAS_NOTAS_FISCAIS ONF   ON ONF.ID_NOTA = CNF.ID_NOTA
            WHERE  PAD.IDENTIFICADOR_CLIENTES = @IdentificadorCliente
            ORDER BY ONF.COD_OCORRENCIAS DESC;";

            var resolve = await conn.QueryFirstOrDefaultAsync(
                sqlResolve,
                new { IdentificadorCliente = identificadorCliente },
                transaction: (SqlTransaction)tx
            );

            codOcorrencias = (int?)resolve?.CodOcorrencias;
            idConhecimentos = (long?)resolve?.IdConhecimentos;
            dataAgendamento = (DateTime?)resolve?.DataAgendamento;
            int? codConhecimentos = (int?)resolve?.CodConhecimentos;


            if (codOcorrencias is null)
            {
                if (codConhecimentos is null)
                {
                    nexus.AddFailureMessage("Não há COD_CONHECIMENTOS associado ao Identificador informado.");
                    await tx.RollbackAsync(ct);
                    return nexus;
                }

                var cmdCriar = new CommandDefinition(
                    "NewSitex.dbo.PR_GERAR_OCORRENCIAS_AGENDAMENTO_AUTOMATICO_CTE",
                    new { COD_CONHECIMENTOS_CTE = codConhecimentos },
                    transaction: (SqlTransaction)tx,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct
                );
                await conn.ExecuteAsync(cmdCriar);

                var resolve2 = await conn.QueryFirstOrDefaultAsync(
                    sqlResolve,
                    new { IdentificadorCliente = identificadorCliente },
                    transaction: (SqlTransaction)tx
                );

                codOcorrencias = (int?)resolve2?.CodOcorrencias;
                dataAgendamento = (DateTime?)resolve2?.DataAgendamento;

                if (codOcorrencias is null)
                {
                    nexus.AddFailureMessage("Falha ao criar a ocorrência 91 para o conhecimento informado.");
                    await tx.RollbackAsync(ct);
                    return nexus;
                }
            }

            var basePath = Environment.GetEnvironmentVariable("TRATATIVAS_BASE_PATH")
                            ?? @"\\arquivos.tragetta.com.br\DESENVOLVIMENTO$\Repository\TratativasOcorrencias";

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
            VALUES
                (@COD_OCORRENCIAS, GETDATE(), GETDATE(), @DS, @USU, @FLV, @COD_TIPO);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

            var codTratativa = await conn.ExecuteScalarAsync<int>(
                sqlInsertTrat,
                new
                {
                    COD_OCORRENCIAS = codOcorrencias,
                    DS = DS_TRATATIVA_VOUCHER,
                    USU = COD_INCLUSAO_USUARIOS_VOUCHER,
                    FLV = FL_VOUCHER,
                    COD_TIPO = COD_TIPO_ANEXOS_VOUCHER
                },
                transaction: (SqlTransaction)tx
            );

            const string sqlInsertArq = @"
            INSERT INTO NewSitex.dbo.TB_OCORRENCIAS_TRATATIVAS_ARQUIVOS
                (COD_OCORRENCIAS, DT_INCLUSAO_OCORRENCIAS_TRATATIVAS, CAMINHO_OCORRENCIAS_TRATATIVAS_ARQUIVOS)
            VALUES
                (@COD_OCORRENCIAS, GETDATE(), @CAMINHO);";

            await conn.ExecuteAsync(
                sqlInsertArq,
                new { COD_OCORRENCIAS = codOcorrencias, CAMINHO = diskPath },
                transaction: (SqlTransaction)tx
            );

            await tx.CommitAsync(ct);

            _logger.LogInformation("CreateVoucherTratativaAsync -> Ocorrência={oc} tratativa gravada e arquivo anexado em {path}", codOcorrencias, diskPath);
            nexus.AddData(true);
            nexus.AddDefaultSuccessMessage();
            return nexus;
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync(ct);
            _logger.LogError(ex, "CreateVoucherTratativaAsync -> Erro ao gravar tratativa/arquivo. Disparando e-mail de fallback.");

            try
            {
                var notasResult = await GetNotasConhecimento(identificadorCliente, ct);
                var notas = notasResult.ResultData?.NotasFiscais ?? new List<NotaFiscalOutputModel>();
                var nomeRecebedor = notas.FirstOrDefault()?.NomeRecebedor ?? "Recebedor";
                var nomeFornecedor = notas.FirstOrDefault()?.NomeFornecedor ?? "Fornecedor";

                var dataFmt = dataAgendamento.HasValue ? dataAgendamento.Value.ToString("dd/MM/yyyy") : null;
                var assunto = dataFmt is null
                    ? $"Erro ao anexar VOUCHER - {nomeFornecedor} - solicitação de agendamento {nomeRecebedor}"
                    : $"Erro ao anexar VOUCHER - {nomeFornecedor} - solicitação de agendamento {nomeRecebedor} para {dataFmt}";

                var htmlTabela = EmailTemplateBuilder.BuildMensagemComTabelaNFs(notas);
                var htmlBody = $@"<p>O RPA não conseguiu anexar o VOUCHER e vincular à ocorrência {(codOcorrencias?.ToString() ?? "N/D")}, sendo necessário cadastrar de forma manual no TMS.</p>{htmlTabela}";

                var destinatarios = await GetDestinatariosEmailAsync(
                    notas.FirstOrDefault()?.CodClientesFornecedor,
                    notas.FirstOrDefault()?.CodClientesRecebedor,
                    ct
                ) ?? new List<ClientesRegrasAgendamentoEmail>();

                var emails = destinatarios
                    .Select(d => d.Email?.Trim())
                    .Where(e => !string.IsNullOrWhiteSpace(e) && e.Contains("@tragetta", StringComparison.OrdinalIgnoreCase))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                byte[] arquivoBytes;
                await using (var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms, ct);
                    arquivoBytes = ms.ToArray();
                }

                var model = new EmailPostFixInputModel
                {
                    DsAssunto = assunto,
                    DsMensagem = htmlBody,
                    FlMensagemHtml = true,
                    DestinatariosJson = Newtonsoft.Json.JsonConvert.SerializeObject(emails),
                    NmArquivo = Path.GetFileName(file.FileName),
                    MimeType = string.IsNullOrWhiteSpace(file.ContentType) ? "application/pdf" : file.ContentType,
                    Arquivo = arquivoBytes,
                    EmailRemetente = "naoresponda@tragetta.srv.br"
                };

                await EnviarEmailPostfixAsync(model, ct);
            }
            catch (Exception mailEx)
            {
                _logger.LogError(mailEx, "CreateVoucherTratativaAsync -> Falha também ao enviar e-mail de fallback.");
            }

            var fail = new NexusResult<bool>();
            fail.AddFailureMessage("Falha ao anexar VOUCHER e salvar tratativa. Um e-mail foi enviado para atendimento.");
            return fail;
        }
    }





}
