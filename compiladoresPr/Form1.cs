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

        public Form1()
        { 
            InitializeComponent();
            conv = new ConvPosFija();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string res = conv.ConvertirPosFija(textExpresion.Text);
            textPosfija.Text = res;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string res = conv.ConvertirPosFija2(textExpresion.Text);
            textBox1.Text = res;
        }
    }
}
