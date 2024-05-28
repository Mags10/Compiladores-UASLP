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
    public partial class CalculoConjuntos : Form
    {
        public CalculoConjuntos(String conjunto, String calculo)
        {
            InitializeComponent();
            this.richTextBox1.Text = calculo;
            this.richTextBox2.Text = conjunto;
        }
    }
}
