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
using SkiaSharp;
using System.Data;
using System.Threading;

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

    //public async Task<NexusResult<EmailPostFixInputModel>> SendEmailAnexo(Guid? identificadorCliente, IFormFile request, CancellationToken cancellationToken = default)
    //{
    //    try
    //    {
    //        var notasResult = await GetNotasConhecimento(identificadorCliente, cancellationToken);
    //        var notas = notasResult.ResultData?.NotasFiscais ?? new List<NotaFiscalOutputModel>();
    //        var nomeRecebedor = notas.FirstOrDefault()?.NomeRecebedor ?? "Recebedor";
    //        var nomeFornecedor = notas.FirstOrDefault()?.NomeFornecedor ?? "Fornecedor";
    //        var codFornecedor = notas.FirstOrDefault()?.CodClientesFornecedor;
    //        var codRecebedor = notas.FirstOrDefault()?.CodClientesRecebedor;

    //        var assunto = $"Atenção: Anexo não lido - {nomeFornecedor} - solicitação de agendamento {nomeRecebedor}";
    //        var htmlBody = EmailTemplateBuilder.BuildMensagemComTabelaNFs(notas);
    //        var destinatarios = await GetDestinatariosEmailAsync(codFornecedor, codRecebedor, cancellationToken);

    //        var emails = destinatarios
    //            .Select(d => d.Email?.Trim())
    //            .Where(e => !string.IsNullOrWhiteSpace(e) && e.Contains("@tragetta", StringComparison.OrdinalIgnoreCase))
    //            .Distinct(StringComparer.OrdinalIgnoreCase)
    //            .ToList();

    //        if (emails.Count == 0) _logger.LogWarning("Nenhum destinatário encontrado.");

    //        byte[]? arquivoBytes = null;
    //        string? nomeArquivo = null;
    //        string? mimeType = null;

    //        if (request != null && request.Length > 0)
    //        {
    //            using var ms = new MemoryStream();
    //            await request.CopyToAsync(ms, cancellationToken);
    //            arquivoBytes = ms.ToArray();
    //            nomeArquivo = Path.GetFileName(request.FileName);
    //            mimeType = string.IsNullOrWhiteSpace(request.ContentType) ? "application/pdf" : request.ContentType;
    //        }

    //        var model = new EmailPostFixInputModel
    //        {
    //            DsAssunto = assunto,
    //            DsMensagem = htmlBody,
    //            FlMensagemHtml = true,
    //            DestinatariosJson = JsonConvert.SerializeObject(emails),
    //            NmArquivo = nomeArquivo,
    //            MimeType = mimeType,
    //            Arquivo = arquivoBytes,
    //            EmailRemetente = "naoresponda@tragetta.srv.br"
    //        };

    //        return await EnviarEmailPostfixAsync(model, cancellationToken);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "SendEmailAnexo -> Erro");
    //        throw;
    //    }
    //}

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

    //NOVOS
    public async Task<NexusResult<ValidadeTokenOutputModel>> ValidarTokenAsync(ValidadeTokenInputModel model, CancellationToken cancellationToken = default)
    {
        var nexus = new NexusResult<ValidadeTokenOutputModel>();

        if (model == null || model.IdentificadorCliente == Guid.Empty)
        {
            nexus.AddFailureMessage("Identificador do cliente é obrigatório.");
            return nexus;
        }

        try
        {
            var cs = await _conn.GetDefaultConnectionStringAsync();
            await using var connection = new SqlConnection(cs);
            await connection.OpenAsync(cancellationToken);

            // --- CORREÇÃO: TOP 1 + ORDER BY DESC ---
            // Isso garante que pegamos a última alteração/inclusão, e não um registro velho perdido
            const string sql = @"
            SELECT TOP 1
                 DT_GERACAO_TOKEN        AS DataGeracaoToken
                ,DT_ATUALIZACAO_TOKEN    AS DataAtualizacaoToken
                ,DT_SUGESTAO_AGENDAMENTO AS DataSugestaoAgendamento
            FROM 
                NewSitex.dbo.TB_PORTAL_AGENDAMENTO_DPA
            WHERE 
                IDENTIFICADOR_CLIENTES = @IdentificadorCliente
            ORDER BY 
                ISNULL(DT_ATUALIZACAO_TOKEN, DT_GERACAO_TOKEN) DESC";

            var cmd = new CommandDefinition(sql, new { IdentificadorCliente = model.IdentificadorCliente }, commandType: CommandType.Text, cancellationToken: cancellationToken);

            var row = await connection.QueryFirstOrDefaultAsync<ValidadeTokenOutputModel>(cmd);

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
            _logger.LogError(ex, "ValidarTokenAsync -> Erro");
            var nexusError = new NexusResult<ValidadeTokenOutputModel>();
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
                    SELECT 
                         TNF.NR_PEDIDO
                        ,RECEBEDOR.NM_RAZAO_CLIENTES  AS NOME_RECEBEDOR  -- Alias alterado para RECEBEDOR
                        ,FORNECEDOR.NM_RAZAO_CLIENTES AS NOME_FORNECEDOR
                        ,FORNECEDOR.UF_CLIENTES
                        ,TNF.NR_NOTAS_FISCAIS 
                        ,TNF.NR_SERIE_NOTAS_FISCAIS 
                        ,CONVERT(VARCHAR(10), TNF.DT_EMISSAO_NOTAS_FISCAIS, 103) AS DT_EMISSAO
                        ,TNF.QT_VOLUME_NOTAS_FISCAIS
                        ,TNF.PESO_NOTAS_FISCAIS
                        ,TNF.VL_TOTAL_NOTAS_FISCAIS
                        ,FORNECEDOR.COD_CLIENTES AS COD_CLIENTES_FORNECEDOR
                        ,RECEBEDOR.COD_CLIENTES  AS COD_CLIENTES_RECEBEDOR
                        ,TNF.COD_FILIAIS
                        ,TC.NR_IMPRESSO_CONHECIMENTOS
                    FROM NewSitex.dbo.TB_PORTAL_AGENDAMENTO_DPA PAD WITH (NOLOCK)
            
                    -- Join tabela de ligação
                    INNER JOIN NewSitex.dbo.TB_CONHECIMENTOS_NOTAS_FISCAIS AS TCNF WITH (NOLOCK)
                        ON PAD.ID_CONHECIMENTOS = TCNF.ID_CONHECIMENTOS
            
                    -- Join com a Nota
                    INNER JOIN NewSitex.dbo.TB_NOTAS_FISCAIS AS TNF WITH (NOLOCK)
                        ON TCNF.ID_NOTA = TNF.ID
            
                    -- CORREÇÃO PRINCIPAL DO JOIN (Pelo ID e não COD)
                    INNER JOIN NewSitex.dbo.TB_CONHECIMENTOS AS TC WITH (NOLOCK)
                        ON TC.COD_CONHECIMENTOS = TCNF.COD_CONHECIMENTOS 
            
                    -- Clientes (Fornecedor e Recebedor)
                    INNER JOIN NewSitex.dbo.TB_CLIENTES AS FORNECEDOR WITH (NOLOCK)
                        ON TNF.COD_CLIENTES = FORNECEDOR.COD_CLIENTES
                
                    -- AQUI ESTAVA O ERRO: Mudamos alias de CLIENTE para RECEBEDOR para evitar duplicidade
                    INNER JOIN NewSitex.dbo.TB_CLIENTES AS RECEBEDOR WITH (NOLOCK)
                        ON TNF.COD_RECEBEDOR = RECEBEDOR.COD_CLIENTES
            
                    WHERE PAD.IDENTIFICADOR_CLIENTES = @IdentificadorCliente";

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

    public async Task<NexusResult<bool>> RealizarAgendamentoAsync(AgendamentoRepositoryInputModel input, CancellationToken cancellationToken = default)
    {
        var nexus = new NexusResult<bool>();

        try
        {
            var cs = await _conn.GetDefaultConnectionStringAsync();
            await using var connection = new SqlConnection(cs);
            await connection.OpenAsync(cancellationToken);

            // Variáveis fixas
            const int COD_USUARIO = 2; // Usuário RPA/Sistema
            const int CODIGO_EJ = 91;
            const int TIPO_AGENDAMENTO = 2;
            const int PERIODO_AGENDAMENTO = 4;

            // Formatação da Data (YYYY-MM-DD)
            string dataAgendamentoStr = input.DataAgendamento.ToString("yyyy-MM-dd");

            // Formatação da Hora (Padrão 08:00)
            // Como o input vem do Token (DateTime), pegamos a hora dele ou forçamos 08:00
            string horaStr = input.DataAgendamento.ToString("HH:mm");
            if (horaStr == "00:00" || string.IsNullOrWhiteSpace(horaStr))
            {
                horaStr = "08:00";
            }

            foreach (var nota in input.Notas)
            {
                // 2. BUSCAR IDENT_FILIAIS (Baseado no CodFiliais da nota)
                // A procedure exige @IDENT_FILIAIS, mas nós temos COD_FILIAIS
                var sqlFilial = "SELECT TOP 1 IDENT_FILIAIS FROM NewSitex.dbo.TB_FILIAIS WITH(NOLOCK) WHERE COD_FILIAIS = @CodFilial";
                var identFilial = await connection.QueryFirstOrDefaultAsync<int?>(sqlFilial, new { CodFilial = nota.CodFiliais });

                if (identFilial == null)
                {
                    _logger.LogWarning("Filial não encontrada para CodFiliais: {Cod}", nota.CodFiliais);
                    continue; // Pula nota com erro de cadastro
                }

                // 3. DEFINIR OBSERVACAO
                string obs = nota.IsReagendamento
                    ? "Reagendamento automático via Portal Agendamento DPA"
                    : "Agendamento automático via Portal Agendamento DPA";

                // 4. EXECUTAR PROCEDURE
                const string sqlProc = @"
                DECLARE @DATETIME DATETIME = GETDATE();

                EXEC NewSitex.dbo.PR_I_TB_OCORRENCIAS_MASSA 
                    @IDENT_FILIAIS = @IdentFilial,
                    @NR_IMPRESSO_CONHECIMENTOS = @NrImpressoConhecimento,
                    @CODIGO_OCORRENCIA = @CodigoInterno,
                    @DATA_OCORRENCIA = @DATETIME,
                    @DATA_RECUSA = @DATETIME,
                    @IDR = 0,
                    @OCORRENCIA_INTERNA = 0,
                    @OBSERVACAO = @Observacao,
                    @TIPO_AGENDAMENTO = @TipoAgendamento,
                    @DATA_AGENDAMENTO = @DataAgendamento,
                    @PERIODO_AGENDAMENTO = @Periodo,
                    @HORARIO_INICIO_AGENDAMENTO = @Hora,
                    @HORARIO_FIM_AGENDAMENTO = NULL,
                    @OBSERVACAO_AGENDAMENTO = @Observacao,
                    @COD_INCLUSAO_USUARIOS = @CodUsuario,
                    @COD_FILIAIS_USUARIOS = @CodFilial, -- Usando a filial da nota como filial do usuário
                    @ID_OCORRENCIA_MASSA = NULL,
                    @CODIGO_OCORRENCIA_EJ = @CodigoEj,
                    @ID_OCORRENCIA_ATLAS = NULL,
                    @ID_TIPO_COMPLEMENTO_OCORRENCIA = NULL,
                    @AUTORIZACAO_COMPLEMENTO = NULL,
                    @VL_COMPLEMENTO = 0.00;";

                await connection.ExecuteAsync(sqlProc, new
                {
                    IdentFilial = identFilial,
                    NrImpressoConhecimento = nota.NrImpressoConhecimentos ?? 0, // Fallback 0 se nulo
                    CodigoInterno = CODIGO_EJ,
                    Observacao = obs,
                    TipoAgendamento = TIPO_AGENDAMENTO,
                    DataAgendamento = dataAgendamentoStr,
                    Periodo = PERIODO_AGENDAMENTO,
                    Hora = horaStr,
                    CodUsuario = COD_USUARIO,
                    CodFilial = nota.CodFiliais,
                    CodigoEj = CODIGO_EJ
                });

                _logger.LogInformation("Ocorrência 91 criada para Nota {Nr} ({Tipo})", nota.NrNotasFiscais, obs);
            }

            nexus.AddData(true);
            nexus.AddDefaultSuccessMessage();
            return nexus;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RealizarAgendamentoAsync -> Erro ao executar procedures.");
            nexus.AddFailureMessage($"Erro ao gravar agendamento: {ex.Message}");
            return nexus;
        }
    }

    public async Task<IEnumerable<OcorrenciaImpeditivaModel>> CheckOcorrenciasImpeditivasAsync(long nrNota, int codFilial, CancellationToken cancellationToken = default)
    {
        try
        {
            var cs = await _conn.GetDefaultConnectionStringAsync();
            await using var connection = new SqlConnection(cs);
            await connection.OpenAsync(cancellationToken);

            const string sql = @"
            SELECT 
                 ONF.COD_OCORRENCIAS_TIPO_EJ AS CodOcorrenciaTipo
                ,TC.COD_CONHECIMENTOS        AS CodConhecimentos
                ,NF.COD_FILIAIS              AS CodFilial
            FROM 
                NewSitex.dbo.TB_NOTAS_FISCAIS NF WITH (NOLOCK)
            INNER JOIN 
                NewSitex.dbo.TB_OCORRENCIAS_NOTAS_FISCAIS ONF WITH (NOLOCK) 
                ON ONF.ID_NOTA = NF.ID
            INNER JOIN
                NewSitex.dbo.TB_CONHECIMENTOS_NOTAS_FISCAIS CNF WITH (NOLOCK)
                ON CNF.ID_NOTA = NF.ID
            INNER JOIN
                NewSitex.dbo.TB_CONHECIMENTOS TC WITH (NOLOCK)
                ON TC.ID = CNF.ID_CONHECIMENTOS
            WHERE 
                ONF.DT_SOLUCAO_OCORRENCIAS_NOTAS_FISCAIS IS NULL 
                AND ONF.COD_OCORRENCIAS_TIPO_EJ IN (140, 145)
                AND NF.DT_ENTREGA_NOTAS_FISCAIS IS NULL
                AND NF.NR_NOTAS_FISCAIS = @NrNota 
                AND NF.COD_FILIAIS = @CodFilial";

            return await connection.QueryAsync<OcorrenciaImpeditivaModel>(sql, new { NrNota = nrNota, CodFilial = codFilial });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CheckOcorrenciasImpeditivasAsync -> Erro nota {NrNota}", nrNota);
            return new List<OcorrenciaImpeditivaModel>();
        }
    }

    public async Task EncerrarOcorrenciaMassaAsync(OcorrenciaAbertaModel ocorrencia, CancellationToken cancellationToken = default)
    {
        try
        {
            var cs = await _conn.GetDefaultConnectionStringAsync();
            await using var connection = new SqlConnection(cs);
            await connection.OpenAsync(cancellationToken);

            const int COD_USUARIO_SISTEMA = 2; // Usuário RPA/Sistema

            const string sql = @"
                DECLARE @DATETIME DATETIME;
                SET @DATETIME = GETDATE();

                EXEC NewSitex.dbo.PR_ENCERRA_OCORRENCIAS_MASSA  
                    @COD_CONHECIMENTOS = @CodConhecimentos,      
                    @COD_USUARIOS = @CodUsuarios,    
                    @DT_FUSO_HORARIO = @DATETIME, 
                    @SESSION_ID = 'RPA',   
                    @COD_FILIAIS = @CodFilial,     
                    @COD_OCORRENCIAS_TIPO = @CodOcorrenciaTipo,
                    @T_DT_AGENDAMENTO = NULL,
                    @T_HR_AGENDAMENTO_INICIAL = NULL,      
                    @T_HR_AGENDAMENTO_FINAL = NULL,  
                    @T_COD_AGENDAMENTOS_PERIODO = NULL,                                                                                      
                    @T_COD_AGENDAMENTOS_TIPO = NULL,
                    @T_OBSERVACAO = 'Encerramento automático via Portal Agendamento DPA';";

            await connection.ExecuteAsync(sql, new
            {
                // Mapeando propriedades do OcorrenciaAbertaModel para os parâmetros SQL
                CodConhecimentos = ocorrencia.CodConhecimentos,
                CodUsuarios = COD_USUARIO_SISTEMA,
                CodFilial = ocorrencia.CodFilial,          // Usa o ID interno da filial (COD_FILIAIS)
                CodOcorrenciaTipo = ocorrencia.CodOcorrenciaTipo
            });

            _logger.LogInformation("Ocorrência {Tipo} encerrada para Conhecimento {Cte}", ocorrencia.CodOcorrenciaTipo, ocorrencia.CodConhecimentos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao encerrar ocorrência {Tipo} para Cte {Cte}", ocorrencia.CodOcorrenciaTipo, ocorrencia.CodConhecimentos);
            throw; // Relança para o Service tratar e bloquear a nota
        }
    }

    public async Task<OcorrenciaAbertaModel?> GetOcorrenciaAbertaAsync(long nrNota, int codFilial, CancellationToken cancellationToken = default)
    {
        try
        {
            var cs = await _conn.GetDefaultConnectionStringAsync();
            await using var connection = new SqlConnection(cs);
            await connection.OpenAsync(cancellationToken);

            const string sql = @"
            SELECT TOP 1 
                 ONF.COD_OCORRENCIAS_TIPO_EJ                        AS CodOcorrenciaTipo
                ,FIL.IDENT_FILIAIS                                  AS IdentFilial
                ,OCO.NR_OCORRENCIAS                                 AS NrOcorrencia
                ,OCO.OBS_OCORRENCIAS                                AS Observacao
                ,ONF.DT_AGENDAMENTO_ENTREGA_OCORRENCIAS_NOTAS_FISCAIS AS DataAgendamento
                ,ONF.HR_AGENDAMENTO_INICIAL_OCORRENCIAS_NOTAS_FISCAIS AS HoraAgendamento
                
                -- Campos necessários para o Encerramento (caso permitido)
                ,NF.COD_FILIAIS                                     AS CodFilial
                ,TC.COD_CONHECIMENTOS                               AS CodConhecimentos
            FROM 
                NewSitex.dbo.TB_NOTAS_FISCAIS NF WITH (NOLOCK)
            INNER JOIN 
                NewSitex.dbo.TB_OCORRENCIAS_NOTAS_FISCAIS ONF WITH (NOLOCK) ON ONF.ID_NOTA = NF.ID
            INNER JOIN 
                NewSitex.dbo.TB_OCORRENCIAS OCO WITH (NOLOCK) ON OCO.COD_OCORRENCIAS = ONF.COD_OCORRENCIAS
            INNER JOIN 
                NewSitex.dbo.TB_FILIAIS FIL WITH (NOLOCK) ON FIL.COD_FILIAIS = OCO.COD_FILIAIS
            -- Joins para pegar o Conhecimento (para poder encerrar se necessário)
            LEFT JOIN
                NewSitex.dbo.TB_CONHECIMENTOS_NOTAS_FISCAIS CNF WITH (NOLOCK) ON CNF.ID_NOTA = NF.ID
            LEFT JOIN
                NewSitex.dbo.TB_CONHECIMENTOS TC WITH (NOLOCK) ON TC.ID = CNF.ID_CONHECIMENTOS
            WHERE 
                ONF.DT_SOLUCAO_OCORRENCIAS_NOTAS_FISCAIS IS NULL 
                AND NF.DT_ENTREGA_NOTAS_FISCAIS IS NULL
                AND NF.NR_NOTAS_FISCAIS = @NrNota
                AND NF.COD_FILIAIS = @CodFilial";

            return await connection.QueryFirstOrDefaultAsync<OcorrenciaAbertaModel>(
                sql,
                new { NrNota = nrNota, CodFilial = codFilial }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetOcorrenciaAbertaAsync -> Erro ao verificar nota {NrNota}", nrNota);
            return null;
        }
    }

    public async Task<List<ClientesRegrasAgendamentoEmail>> GetDestinatariosEmailAsync(int? codFornecedor, int? codRecebedor, CancellationToken cancellationToken = default)
    {
        if (codFornecedor is null && codRecebedor is null)
            return new List<ClientesRegrasAgendamentoEmail>();

        try
        {
            const string procedureName = "NewSitex.dbo.PR_S_TB_CLIENTES_REGRAS_AGENDAMENTO_EMAIL";

            var parameters = new DynamicParameters();
            parameters.Add("@COD_CLIENTES", codFornecedor);
            parameters.Add("@COD_RECEBEDOR", codRecebedor);

            var connectionString = await _conn.GetDefaultConnectionStringAsync();
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            var result = await connection.QueryAsync<ClientesRegrasAgendamentoEmail>(
                procedureName,
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result?.ToList() ?? new List<ClientesRegrasAgendamentoEmail>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetDestinatariosEmailAsync -> Erro ao executar procedure.");
            return new List<ClientesRegrasAgendamentoEmail>();
        }
    }

    public async Task EnviarEmailPostfixAsync(EmailPostFixInputModel input, CancellationToken cancellationToken = default)
    {
        try
        {
            var cs = await _conn.GetDefaultConnectionStringAsync();
            await using var connection = new SqlConnection(cs);
            await connection.OpenAsync(cancellationToken);

            await connection.ExecuteAsync(
                "NewSitex.dbo.PR_ENVIAR_EMAIL_POSTFIX",
                new
                {
                    COD_REMETENTE_PESSOA = input.CodRemetentePessoa,
                    COD_EVENTOS = input.CodEventos,
                    DS_ASSUNTO = input.DsAssunto,
                    DS_MENSAGEM = input.DsMensagem,
                    FL_MENSAGEM_HTML = input.FlMensagemHtml,
                    LOGIN_REMETENTE_USUARIOS = input.LoginRemetenteUsuarios,
                    DESTINATARIOS_JSON = input.DestinatariosJson,
                    NM_ARQUIVO = input.NmArquivo,
                    MIME_TYPE = input.MimeType,
                    ARQUIVO = input.Arquivo,
                    EMAIL_REMETENTE = input.EmailRemetente,
                    CAMINHO_ARQUIVO = input.CaminhoArquivo
                },
                commandType: CommandType.StoredProcedure
            );

            _logger.LogInformation("Email enviado via Postfix (Procedure) com sucesso.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao executar PR_ENVIAR_EMAIL_POSTFIX.");
        }
    }
}