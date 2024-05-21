using compiladoresPr.Algoritmos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace compiladoresPr.Formularios
{
    public partial class TablaAS : Form
    {
        Gramatica g;

        public TablaAS(Gramatica g)
        {
            this.g = g;
            InitializeComponent();
            g.TablaAS(this);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            new CalculoConjuntos(g.PrimLog, g.primString()).ShowDialog();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            new CalculoConjuntos(g.SigLog, g.sigString()).ShowDialog();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            new CalculoConjuntos(g.TablaLog, "").ShowDialog();
        }
    }
}
