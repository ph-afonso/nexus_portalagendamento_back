using Nexus.Framework.Data.Attributes;

namespace Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

/// <summary>
/// Modelo de saída para consultas de RelatorioLog
/// </summary>
public class RelatorioLogOutputModel
{
        [NexusParameter("COD_LOG")]
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

}
