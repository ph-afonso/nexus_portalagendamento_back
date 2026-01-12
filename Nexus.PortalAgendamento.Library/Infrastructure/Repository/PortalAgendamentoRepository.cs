using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Nexus.Framework.Common;
using Nexus.Framework.Data;
using Nexus.Framework.Data.Model.Result;
using Nexus.Framework.Data.Repository.Default;
using Nexus.Framework.Data.Repository.Interfaces;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.InputModel;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.ListModel;
using Nexus.PortalAgendamento.Library.Infrastructure.Repository.Interfaces;
using System.Data;

namespace Nexus.PortalAgendamento.Library.Infrastructure.Repository;

public class PortalAgendamentoRepository : ProcedureRepository, IPortalAgendamentoRepository
{
    private readonly IServiceBase _serviceBase;
    private readonly IConnectionStringProvider _conn;
    private readonly ILogger<PortalAgendamentoRepository> _log;

    public PortalAgendamentoRepository(
        IConnectionStringProvider connectionStringProvider,
        ILogger<PortalAgendamentoRepository> logger,
        IServiceBase serviceBase)
        : base(connectionStringProvider, logger)
    {
        _serviceBase = serviceBase ?? throw new ArgumentNullException(nameof(serviceBase));
        _conn = connectionStringProvider ?? throw new ArgumentNullException(nameof(connectionStringProvider));
        _log = logger ?? throw new ArgumentNullException(nameof(logger));
    }

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

            const string sql = @"
            SELECT TOP 1
                 DT_GERACAO_TOKEN        AS DataGeracaoToken
                ,DT_ATUALIZACAO_TOKEN    AS DataAtualizacaoToken
                ,DT_SUGESTAO_AGENDAMENTO AS DataSugestaoAgendamento
            FROM 
                NewSitex.dbo.TB_PORTAL_AGENDAMENTO_DPA WITH(NOLOCK)
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
            _log.LogError(ex, "ValidarTokenAsync -> Erro");
            nexus.AddFailureMessage($"Erro: {ex.Message}");
            return nexus;
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
                        ,RECEBEDOR.NM_RAZAO_CLIENTES  AS NOME_RECEBEDOR
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
                    INNER JOIN NewSitex.dbo.TB_CONHECIMENTOS_NOTAS_FISCAIS AS TCNF WITH (NOLOCK)
                        ON PAD.ID_CONHECIMENTOS = TCNF.ID_CONHECIMENTOS
                    INNER JOIN NewSitex.dbo.TB_NOTAS_FISCAIS AS TNF WITH (NOLOCK)
                        ON TCNF.ID_NOTA = TNF.ID
                    INNER JOIN NewSitex.dbo.TB_CONHECIMENTOS AS TC WITH (NOLOCK)
                        ON TC.COD_CONHECIMENTOS = TCNF.COD_CONHECIMENTOS 
                    INNER JOIN NewSitex.dbo.TB_CLIENTES AS FORNECEDOR WITH (NOLOCK)
                        ON TNF.COD_CLIENTES = FORNECEDOR.COD_CLIENTES
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
            _log.LogError(ex, "GetNotasConhecimento -> Erro");
            nexus.AddFailureMessage($"Erro: {ex.Message}");
            return nexus;
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
                 ONF.COD_OCORRENCIAS_TIPO_EJ        AS CodOcorrenciaTipo
                ,FIL.IDENT_FILIAIS                  AS IdentFilial
                ,OCO.NR_OCORRENCIAS                 AS NrOcorrencia
                ,OCO.OBS_OCORRENCIAS                AS Observacao
                ,ONF.DT_AGENDAMENTO_ENTREGA_OCORRENCIAS_NOTAS_FISCAIS AS DataAgendamento
                ,ONF.HR_AGENDAMENTO_INICIAL_OCORRENCIAS_NOTAS_FISCAIS AS HoraAgendamento
                ,NF.COD_FILIAIS                     AS CodFilial
                ,TC.COD_CONHECIMENTOS               AS CodConhecimentos
                ,OCO.COD_OCORRENCIAS                AS IdOcorrencia  
            FROM 
                NewSitex.dbo.TB_NOTAS_FISCAIS NF WITH (NOLOCK)
            INNER JOIN 
                NewSitex.dbo.TB_OCORRENCIAS_NOTAS_FISCAIS ONF WITH (NOLOCK) ON ONF.ID_NOTA = NF.ID
            INNER JOIN 
                NewSitex.dbo.TB_OCORRENCIAS OCO WITH (NOLOCK) ON OCO.COD_OCORRENCIAS = ONF.COD_OCORRENCIAS
            INNER JOIN 
                NewSitex.dbo.TB_FILIAIS FIL WITH (NOLOCK) ON FIL.COD_FILIAIS = OCO.COD_FILIAIS
            LEFT JOIN
                NewSitex.dbo.TB_CONHECIMENTOS_NOTAS_FISCAIS CNF WITH (NOLOCK) ON CNF.ID_NOTA = NF.ID
            LEFT JOIN
                NewSitex.dbo.TB_CONHECIMENTOS TC WITH (NOLOCK) ON TC.ID = CNF.ID_CONHECIMENTOS
            WHERE 
                ONF.DT_SOLUCAO_OCORRENCIAS_NOTAS_FISCAIS IS NULL 
                AND NF.DT_ENTREGA_NOTAS_FISCAIS IS NULL
                AND NF.NR_NOTAS_FISCAIS = @NrNota
                AND NF.COD_FILIAIS = @CodFilial
            ORDER BY OCO.COD_OCORRENCIAS DESC";

