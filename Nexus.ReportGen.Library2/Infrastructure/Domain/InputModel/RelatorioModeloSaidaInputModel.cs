using System.ComponentModel.DataAnnotations;
using System.Data;
using Nexus.Framework.Data.Attributes;
using Nexus.Framework.Data.Model.Input;

namespace Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;

/// <summary>
/// Modelo de entrada para operações de RelatorioModeloSaida
/// </summary>
public class RelatorioModeloSaidaInputModel : StoredProcedureInputModel
{
        [NexusParameter("COD_MODELO_SAIDA", direction: ParameterDirection.InputOutput)]
        public int? CodModeloSaida { get; set; }

        [NexusParameter("COD_RELATORIO_MODELO")]
        public int? CodRelatorioModelo { get; set; }

        [NexusParameter("NM_ABA_ARQUIVO")]
        public string NmAbaArquivo { get; set; }

        [NexusParameter("DS_SAIDA")]
        public string DsSaida { get; set; }

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

        public RelatorioModeloSaidaInputModel()
        {
            _commandName = "APP_TB_RELATORIO_MODELO_SAIDA";
        }
    }
