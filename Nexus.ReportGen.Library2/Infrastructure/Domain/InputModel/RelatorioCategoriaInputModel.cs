using System.ComponentModel.DataAnnotations;
using System.Data;
using Nexus.Framework.Data.Attributes;
using Nexus.Framework.Data.Model.Input;

namespace Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;

/// <summary>
/// Modelo de entrada para operações de RelatorioCategoria
/// </summary>
public class RelatorioCategoriaInputModel : StoredProcedureInputModel
{
        [NexusParameter("COD_CATEGORIA", direction: ParameterDirection.InputOutput)]
        public int? CodCategoria { get; set; }

        [NexusParameter("NM_CATEGORIA")]
        public string NmCategoria { get; set; }

        [NexusParameter("DS_CATEGORIA")]
        public string DsCategoria { get; set; }

        [NexusParameter("FL_ATIVO")]
        public bool? FlAtivo { get; set; }

        [NexusParameter("NR_ORDEM")]
        public int? NrOrdem { get; set; }

        [NexusParameter("COD_USUARIOS")]
        public int? CodUsuarios { get; set; }

        /// <summary>
        /// Gets or sets the operation identifier associated with the current context.
        /// </summary>
        [NexusParameter("OPERACAO")]
        [Required]
        public string Operacao { get; set; } = string.Empty;

        public RelatorioCategoriaInputModel()
        {
            _commandName = "APP_TB_RELATORIO_CATEGORIA";
        }
    }
