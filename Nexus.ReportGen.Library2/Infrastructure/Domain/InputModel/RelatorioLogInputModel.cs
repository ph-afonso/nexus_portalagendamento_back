using System.ComponentModel.DataAnnotations;
using System.Data;
using Nexus.Framework.Data.Attributes;
using Nexus.Framework.Data.Model.Input;

namespace Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;

/// <summary>
/// Modelo de entrada para operações de RelatorioLog
/// </summary>
public class RelatorioLogInputModel : StoredProcedureInputModel
{
        [NexusParameter("COD_LOG", direction: ParameterDirection.InputOutput)]
        public long? CodLog { get; set; }

        [NexusParameter("COD_RELATORIO_SOLICITACAO")]
        public long? CodRelatorioSolicitacao { get; set; }

        [NexusParameter("DT_LOG")]
        public DateTime? DtLog { get; set; }

        [NexusParameter("DS_EVENTO")]
        public string DsEvento { get; set; }

        [NexusParameter("DS_DETALHES")]
        public string DsDetalhes { get; set; }

        [NexusParameter("QT_TEMPO_EXECUCAO_MS")]
        public int? QtTempoExecucaoMs { get; set; }

        [NexusParameter("COD_USUARIOS")]
        public int? CodUsuarios { get; set; }

        /// <summary>
        /// Gets or sets the operation identifier associated with the current context.
        /// </summary>
        [NexusParameter("OPERACAO")]
        [Required]
        public string Operacao { get; set; } = string.Empty;

        public RelatorioLogInputModel()
        {
            _commandName = "APP_TB_RELATORIO_LOG";
        }
    }
