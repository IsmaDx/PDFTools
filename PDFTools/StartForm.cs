using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PDFTools
{
    public partial class StartForm : Form
    {
        public StartForm()
        {
            InitializeComponent();
        }

        private void StartForm_Load(object sender, EventArgs e)
        {
            //Lee el archivo de configuracion, verifica es el primer aranque
            string firstRun = ConfigurationManager.AppSettings["PrimerArranque"];
            if (firstRun == "1")
            {
                MessageBox.Show("Bienvenido a PDF Tools. Antes de comenzar, por favor configura la ruta de salida para los archivos PDF fusionados o divididos.");
                ConfiguraRutas configuraRutas = new ConfiguraRutas();
                configuraRutas.Show();
                Form1 form1 = new Form1();
                form1.Show();
                this.Hide();

            }

        }
    }
}
