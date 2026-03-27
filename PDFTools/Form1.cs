using System.Configuration;

namespace PDFTools
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Lee el archivo de configuracion, verifica es el primer aranque
            string firstRun = ConfigurationManager.AppSettings["PrimerArranque"];
            if (firstRun == "1")
            {
                MessageBox.Show("Bienvenido a PDF Tools. Antes de comenzar, por favor configura la ruta de salida para los archivos PDF fusionados o divididos.");
                ConfiguraRutas configuraRutas = new ConfiguraRutas();
                configuraRutas.Show();

                //Escribe 0 en el archivo de configuración para indicar que ya se ha realizado el primer arranque
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["PrimerArranque"].Value = "0";
                config.Save();
                ConfigurationManager.RefreshSection("appSettings");

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MergePDF mergePDF = new MergePDF();
            mergePDF.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SplitPDF splitPDF = new SplitPDF();
            splitPDF.Show();
            this.Hide();
        }

        private void configuracionesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfiguraRutas configuraRutas = new ConfiguraRutas();
            configuraRutas.Show();

        }

        private void acercaDeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("\t\t\tPDFTools V1.0.0\n\nPDFTools es una solucion gratuita para la manipulacion basica de PDF\nLos PDF son procesados de manera local por lo que no se envian/guardan en un servidor remoto\n\nDesarrollado por: RCMX-2026");
        }
    }
}
