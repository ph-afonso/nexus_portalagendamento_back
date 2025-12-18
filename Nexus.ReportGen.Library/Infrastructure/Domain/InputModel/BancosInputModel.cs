using System.ComponentModel.DataAnnotations;
using System.Data;
using Nexus.Framework.Data.Attributes;
using Nexus.Framework.Data.Model.Input;

namespace Nexus.Sample.Library.Infrastructure.Domain.InputModel;

/// <summary>
/// Modelo de entrada para operações de Bancos
/// </summary>
public class BancosInputModel : StoredProcedureInputModel
{
        [Key]
        [NexusParameter("COD_BANCOS", direction: ParameterDirection.InputOutput)]
        public int? CodBancos { get; set; }

        [NexusParameter("DS_BANCOS")]
        public string DsBancos { get; set; }

        [NexusParameter("ISPB_BANCOS")]
        public string IspbBancos { get; set; }

        [NexusParameter("FL_EMITE_BOLETO_BANCOS")]
        public bool? FlEmiteBoletoBancos { get; set; }

        [Key]
        [NexusParameter("COD_DIGITO_BANCOS")]
        public string CodDigitoBancos { get; set; }

        [NexusParameter("FL_EXCLUIDO")]
        public bool? FlExcluido { get; set; }

        [NexusParameter("COD_USUARIOS")]
        public int? CodUsuarios { get; set; }

        /// <summary>
        /// Gets or sets the operation identifier associated with the current context.
        /// </summary>
        [NexusParameter("OPERACAO")]
        [Required]
        public string Operacao { get; set; } = string.Empty;

        public BancosInputModel()
        {
            _commandName = "APP_TB_BANCOS";
        }
    }
