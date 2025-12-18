using Nexus.Framework.Data.Attributes;

namespace Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

/// <summary>
/// Modelo de saída para consultas de StatusProcessamento
/// </summary>
public class StatusProcessamentoOutputModel
{
        [NexusParameter("COD_STATUS_PROCESSAMENTO")]
        public int? CodStatusProcessamento { get; set; }

        [NexusParameter("DS_STATUS")]
        public string DsStatus { get; set; }

        [NexusParameter("SIT_ATIVO")]
        public bool? SitAtivo { get; set; }

        [NexusParameter("COD_USUARIOS")]
        public int? CodUsuarios { get; set; }

}
