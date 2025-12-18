using System.ComponentModel.DataAnnotations;
using System.Data;
using Nexus.Framework.Data.Attributes;
using Nexus.Framework.Data.Model.Input;

namespace Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;

/// <summary>
/// Modelo de entrada para operações de TipoParametro
/// </summary>
public class TipoParametroInputModel : StoredProcedureInputModel
{
        [NexusParameter("COD_TIPO_PARAMETRO", direction: ParameterDirection.InputOutput)]
        public int? CodTipoParametro { get; set; }

        [NexusParameter("DS_TIPO")]
        public string DsTipo { get; set; }

        [NexusParameter("DS_VALIDACAO_REGEX")]
        public string DsValidacaoRegex { get; set; }

        [NexusParameter("FL_OBRIGATORIO_PADRAO")]
        public bool? FlObrigatorioPadrao { get; set; }

        [NexusParameter("SIT_ATIVO")]
        public bool? SitAtivo { get; set; }

        [NexusParameter("COD_USUARIOS")]
        public int? CodUsuarios { get; set; }

        /// <summary>
        /// Gets or sets the operation identifier associated with the current context.
        /// </summary>
        [NexusParameter("OPERACAO")]
        [Required]
        public string Operacao { get; set; } = string.Empty;

        public TipoParametroInputModel()
        {
            _commandName = "APP_TB_TIPO_PARAMETRO";
        }
    }
