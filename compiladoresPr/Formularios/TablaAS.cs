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
        private Gramatica g;

        public TablaAS()
        {
            InitializeComponent();

            g = new Gramatica();

            Produccion p;
            #region Producciones

                p = new Produccion("programa");
                p.addProduccion("secuencia-set", false);
                g.addProduccion(p);

                p = new Produccion("secuencia-set");
                p.addProduccion("sentencia", false, "secuencia-set'", false);
                g.addProduccion(p);

                p = new Produccion("secuencia-set'");
                p.addProduccion(";", true, "sentencia", false, "secuencia-set'", false);
                p.addProduccion("#", true);
                g.addProduccion(p);

                p = new Produccion("sentencia");
                p.addProduccion("sent-if", false);
                p.addProduccion("sent-repeat", false);
                p.addProduccion("sent-assign", false);
                p.addProduccion("sent-read", false);
                p.addProduccion("sent-write", false);
                g.addProduccion(p);

                p = new Produccion("sent-if");
                p.addProduccion("if", true, "exp", false, "then", true, "secuencia-set", false, "sent-if'", false);
                g.addProduccion(p);

                p = new Produccion("sent-if'");
                p.addProduccion("end", true);
                p.addProduccion("else", true, "secuencia-set", false, "end", true);
                g.addProduccion(p);

                p = new Produccion("sent-repeat");
                p.addProduccion("repeat", true, "secuencia-set", false, "until", true, "exp", false);
                g.addProduccion(p);

                p = new Produccion("sent-assign");
                p.addProduccion("identificador", true, ":=", true, "exp", false);
                g.addProduccion(p);

                p = new Produccion("sent-read");
                p.addProduccion("read", true, "identificador", true);
                g.addProduccion(p);

                p = new Produccion("sent-write");
                p.addProduccion("write", true, "exp", false);
                g.addProduccion(p);

                p = new Produccion("exp");
                p.addProduccion("exp-simple", false, "exp'", false);
                g.addProduccion(p);

                p = new Produccion("exp'");
                p.addProduccion("op-comp", false, "exp-simple", false);
                p.addProduccion("#", false);
                g.addProduccion(p);

                p = new Produccion("op-comp");
                p.addProduccion("<", true);
                p.addProduccion(">", true);
                p.addProduccion("=", true);
                g.addProduccion(p);

                p = new Produccion("exp-simple");
                p.addProduccion("term", false, "exp-simple'", false);
                g.addProduccion(p);

                p = new Produccion("exp-simple'");
                p.addProduccion("opsuma", false, "term", false, "exp-simple'", false);
                p.addProduccion("#", true);
                g.addProduccion(p);

                p = new Produccion("opsuma");
                p.addProduccion("+", true);
                p.addProduccion("-", true);
                g.addProduccion(p);

                p = new Produccion("term");
                p.addProduccion("factor", false, "term'", false);
                g.addProduccion(p);

                p = new Produccion("term'");
                p.addProduccion("opmult", false, "factor", false, "term'", false);
                p.addProduccion("#", true);
                g.addProduccion(p);

                p = new Produccion("opmult");
                p.addProduccion("*", true);
                p.addProduccion("/", true);
                g.addProduccion(p);

                p = new Produccion("factor");
                p.addProduccion("(", true, "exp", false, ")", true);
                p.addProduccion("numero", true);
                p.addProduccion("identificador", true);
                g.addProduccion(p);

                g.calcPrimeros();

                g.calcSiguientes();

                g.calcTabla();
                TabAS = g.TablaAS(this);

                //Console.WriteLine(g);
            #endregion

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
