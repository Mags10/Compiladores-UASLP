using compiladoresPr.Formularios;
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
        TransitionTable tt = null;

        public Form1()
        { 
            InitializeComponent();
            conv = new ConvPosFija();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

            textExpresion.Focus();
            if (textExpresion.Text == "")
            {
                textPosfija.Text = "";
                textEva.Text = "";
                labEva.Text = "";
                textBox1.Text = "";
                return;
            }
            string res = "";
            try
            {
                res = conv.NormaliceExpresion(textExpresion.Text);
                textBox1.Text = res;
                textPosfija.Text = conv.ConvertirPosFija(res);
            }catch (Exception ex)
            {
                textExpresion.Focus();
                textPosfija.Text = ex.Message;
                return;
            }
            string tmp = textPosfija.Text;

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

            textExpresion.Focus();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.tt == null)
            {
                tt = new TransitionTable(textPosfija.Text);
                tt.Show();
            }
            else
            {
                tt.update(textPosfija.Text);
            }
        }

        private void textPosfija_TextChanged(object sender, EventArgs e)
        {
            if (this.tt != null)
            {
                tt.update(textPosfija.Text);

            }
        }
        
        private void textExpresion_TextChanged(object sender, EventArgs e)
        {
            button1_Click_1(sender, e);
        }
    }
}