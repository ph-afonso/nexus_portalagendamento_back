using System.ComponentModel.DataAnnotations;
using System.Data;
using Nexus.Framework.Data.Attributes;
using Nexus.Framework.Data.Model.Input;

namespace Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;

/// <summary>
/// Modelo de entrada para operações de RelatorioSolicitacaoDestino
/// </summary>
public class RelatorioSolicitacaoDestinoInputModel : StoredProcedureInputModel
{
        [NexusParameter("COD_SOLICITACAO_DESTINO", direction: ParameterDirection.InputOutput)]
        public long? CodSolicitacaoDestino { get; set; }

        [NexusParameter("COD_RELATORIO_SOLICITACAO")]
        public long? CodRelatorioSolicitacao { get; set; }

        [NexusParameter("COD_DESTINO_TIPO")]
        public int? CodDestinoTipo { get; set; }

        [NexusParameter("DS_CONFIGURACAO_JSON")]
        public string DsConfiguracaoJson { get; set; }

        [NexusParameter("FL_ANEXAR_ARQUIVO")]
        public bool? FlAnexarArquivo { get; set; }

        [NexusParameter("COD_STATUS_PROCESSAMENTO")]
        public int? CodStatusProcessamento { get; set; }

        [NexusParameter("DT_ENVIO")]
        public DateTime? DtEnvio { get; set; }

        [NexusParameter("MSG_ERRO")]
        public string MsgErro { get; set; }

        [NexusParameter("QT_TENTATIVAS")]
        public int? QtTentativas { get; set; }

        [NexusParameter("COD_USUARIOS")]
        public int? CodUsuarios { get; set; }

        /// <summary>
        /// Gets or sets the operation identifier associated with the current context.
        /// </summary>
        [NexusParameter("OPERACAO")]
        [Required]
        public string Operacao { get; set; } = string.Empty;

        public RelatorioSolicitacaoDestinoInputModel()
        {
            _commandName = "APP_TB_RELATORIO_SOLICITACAO_DESTINO";
        }
    }
