using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PDFTools
{
    public partial class ConfiguraRutas : Form
    {
        public ConfiguraRutas()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Selecciona una carpeta";
                folderDialog.UseDescriptionForTitle = true;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string carpetaSeleccionada = folderDialog.SelectedPath;
                    textBox1.Text = carpetaSeleccionada;
                }
            }
        }

        protected void SaveConfigs(string conf1)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["RutaSalidaPDF"].Value = conf1;





            config.Save();
            ConfigurationManager.RefreshSection("appSettings");
            MessageBox.Show("Ruta salvada con exito");
            this.Hide();
        }

        private void ConfiguraRutas_Load(object sender, EventArgs e)
        {
            //Lee la ruta del acrhivo de configuración y la muestra en el TextBox
            string rutaGuardada = ConfigurationManager.AppSettings["RutaSalidaPDF"];
            if (!string.IsNullOrEmpty(rutaGuardada))
            {
                textBox1.Text = rutaGuardada;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != null && textBox1.Text != String.Empty && textBox1.Text != "")
            {
                SaveConfigs(textBox1.Text);
            }
            else
            {
                MessageBox.Show("Por favor, selecciona una ruta válida antes de guardar.");

            }
        }
    }
}
