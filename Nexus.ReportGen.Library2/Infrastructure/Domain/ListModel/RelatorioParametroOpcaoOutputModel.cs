using Nexus.Framework.Data.Attributes;

namespace Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

/// <summary>
/// Modelo de saída para consultas de RelatorioParametroOpcao
/// </summary>
public class RelatorioParametroOpcaoOutputModel
{
        [NexusParameter("COD_OPCAO")]
        public int? CodOpcao { get; set; }

        [NexusParameter("COD_PARAMETRO")]
        public int? CodParametro { get; set; }

        [NexusParameter("VL_OPCAO")]
        public string VlOpcao { get; set; }

        [NexusParameter("DS_OPCAO")]
        public string DsOpcao { get; set; }

        [NexusParameter("NR_ORDEM")]
        public int? NrOrdem { get; set; }

        [NexusParameter("FL_ATIVO")]
        public bool? FlAtivo { get; set; }

        [NexusParameter("COD_USUARIOS")]
        public int? CodUsuarios { get; set; }

}
