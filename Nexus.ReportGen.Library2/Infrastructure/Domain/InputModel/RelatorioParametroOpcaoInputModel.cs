using System.ComponentModel.DataAnnotations;
using System.Data;
using Nexus.Framework.Data.Attributes;
using Nexus.Framework.Data.Model.Input;

namespace Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;

/// <summary>
/// Modelo de entrada para operações de RelatorioParametroOpcao
/// </summary>
public class RelatorioParametroOpcaoInputModel : StoredProcedureInputModel
{
        [NexusParameter("COD_OPCAO", direction: ParameterDirection.InputOutput)]
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

        /// <summary>
        /// Gets or sets the operation identifier associated with the current context.
        /// </summary>
        [NexusParameter("OPERACAO")]
        [Required]
        public string Operacao { get; set; } = string.Empty;

        public RelatorioParametroOpcaoInputModel()
        {
            _commandName = "APP_TB_RELATORIO_PARAMETRO_OPCAO";
        }
    }
