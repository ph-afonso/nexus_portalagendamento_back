using System.ComponentModel.DataAnnotations;
using System.Data;
using Nexus.Framework.Data.Attributes;
using Nexus.Framework.Data.Model.Input;

namespace Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;

/// <summary>
/// Modelo de entrada para operações de RelatorioModeloPermissao
/// </summary>
public class RelatorioModeloPermissaoInputModel : StoredProcedureInputModel
{
        [NexusParameter("COD_PERMISSAO", direction: ParameterDirection.InputOutput)]
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

        /// <summary>
        /// Gets or sets the operation identifier associated with the current context.
        /// </summary>
        [NexusParameter("OPERACAO")]
        [Required]
        public string Operacao { get; set; } = string.Empty;

        public RelatorioModeloPermissaoInputModel()
        {
            _commandName = "APP_TB_RELATORIO_MODELO_PERMISSAO";
        }
    }
