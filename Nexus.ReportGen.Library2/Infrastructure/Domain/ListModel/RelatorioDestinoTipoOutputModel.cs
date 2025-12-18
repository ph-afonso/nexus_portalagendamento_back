using Nexus.Framework.Data.Attributes;

namespace Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

/// <summary>
/// Modelo de saída para consultas de RelatorioDestinoTipo
/// </summary>
public class RelatorioDestinoTipoOutputModel
{
        [NexusParameter("COD_DESTINO_TIPO")]
        public int? CodDestinoTipo { get; set; }

        [NexusParameter("NM_TIPO")]
        public string NmTipo { get; set; }

        [NexusParameter("DS_TIPO")]
        public string DsTipo { get; set; }

        [NexusParameter("FL_ATIVO")]
        public bool? FlAtivo { get; set; }

        [NexusParameter("COD_USUARIOS")]
        public int? CodUsuarios { get; set; }

}
