using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFTools.CommonMethods
{
    internal class Utils
    {
        public void Terminus()
        {
            //matar todo el programa
            Application.Exit();
        }

        public void AbreExplorer(string ruta)
        {
            ruta = ruta.Substring(0, ruta.LastIndexOf("\\"));
            if (Directory.Exists(ruta))
            {
                System.Diagnostics.Process.Start("explorer.exe", ruta);
            }
            else
            {
                MessageBox.Show("La ruta especificada no existe.");
            }
        }
    }
}
