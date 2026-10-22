using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.Advanced;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFTools.CommonMethods
{
    internal class PDFUtils
    {
        public static string MergerPDF(string[] pdfList, string fileName)
        {
            if (pdfList == null || pdfList.Length == 0)
                throw new ArgumentException("La lista de archivos PDF está vacía.");

            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("El nombre de archivo de salida no es válido.");

            // Leer la ruta de salida desde app.config
            string outputDir = ConfigurationManager.AppSettings["RutaSalidaPDF"];
            if (string.IsNullOrWhiteSpace(outputDir))
                throw new InvalidOperationException("La ruta de salida no está configurada en app.config.");

            // Asegurarse de que la ruta existe
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            // Construir la ruta completa del archivo
            string outputPath = Path.Combine(outputDir, fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) ? fileName : fileName + ".pdf");

            try
            {
                using (PdfDocument outputDocument = new PdfDocument())
                {
                    foreach (var pdfFile in pdfList)
                    {
                        if (!File.Exists(pdfFile))
                            continue;

                        using (PdfDocument inputDocument = PdfReader.Open(pdfFile, PdfDocumentOpenMode.Import))
                        {
                            for (int idx = 0; idx < inputDocument.PageCount; idx++)
                            {
                                outputDocument.AddPage(inputDocument.Pages[idx]);
                            }
                        }
                    }

                    outputDocument.Save(outputPath);
                }
                return outputPath;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static int PageCount(string pdfFile)
        {
            if (string.IsNullOrWhiteSpace(pdfFile) || !File.Exists(pdfFile))
                throw new ArgumentException("El archivo PDF no es válido o no existe.");
            try
            {
                using (PdfDocument document = PdfReader.Open(pdfFile))
                {
                    return document.PageCount;
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static string SplitPDFByRange(string fileName, int fromRange, int toRange)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("El nombre del archivo es requerido.");

            if (!File.Exists(fileName))
                throw new FileNotFoundException("El archivo no existe.");

            if (fromRange < 1 || fromRange > toRange)
                throw new ArgumentException("El rango de páginas es inválido.");

            string rutaSalida = ConfigurationManager.AppSettings["RutaSalidaPDF"];

            if (string.IsNullOrEmpty(rutaSalida))
                throw new InvalidOperationException("La ruta de salida no está configurada.");

            string nombreOriginal = Path.GetFileNameWithoutExtension(fileName);
            string extension = Path.GetExtension(fileName);
            string nombreNuevo = $"{nombreOriginal}_split{extension}";
            string rutaFinal = Path.Combine(rutaSalida, nombreNuevo);

            // Abrir documento origen
            using (PdfDocument inputDocument = PdfReader.Open(fileName, PdfDocumentOpenMode.Import))
            {
                // Validar rango contra total de páginas
                if (toRange > inputDocument.PageCount)
                    throw new ArgumentException("El rango excede el total de páginas del PDF.");

                using (PdfDocument outputDocument = new PdfDocument())
                {
                    for (int i = fromRange - 1; i < toRange; i++)
                    {
                        outputDocument.AddPage(inputDocument.Pages[i]);
                    }

                    // Guardar resultado
                    outputDocument.Save(rutaFinal);
                }
            }
            return rutaFinal;
        }

        public static string OverlayPDF(string pdfBase, string pdfSuperior, double opacidadBase, double opacidadSuperior, string fileName = null)
        {
            if (string.IsNullOrWhiteSpace(pdfBase) || !File.Exists(pdfBase))
                throw new FileNotFoundException("El PDF base no existe.", pdfBase);

            if (string.IsNullOrWhiteSpace(pdfSuperior) || !File.Exists(pdfSuperior))
                throw new FileNotFoundException("El PDF superior no existe.", pdfSuperior);

            if (opacidadBase < 0 || opacidadBase > 100)
                throw new ArgumentOutOfRangeException(nameof(opacidadBase), "La opacidad debe estar entre 0 y 100.");

            if (opacidadSuperior < 0 || opacidadSuperior > 100)
                throw new ArgumentOutOfRangeException(nameof(opacidadSuperior), "La opacidad debe estar entre 0 y 100.");

            // Leer la ruta de salida desde app.config
            string outputDir = ConfigurationManager.AppSettings["RutaSalidaPDF"];
            if (string.IsNullOrWhiteSpace(outputDir))
                throw new InvalidOperationException("La ruta de salida no está configurada en app.config.");

            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            if (string.IsNullOrWhiteSpace(fileName))
                fileName = $"overlay_{DateTime.Now:yyyyMMdd_HHmmss}";

            string outputPath = Path.Combine(outputDir, fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) ? fileName : fileName + ".pdf");

            // Se aplica la opacidad a cada PDF por separado (en memoria, sin tocar los originales)
            using (MemoryStream baseStream = AplicarOpacidad(pdfBase, opacidadBase / 100.0))
            using (MemoryStream superiorStream = AplicarOpacidad(pdfSuperior, opacidadSuperior / 100.0))
            using (XPdfForm formBase = XPdfForm.FromStream(baseStream))
            using (XPdfForm formSuperior = XPdfForm.FromStream(superiorStream))
            using (PdfDocument outputDocument = new PdfDocument())
            {
                int paginasBase = formBase.PageCount;
                int paginasSuperior = formSuperior.PageCount;
                int total = Math.Max(paginasBase, paginasSuperior);

                for (int i = 0; i < total; i++)
                {
                    // Si uno de los PDF tiene una sola página, se reutiliza en todas las páginas del otro.
                    int idxBase = i < paginasBase ? i : (paginasBase == 1 ? 0 : -1);
                    int idxSuperior = i < paginasSuperior ? i : (paginasSuperior == 1 ? 0 : -1);

                    // Tamaño de la página de salida: el del PDF base; si no hay página base, el del superior.
                    double ancho, alto;
                    if (idxBase >= 0)
                    {
                        formBase.PageNumber = idxBase + 1;
                        ancho = formBase.PointWidth;
                        alto = formBase.PointHeight;
                    }
                    else
                    {
                        formSuperior.PageNumber = idxSuperior + 1;
                        ancho = formSuperior.PointWidth;
                        alto = formSuperior.PointHeight;
                    }

                    PdfPage page = outputDocument.AddPage();
                    page.Width = ancho;
                    page.Height = alto;

                    using (XGraphics gfx = XGraphics.FromPdfPage(page))
                    {
                        // 1) PDF base (abajo)
                        if (idxBase >= 0)
                        {
                            formBase.PageNumber = idxBase + 1;
                            gfx.DrawImage(formBase, new XRect(0, 0, ancho, alto));
                        }

                        // 2) PDF superior (encima), ajustado a la página sin deformarse y centrado
                        if (idxSuperior >= 0)
                        {
                            formSuperior.PageNumber = idxSuperior + 1;
                            double escala = Math.Min(ancho / formSuperior.PointWidth, alto / formSuperior.PointHeight);
                            double w = formSuperior.PointWidth * escala;
                            double h = formSuperior.PointHeight * escala;
                            gfx.DrawImage(formSuperior, new XRect((ancho - w) / 2, (alto - h) / 2, w, h));
                        }
                    }
                }

                outputDocument.Save(outputPath);
            }

            return outputPath;
        }

        /// <summary>
        /// Abre un PDF, envuelve el contenido de cada página con un estado gráfico de opacidad
        /// (ExtGState /ca y /CA) y devuelve el resultado en memoria.
        /// </summary>
        private static MemoryStream AplicarOpacidad(string pdfFile, double opacidad)
        {
            MemoryStream result = new MemoryStream();

            using (PdfDocument doc = PdfReader.Open(pdfFile, PdfDocumentOpenMode.Modify))
            {
                foreach (PdfPage page in doc.Pages)
                {
                    // Recursos de la página
                    PdfDictionary resources = page.Elements.GetDictionary("/Resources");
                    if (resources == null)
                    {
                        resources = new PdfDictionary(doc);
                        page.Elements["/Resources"] = resources;
                    }

                    PdfDictionary extGStates = resources.Elements.GetDictionary("/ExtGState");
                    if (extGStates == null)
                    {
                        extGStates = new PdfDictionary(doc);
                        resources.Elements["/ExtGState"] = extGStates;
                    }

                    // Estado gráfico con la opacidad (/ca = relleno e imágenes, /CA = trazos)
                    PdfDictionary gs = new PdfDictionary(doc);
                    gs.Elements.SetName("/Type", "/ExtGState");
                    gs.Elements.SetReal("/ca", opacidad);
                    gs.Elements.SetReal("/CA", opacidad);
                    extGStates.Elements["/GSOverlay"] = gs;

                    // Contenido de la página: q /GSOverlay gs ... Q
                    PdfContent content = page.Contents.CreateSingleContent();
                    if (content.Stream == null)
                        continue; // página vacía

                    if (!content.Stream.TryUnfilter())
                        throw new NotSupportedException("No se pudo leer el contenido de una página del PDF: " + pdfFile);

                    byte[] original = content.Stream.Value ?? new byte[0];
                    byte[] prefijo = Encoding.ASCII.GetBytes("q\n/GSOverlay gs\n");
                    byte[] sufijo = Encoding.ASCII.GetBytes("\nQ\n");

                    byte[] nuevo = new byte[prefijo.Length + original.Length + sufijo.Length];
                    Buffer.BlockCopy(prefijo, 0, nuevo, 0, prefijo.Length);
                    Buffer.BlockCopy(original, 0, nuevo, prefijo.Length, original.Length);
                    Buffer.BlockCopy(sufijo, 0, nuevo, prefijo.Length + original.Length, sufijo.Length);

                    content.Stream.Value = nuevo;
                }

                doc.Save(result, false);
            }

            result.Position = 0;
            return result;
        }
    }
}
