using System.Text;
using System.Text.RegularExpressions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace Nexus.PortalAgendamento.Library.Infrastructure.Helper
{
    public class PdfHelper
    {
        private readonly ILogger<PdfHelper> _logger;

        public PdfHelper(ILogger<PdfHelper> logger)
        {
            _logger = logger;
        }

        public (List<string> Datas, List<string> Horas) ExtrairDadosDoPdf(string caminhoPdf)
        {
            var datas = new HashSet<string>();
            var horas = new HashSet<string>();
            var textoCompleto = new StringBuilder();

            if (!File.Exists(caminhoPdf)) return (new List<string>(), new List<string>());

            try
            {
                using (var pdfReader = new PdfReader(caminhoPdf))
                using (var pdfDoc = new PdfDocument(pdfReader))
                {
                    int numeroPaginas = pdfDoc.GetNumberOfPages();

                    for (int i = 1; i <= numeroPaginas; i++)
                    {
                        var pagina = pdfDoc.GetPage(i);
                        var strategy = new LocationTextExtractionStrategy();
                        string textoPagina = PdfTextExtractor.GetTextFromPage(pagina, strategy);
                        textoCompleto.AppendLine(textoPagina);
                    }
                }

                string textoFinal = textoCompleto.ToString();
                _logger.LogInformation("\n\n>>> TEXTO ITEXT7 <<<\n{Texto}\n>>> FIM TEXTO <<<\n", textoFinal);

                return ParsearTexto(textoFinal);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao extrair texto do PDF com iText7.");
                return (new List<string>(), new List<string>());
            }
        }

        private (List<string> Datas, List<string> Horas) ParsearTexto(string textoOriginal)
        {
            var datas = new HashSet<string>();
            var horas = new HashSet<string>();

            string textoLimpo = textoOriginal.Replace("\"", "").Replace("|", " ").Trim();

            var regexData = new Regex(@"(?:Data|Dt\.?)\s*[:.]?\s*(\d{2}[./-]\d{2}[./-]\d{4})", RegexOptions.IgnoreCase);

            foreach (Match m in regexData.Matches(textoLimpo))
            {
                string dataFmt = NormalizarData(m.Groups[1].Value);
                if (ValidarData(dataFmt)) datas.Add(dataFmt);
            }

            var regexHora = new Regex(@"(?:Hor[aá]rio|Hora)\s*[:.]?\s*(\d{2}[:.]\d{2}(?:[:.]\d{2})?)", RegexOptions.IgnoreCase);

            foreach (Match m in regexHora.Matches(textoLimpo))
            {
                string horaBruta = m.Groups[1].Value.Replace(".", ":");
                if (TimeSpan.TryParse(horaBruta, out var ts))
                {
                    horas.Add(ts.ToString(@"hh\:mm"));
                }
            }

            if (datas.Count == 0)
            {
                var regexGeral = new Regex(@"(\d{2}[./-]\d{2}[./-]\d{4})");
                foreach (Match m in regexGeral.Matches(textoLimpo))
                {
                    string d = NormalizarData(m.Value);

                    if (ValidarData(d)) datas.Add(d);
                }
            }

            if (horas.Count > 1 && horas.Contains("00:00")) horas.Remove("00:00");

            return (datas.ToList(), horas.ToList());
        }

        private string NormalizarData(string raw) => raw.Replace(".", "/").Replace("-", "/");

        private bool ValidarData(string input)
        {
            var hoje = DateTime.Now.Date;
            if (DateTime.TryParseExact(input, "dd/MM/yyyy", new CultureInfo("pt-BR"), DateTimeStyles.None, out DateTime dt))
            {

                if (dt >= hoje && dt <= hoje.AddYears(2)) return true;
            }
            return false;
        }
    }
}