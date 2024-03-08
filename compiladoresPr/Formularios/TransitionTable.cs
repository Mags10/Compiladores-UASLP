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
    public partial class TransitionTable : Form
    {
        public TransitionTable(string posregex)
        {
            InitializeComponent();
            Automata a = new Automata(posregex);
            a.SetTransitionsTable(this.dataGridView1);
            textBox1.Text = a.StateCount.ToString();
            textBox2.Text = a.getTransitionsCount().ToString();
            textBox3.Text = a.getEpsilonTransitionsCount().ToString();
          
        }

        public void update (string posregex)
        {
            this.dataGridView1.Rows.Clear();
            Automata a = new Automata(posregex);
            a.SetTransitionsTable(this.dataGridView1);
            textBox1.Text = a.StateCount.ToString();
            textBox2.Text = a.getTransitionsCount().ToString();
            textBox3.Text = a.getEpsilonTransitionsCount().ToString();
           
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
