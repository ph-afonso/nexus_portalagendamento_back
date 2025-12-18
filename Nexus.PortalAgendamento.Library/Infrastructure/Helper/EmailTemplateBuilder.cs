using System.Globalization;
using System.Text;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.ListModel;

public static class EmailTemplateBuilder
{
    public static string BuildMensagemComTabelaNFs(IEnumerable<NotaFiscalOutputModel> notas)
    {
        var br = new CultureInfo("pt-BR");
        var sb = new StringBuilder();

        sb.Append("""
        <div style="font-family: Arial, Helvetica, sans-serif; font-size:14px; line-height:1.5; color:#222">
          <p style="margin:0 0 12px 0">
            O cliente tentou agendar as NF's abaixo, porém o anexo enviado não pode ser lido corretamente
            pelo sistema. Sendo assim, é necessário entrar em contato com o cliente e seguir com o processo
            de forma manual.
          </p>

          <h4 style="margin:18px 0 8px 0; font-size:16px; font-weight:bold;">Notas Fiscais</h4>

          <table role="table" cellpadding="0" cellspacing="0" border="0"
                 style="border-collapse:collapse; width:100%; max-width:900px;">
            <thead>
              <tr>
                <th style="text-align:left; padding:8px; border:1px solid #ddd; background:#f5f5f5;">Pedido</th>
                <th style="text-align:left; padding:8px; border:1px solid #ddd; background:#f5f5f5;">Fornecedor</th>
                <th style="text-align:left; padding:8px; border:1px solid #ddd; background:#f5f5f5;">UF</th>
                <th style="text-align:right; padding:8px; border:1px solid #ddd; background:#f5f5f5;">NF</th>
                <th style="text-align:left; padding:8px; border:1px solid #ddd; background:#f5f5f5;">Série</th>
                <th style="text-align:center; padding:8px; border:1px solid #ddd; background:#f5f5f5;">Emissão</th>
                <th style="text-align:right; padding:8px; border:1px solid #ddd; background:#f5f5f5;">Volumes</th>
                <th style="text-align:right; padding:8px; border:1px solid #ddd; background:#f5f5f5;">Peso</th>
                <th style="text-align:right; padding:8px; border:1px solid #ddd; background:#f5f5f5;">Valor Total</th>
              </tr>
            </thead>
            <tbody>
        """);

        if (notas != null)
        {
            foreach (var n in notas)
            {
                // Garantir strings seguras e valores formatados
                var pedido = HtmlEncode(n.NrPedido);
                var fornecedor = HtmlEncode(n.NomeFornecedor);
                var uf = HtmlEncode(n.UFClientes);
                var nf = n.NrNotasFiscais?.ToString(br) ?? "";
                var serie = HtmlEncode(n.NrSerieNotasFiscais);
                var emissao = HtmlEncode(n.DtEmissao); // já vem formatada 'dd/MM/yyyy'
                var volumes = n.QtVolumeNotasFiscais?.ToString(br) ?? "";
                var peso = n.PesoNotasFiscais.HasValue ? n.PesoNotasFiscais.Value.ToString("#,0.###", br) : "";
                var valor = n.VlTotalNotasFiscais.HasValue ? n.VlTotalNotasFiscais.Value.ToString("C", br) : "";

                sb.Append($@"
                  <tr>
                    <td style=""padding:8px; border:1px solid #ddd;"">{pedido}</td>
                    <td style=""padding:8px; border:1px solid #ddd;"">{fornecedor}</td>
                    <td style=""padding:8px; border:1px solid #ddd;"">{uf}</td>
                    <td style=""padding:8px; border:1px solid #ddd; text-align:right;"">{nf}</td>
                    <td style=""padding:8px; border:1px solid #ddd;"">{serie}</td>
                    <td style=""padding:8px; border:1px solid #ddd; text-align:center;"">{emissao}</td>
                    <td style=""padding:8px; border:1px solid #ddd; text-align:right;"">{volumes}</td>
                    <td style=""padding:8px; border:1px solid #ddd; text-align:right;"">{peso}</td>
                    <td style=""padding:8px; border:1px solid #ddd; text-align:right;"">{valor}</td>
                  </tr>");
            }
        }

        sb.Append("""
            </tbody>
          </table>
        </div>
        """);

        return sb.ToString();

        static string HtmlEncode(string? s) =>
            string.IsNullOrEmpty(s) ? "" : System.Net.WebUtility.HtmlEncode(s);
    }
}
