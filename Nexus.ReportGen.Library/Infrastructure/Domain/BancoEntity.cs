using Nexus.Framework.Data.Entity;

namespace Nexus.Sample.Library.Infrastructure.Domain;

/// <summary>
/// Entidade que representa um banco
/// </summary>
public class BancoEntity : EntityBase<int>
{
    /// <summary>
    /// Código do banco
    /// </summary>
    public int CodBancos { get; set; }

    /// <summary>
    /// Código dígito do banco
    /// </summary>
    public int CodDigitoBancos { get; set; }

    /// <summary>
    /// Descrição/Nome do banco
    /// </summary>
    public string DsBancos { get; set; } = string.Empty;

    /// <summary>
    /// ISPB do banco
    /// </summary>
    public string? ISPBBancos { get; set; }

    /// <summary>
    /// Flag indicando se o banco emite boleto
    /// </summary>
    public bool FlEmiteBoletoBancos { get; set; }

    /// <summary>
    /// Flag indicando se o banco está excluído
    /// </summary>
    public bool FlExcluido { get; set; }

    /// <summary>
    /// Data de criação
    /// </summary>
    public DateTime? DataCriacao { get; set; }

    /// <summary>
    /// Data de atualização
    /// </summary>
    public DateTime? DataAtualizacao { get; set; }

    /// <summary>
    /// Construtor padrão
    /// </summary>
    public BancoEntity()
    {
        DataCriacao = DateTime.Now;
    }

    /// <summary>
    /// Construtor com parâmetros
    /// </summary>
    public BancoEntity(int codDigitoBancos, string dsBancos, string? ispbBancos, bool flEmiteBoletoBancos) : this()
    {
        if (string.IsNullOrWhiteSpace(dsBancos))
            throw new ArgumentException("Descrição do banco é obrigatória", nameof(dsBancos));

        CodDigitoBancos = codDigitoBancos;
        DsBancos = dsBancos;
        ISPBBancos = ispbBancos;
        FlEmiteBoletoBancos = flEmiteBoletoBancos;
        FlExcluido = false;
    }

    /// <summary>
    /// Atualiza os dados do banco
    /// </summary>
    public void Atualizar(int codDigitoBancos, string dsBancos, string? ispbBancos, bool flEmiteBoletoBancos)
    {
        if (string.IsNullOrWhiteSpace(dsBancos))
            throw new ArgumentException("Descrição do banco é obrigatória", nameof(dsBancos));

        CodDigitoBancos = codDigitoBancos;
        DsBancos = dsBancos;
        ISPBBancos = ispbBancos;
        FlEmiteBoletoBancos = flEmiteBoletoBancos;
        DataAtualizacao = DateTime.Now;
    }

    /// <summary>
    /// Exclui o banco (exclusão lógica)
    /// </summary>
    public void Excluir()
    {
        FlExcluido = true;
        DataAtualizacao = DateTime.Now;
    }

    /// <summary>
    /// Restaura o banco (remove a exclusão lógica)
    /// </summary>
    public void Restaurar()
    {
        FlExcluido = false;
        DataAtualizacao = DateTime.Now;
    }
}