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
    public partial class OverlayPDF : Form
    {
        public OverlayPDF()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Archivos PDF (*.pdf)|*.pdf";
            openFileDialog1.Title = "Seleccionar archivos PDF";
            openFileDialog1.Multiselect = true; // Permite seleccionar varios archivos, si lo deseas

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string[] archivosSeleccionados = openFileDialog1.FileNames;
                // Agrega los archivos seleccionados al ListBox
                textBox1.Text = archivosSeleccionados[0];
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog2.Filter = "Archivos PDF (*.pdf)|*.pdf";
            openFileDialog2.Title = "Seleccionar archivos PDF";
            openFileDialog2.Multiselect = true; // Permite seleccionar varios archivos, si lo deseas

            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                string[] archivosSeleccionados = openFileDialog1.FileNames;
                PDF2.Text = archivosSeleccionados[0];

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            PDFUtils.OverlayPDF(textBox1.Text,PDF2.Text,50,50,"TEST.pdf");
        }
    }
}