            return await connection.QueryFirstOrDefaultAsync<OcorrenciaAbertaModel>(sql, new { NrNota = nrNota, CodFilial = codFilial });
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "GetOcorrenciaAbertaAsync -> Erro Nota: {Nota}", nrNota);
            return null;
        }
    }

    public async Task EncerrarOcorrenciaMassaAsync(OcorrenciaAbertaModel ocorrencia, CancellationToken cancellationToken = default)
    {
        try
        {
            var cs = await _conn.GetDefaultConnectionStringAsync();
            await using var connection = new SqlConnection(cs);
            await connection.OpenAsync(cancellationToken);

            const string sql = @"
                DECLARE @DATETIME DATETIME = GETDATE();
                
                EXEC NewSitex.dbo.PR_ENCERRA_OCORRENCIAS_MASSA  
                    @COD_CONHECIMENTOS = @CodConhecimentos,       
                    @COD_USUARIOS = 2,     
                    @DT_FUSO_HORARIO = @DATETIME, 
                    @SESSION_ID = 'RPA',   
                    @COD_FILIAIS = @CodFilial,       
                    @COD_OCORRENCIAS_TIPO = @CodOcorrenciaTipo,
                    
                    -- Parâmetros Opcionais (DEVEM ser passados, mesmo que NULL)
                    @T_DT_AGENDAMENTO = @DataAgendamento,
                    @T_HR_AGENDAMENTO_INICIAL = @HoraInicial,       
                    @T_HR_AGENDAMENTO_FINAL = @HoraFinal,  
                    @T_COD_AGENDAMENTOS_PERIODO = @Periodo,                                                                        
                    @T_COD_AGENDAMENTOS_TIPO = @TipoAgendamento,
                    
                    @T_OBSERVACAO = 'Encerramento automático via Portal Agendamento DPA';";

            await connection.ExecuteAsync(sql, new
            {
                CodConhecimentos = ocorrencia.CodConhecimentos,
                CodFilial = ocorrencia.CodFilial,
                CodOcorrenciaTipo = ocorrencia.CodOcorrenciaTipo,

                DataAgendamento = (DateTime?)null,
                HoraInicial = (string?)null,
                HoraFinal = (string?)null,
                Periodo = (int?)null,
                TipoAgendamento = (int?)null
            });
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "EncerrarOcorrenciaMassaAsync -> Erro Id: {Id}", ocorrencia.IdOcorrencia);
            throw;
        }
    }

    // 4. AGENDAMENTO
    public async Task<NexusResult<bool>> RealizarAgendamentoAsync(AgendamentoRepositoryInputModel input, CancellationToken cancellationToken = default)
    {
        var nexus = new NexusResult<bool>();
        try
        {
            var cs = await _conn.GetDefaultConnectionStringAsync();
            await using var connection = new SqlConnection(cs);
            await connection.OpenAsync(cancellationToken);

            string dataAgendamentoStr = input.DataAgendamento.ToString("yyyy-MM-dd");
            string horaStr = input.DataAgendamento.ToString("HH:mm");
            if (horaStr == "00:00") horaStr = "08:00";

            var conhecimentosDistintos = input.Notas
                .Where(n => n.NrImpressoConhecimentos.HasValue)
                .GroupBy(n => new { n.CodFiliais, n.NrImpressoConhecimentos })
                .Select(g => g.First())
                .ToList();

            foreach (var nota in conhecimentosDistintos)
            {
                var sqlFilial = "SELECT TOP 1 IDENT_FILIAIS FROM NewSitex.dbo.TB_FILIAIS WITH(NOLOCK) WHERE COD_FILIAIS = @CodFilial";
                var identFilial = await connection.QueryFirstOrDefaultAsync<int?>(sqlFilial, new { CodFilial = nota.CodFiliais });

                if (identFilial == null) continue;

                string obs = nota.IsReagendamento
                    ? "Reagendamento automático via Portal Agendamento DPA"
                    : "Agendamento automático via Portal Agendamento DPA";

                const string sqlProc = @"
                DECLARE @DATETIME DATETIME = GETDATE();
                
                EXEC NewSitex.dbo.PR_I_TB_OCORRENCIAS_MASSA 
                    @IDENT_FILIAIS = @IdentFilial,
                    @NR_IMPRESSO_CONHECIMENTOS = @NrImpressoConhecimento,
                    @CODIGO_OCORRENCIA = 91,
                    @DATA_OCORRENCIA = @DATETIME,
                    @DATA_RECUSA = @DATETIME,
                    @IDR = 0,
                    @OCORRENCIA_INTERNA = 0,
                    @OBSERVACAO = @Observacao,
                    @TIPO_AGENDAMENTO = 2,
                    @DATA_AGENDAMENTO = @DataAgendamento,
                    @PERIODO_AGENDAMENTO = 4,
                    @HORARIO_INICIO_AGENDAMENTO = @Hora,
                    
                    @HORARIO_FIM_AGENDAMENTO = NULL,
                    @OBSERVACAO_AGENDAMENTO = @Observacao,
                    @COD_INCLUSAO_USUARIOS = 2,
                    @COD_FILIAIS_USUARIOS = @CodFilial,
                    
                    @ID_OCORRENCIA_MASSA = NULL,
                    @CODIGO_OCORRENCIA_EJ = 91,
                    
                    @ID_OCORRENCIA_ATLAS = NULL,
                    @ID_TIPO_COMPLEMENTO_OCORRENCIA = NULL,
                    @AUTORIZACAO_COMPLEMENTO = NULL,
                    @VL_COMPLEMENTO = 0.00;";

                await connection.ExecuteAsync(sqlProc, new
                {
                    IdentFilial = identFilial,
                    NrImpressoConhecimento = nota.NrImpressoConhecimentos ?? 0,
                    Observacao = obs,
                    DataAgendamento = dataAgendamentoStr,
                    Hora = horaStr,
                    CodFilial = nota.CodFiliais
                });
            }

            nexus.AddDefaultSuccessMessage();
            nexus.ResultData = true;
            return nexus;
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "RealizarAgendamentoAsync -> Erro");
            nexus.AddFailureMessage($"Erro: {ex.Message}");
            return nexus;
        }
    }

    public async Task<List<ClientesRegrasAgendamentoEmail>> GetDestinatariosEmailAsync(int? codFornecedor, int? codRecebedor, CancellationToken cancellationToken = default)
    {
        if (codFornecedor == null && codRecebedor == null) return new List<ClientesRegrasAgendamentoEmail>();

        try
        {
            var cs = await _conn.GetDefaultConnectionStringAsync();
            await using var connection = new SqlConnection(cs);
            await connection.OpenAsync(cancellationToken);

            var result = await connection.QueryAsync<ClientesRegrasAgendamentoEmail>(
                "NewSitex.dbo.PR_S_TB_CLIENTES_REGRAS_AGENDAMENTO_EMAIL",
                new { COD_CLIENTES = codFornecedor, COD_RECEBEDOR = codRecebedor },
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "GetDestinatariosEmailAsync -> Erro");
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

            await connection.ExecuteAsync("NewSitex.dbo.PR_ENVIAR_EMAIL_POSTFIX", new
            {
                COD_REMETENTE_PESSOA = input.CodRemetentePessoa,
                COD_EVENTOS = input.CodEventos,
                DS_ASSUNTO = input.DsAssunto,
                DS_MENSAGEM = input.DsMensagem,
                FL_MENSAGEM_HTML = input.FlMensagemHtml,
                LOGIN_REMETENTE_USUARIOS = input.LoginRemetenteUsuarios,
                DESTINATARIOS_JSON = input.DestinatariosJson,
                EMAIL_REMETENTE = input.EmailRemetente,
                NM_ARQUIVO = input.NmArquivo,
                MIME_TYPE = input.MimeType,
                ARQUIVO = input.Arquivo,
                CAMINHO_ARQUIVO = input.CaminhoArquivo
            }, commandType: CommandType.StoredProcedure);

            _log.LogInformation("EnviarEmailPostfixAsync -> Email enviado.");
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "EnviarEmailPostfixAsync -> Erro");
        }
    }

    public async Task<NexusResult<bool>> VincularAnexoOcorrenciaAsync(int codOcorrencia, string nomeArquivoOriginal, byte[] arquivoBytes, CancellationToken ct)
    {
        var nexus = new NexusResult<bool>();

        // Constantes
        const string DS_TRATATIVA_VOUCHER = "VOUCHER PORTAL CSDPA";
        const short COD_TIPO_ANEXOS_VOUCHER = 12;
        const bool FL_VOUCHER = true;
        const int COD_INCLUSAO_USUARIOS_VOUCHER = 2;

        var cs = await _conn.GetDefaultConnectionStringAsync();
        await using var conn = new SqlConnection(cs);
        await conn.OpenAsync(ct);
        await using var tx = await conn.BeginTransactionAsync(ct);

        try
        {
            var basePath = Environment.GetEnvironmentVariable("TRATATIVAS_BASE_PATH_REPO")
                            ?? @"\\arquivos.tragetta.com.br\DESENVOLVIMENTO$\Repository\TratativasOcorrencias";

            if (!Directory.Exists(basePath) && !basePath.StartsWith(@"\\"))
                Directory.CreateDirectory(basePath);

            var bucket = Guid.NewGuid().ToString("N");
            var dir = Path.Combine(basePath, bucket);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            var safeName = Path.GetFileName(nomeArquivoOriginal);
            var diskPath = Path.Combine(dir, safeName);

            await File.WriteAllBytesAsync(diskPath, arquivoBytes, ct);

            const string sqlScript = @"
            DECLARE @TabelaData TABLE (DataGravada DATETIME);
            DECLARE @DataFinal DATETIME;

            -- 1. Insert no Pai + OUTPUT para pegar a data que foi REALMENTE gravada
            INSERT INTO NewSitex.dbo.TB_OCORRENCIAS_TRATATIVAS
                (COD_OCORRENCIAS, DT_INCLUSAO_OCORRENCIAS_TRATATIVAS, DT_OCORRENCIAS_TRATATIVAS, 
                 DS_OCORRENCIAS_TRATATIVAS, COD_INCLUSAO_USUARIOS, FL_VOUCHER, COD_TIPO_ANEXOS)
            OUTPUT INSERTED.DT_INCLUSAO_OCORRENCIAS_TRATATIVAS INTO @TabelaData
            VALUES
                (@COD_OCORRENCIAS, GETDATE(), GETDATE(), 
                 @DS, @USU, @FLV, @COD_TIPO);

            -- 2. Recupera a data exata da tabela temporária
            SELECT TOP 1 @DataFinal = DataGravada FROM @TabelaData;

            -- 3. Insert no Filho usando a @DataFinal (Garante match de 100% com a FK)
            INSERT INTO NewSitex.dbo.TB_OCORRENCIAS_TRATATIVAS_ARQUIVOS
                (COD_OCORRENCIAS, DT_INCLUSAO_OCORRENCIAS_TRATATIVAS, CAMINHO_OCORRENCIAS_TRATATIVAS_ARQUIVOS)
            VALUES
                (@COD_OCORRENCIAS, @DataFinal, @CAMINHO);
        ";

            await conn.ExecuteAsync(sqlScript, new
            {
                COD_OCORRENCIAS = codOcorrencia,
                DS = DS_TRATATIVA_VOUCHER,
                USU = COD_INCLUSAO_USUARIOS_VOUCHER,
                FLV = FL_VOUCHER,
                COD_TIPO = COD_TIPO_ANEXOS_VOUCHER,
                CAMINHO = diskPath
            }, transaction: (SqlTransaction)tx);

            await tx.CommitAsync(ct);

            nexus.AddData(true);
            nexus.AddDefaultSuccessMessage();
            return nexus;
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync(ct);
            _logger.LogError(ex, "VincularAnexoOcorrenciaAsync -> Erro ID {Id}", codOcorrencia);
            nexus.AddFailureMessage($"Erro no Banco de Dados: {ex.Message}");
            return nexus;
        }
    }
}