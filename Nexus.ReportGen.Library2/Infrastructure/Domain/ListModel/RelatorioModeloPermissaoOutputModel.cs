using Nexus.Framework.Data.Attributes;

namespace Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

/// <summary>
/// Modelo de saída para consultas de RelatorioModeloPermissao
/// </summary>
public class RelatorioModeloPermissaoOutputModel
{
        [NexusParameter("COD_PERMISSAO")]
        public int? CodPermissao { get; set; }

        [NexusParameter("COD_RELATORIO_MODELO")]
        public int? CodRelatorioModelo { get; set; }

        [NexusParameter("COD_USUARIO")]
        public int? CodUsuario { get; set; }

        [NexusParameter("COD_PERFIL")]
        public int? CodPerfil { get; set; }

        [NexusParameter("COD_DEPARTAMENTO")]
        public int? CodDepartamento { get; set; }

        [NexusParameter("FL_PODE_EXECUTAR")]
        public bool? FlPodeExecutar { get; set; }

        [NexusParameter("FL_PODE_VISUALIZAR")]
        public bool? FlPodeVisualizar { get; set; }

        [NexusParameter("FL_PODE_BAIXAR")]
        public bool? FlPodeBaixar { get; set; }

        [NexusParameter("COD_USUARIOS")]
        public int? CodUsuarios { get; set; }

}
