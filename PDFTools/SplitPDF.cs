using PDFTools.CommonMethods;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PDFTools
{
    public partial class SplitPDF : Form
    {
        private bool pdfSelected = false;

        public SplitPDF()
        {
            InitializeComponent();
            groupBox1.Enabled = false; // Deshabilita el GroupBox al cargar el formulario
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Archivos PDF (*.pdf)|*.pdf";
            openFileDialog1.Title = "Seleccionar archivo PDF";
            openFileDialog1.Multiselect = false; // Permite seleccionar varios archivos, si lo deseas

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string archivosSeleccionados = openFileDialog1.FileName; // Obtiene la ruta del archivo seleccionado
                textBox1.Text = archivosSeleccionados; // Agrega los archivos seleccionados al ListBox
                int totalPages = PDFUtils.PageCount(archivosSeleccionados);

                numericUpDown2.Maximum = totalPages; // Establece el valor máximo del NumericUpDown al número total de páginas del PDF
                numericUpDown2.Minimum = 1;
                numericUpDown2.Value = totalPages;

                numericUpDown1.Maximum = totalPages; // Establece el valor máximo del NumericUpDown al número total de páginas del PDF
                numericUpDown1.Minimum = 1; // Establece el valor mínimo del NumericUpDown al número mínimo de páginas (1)
                numericUpDown1.Value = 1;
                groupBox1.Enabled = true; // Habilita el GroupBox después de seleccionar un archivo PDF
            }

        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void SplitPDF_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {

        }


        private void button3_Click(object sender, EventArgs e)
        {
            string rutaFinal = PDFUtils.SplitPDFByRange(textBox1.Text, (int)numericUpDown1.Value, (int)numericUpDown2.Value);
            if (rutaFinal != null)
            {
                DialogResult result = MessageBox.Show("PDF dividido exitosamente. Archivo guardado en: " + rutaFinal + "\n¿Abrir ruta del archivo?", "Proceso terminado", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                   Utils utils = new Utils();
                    utils.AbreExplorer(rutaFinal);
                }
                else if (result == DialogResult.No)
                {
                    // No hacer nada, simplemente cerrar el mensaje
                }

            }
            else
            {
                MessageBox.Show("Error al dividir el PDF. Por favor, inténtalo de nuevo.");
            }
        }
    }
}
