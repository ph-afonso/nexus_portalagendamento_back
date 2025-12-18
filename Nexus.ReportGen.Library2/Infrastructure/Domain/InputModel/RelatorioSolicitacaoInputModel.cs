using System.ComponentModel.DataAnnotations;
using System.Data;
using Nexus.Framework.Data.Attributes;
using Nexus.Framework.Data.Model.Input;

namespace Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;

/// <summary>
/// Modelo de entrada para operações de RelatorioSolicitacao
/// </summary>
public class RelatorioSolicitacaoInputModel : StoredProcedureInputModel
{
        [NexusParameter("COD_RELATORIO_SOLICITACAO", direction: ParameterDirection.InputOutput)]
        public long? CodRelatorioSolicitacao { get; set; }

        [NexusParameter("COD_RELATORIO_MODELO")]
        public int? CodRelatorioModelo { get; set; }

        [NexusParameter("COD_USUARIO")]
        public int? CodUsuario { get; set; }

        [NexusParameter("DS_PARAMETROS")]
        public string DsParametros { get; set; }

        [NexusParameter("COD_STATUS_PROCESSAMENTO")]
        public int? CodStatusProcessamento { get; set; }

        [NexusParameter("DT_INICIO_PROCESSAMENTO")]
        public DateTime? DtInicioProcessamento { get; set; }

        [NexusParameter("DT_FIM_PROCESSAMENTO")]
        public DateTime? DtFimProcessamento { get; set; }

        [NexusParameter("URL_ARQUIVO")]
        public string UrlArquivo { get; set; }

        [NexusParameter("NM_ARQUIVO")]
        public string NmArquivo { get; set; }

        [NexusParameter("TP_ARQUIVO")]
        public string TpArquivo { get; set; }

        [NexusParameter("MSG_ERRO")]
        public string MsgErro { get; set; }

        [NexusParameter("QT_TENTATIVAS")]
        public int? QtTentativas { get; set; }

        [NexusParameter("FL_NOTIFICADO")]
        public bool? FlNotificado { get; set; }

        [NexusParameter("COD_JOB_EXTERNO")]
        public string CodJobExterno { get; set; }

        [NexusParameter("QT_REGISTROS_GERADOS")]
        public int? QtRegistrosGerados { get; set; }

        [NexusParameter("QT_TAMANHO_ARQUIVO_KB")]
        public int? QtTamanhoArquivoKb { get; set; }

        [NexusParameter("NM_PROCEDURE_AD_HOC")]
        public string NmProcedureAdHoc { get; set; }

        [NexusParameter("TP_ARQUIVO_SOLICITADO")]
        public string TpArquivoSolicitado { get; set; }

        [NexusParameter("DS_LAYOUT_SAIDA_JSON")]
        public string DsLayoutSaidaJson { get; set; }

        [NexusParameter("COD_USUARIOS")]
        public int? CodUsuarios { get; set; }

        /// <summary>
        /// Gets or sets the operation identifier associated with the current context.
        /// </summary>
        [NexusParameter("OPERACAO")]
        [Required]
        public string Operacao { get; set; } = string.Empty;

        public RelatorioSolicitacaoInputModel()
        {
            _commandName = "APP_TB_RELATORIO_SOLICITACAO";
        }
    }
