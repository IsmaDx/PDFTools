using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

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
    }
}
