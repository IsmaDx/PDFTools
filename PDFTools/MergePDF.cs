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
    public partial class MergePDF : Form
    {
        public MergePDF()
        {
            InitializeComponent();
        }

        private void MergePDF_Load(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 0)
            {
                MessageBox.Show("No hay archivos para fusionar. Por favor, agrega archivos PDF a la lista.");
                return;
            }
            if (textBox1.Text == "Nombre del archivo")
            {
                MessageBox.Show("Por favor, ingresa un nombre para el archivo de salida.");
                return;
            }
            else
            {
                // Use the class name directly to call the static method
                string fileSaved = PDFUtils.MergerPDF(listBox1.Items.Cast<string>().ToArray(), textBox1.Text);
                if (fileSaved != null)
                {
                    DialogResult result = MessageBox.Show($"Archivo PDF fusionado guardado en:{fileSaved}\\n¿Abrir ruta del archivo?", "Proceso terminado", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        Utils utils = new Utils();
                        utils.AbreExplorer(fileSaved);
                    }
                    else if (result == DialogResult.No)
                    {
                        // No hacer nada, simplemente cerrar el mensaje
                    }
                }
                else
                {
                    MessageBox.Show("Ocurrió un error al fusionar los archivos PDF. Por favor, intenta nuevamente.");
                }
            }
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Archivos PDF (*.pdf)|*.pdf";
            openFileDialog1.Title = "Seleccionar archivos PDF";
            openFileDialog1.Multiselect = true; // Permite seleccionar varios archivos, si lo deseas

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string[] archivosSeleccionados = openFileDialog1.FileNames;
                listBox1.Items.AddRange(archivosSeleccionados); // Agrega los archivos seleccionados al ListBox

            }

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            listBox1.Items.Remove(listBox1.SelectedItem); // Elimina el archivo seleccionado del ListBox
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            if (index > 0)
            {
                object item = listBox1.SelectedItem;
                listBox1.Items.RemoveAt(index);
                listBox1.Items.Insert(index - 1, item);
                listBox1.SelectedIndex = index - 1;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            if (index >= 0 && index < listBox1.Items.Count - 1)
            {
                object item = listBox1.SelectedItem;
                listBox1.Items.RemoveAt(index);
                listBox1.Items.Insert(index + 1, item);
                listBox1.SelectedIndex = index + 1;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void MergePDF_FormClosed(object sender, FormClosedEventArgs e)
        {
            Utils utils = new Utils();
            utils.Terminus();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }
    }
}




