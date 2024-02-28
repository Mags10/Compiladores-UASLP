using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace compiladoresPr
{
    public partial class Form1 : Form
    {

        ConvPosFija conv;
        EvaPosFija eva;

        public Form1()
        { 
            InitializeComponent();
            conv = new ConvPosFija();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string res = conv.NormaliceExpresion(textExpresion.Text);
            textBox1.Text = res;
            textPosfija.Text = conv.ConvertirPosFija(res);
            string tmp = textPosfija.Text;

            Automata a = new Automata(tmp);

            labEva.Text = tmp;
            eva = new EvaPosFija(tmp);
            string resEva = eva.EvaluarPosFija();
            if (res == resEva) {
                labEva.Text = "Es correcta la conversion";
                textEva.Text = resEva;
            }
            else {
                labEva.Text = "No es correcta la conversion";
                textEva.Text = resEva;
            };
        }

    }
}
