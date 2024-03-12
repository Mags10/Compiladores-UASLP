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
            // Change the title of the form
            this.InitializeComponent();
            this.update(posregex);
        }

        public void update (string posregex)
        {
            try
            {
                this.loadTable(posregex);
            }
            catch (Exception ex)
            {
                // Clear all the fields
                this.dataGridView1.Rows.Clear();
                this.dataGridView2.Rows.Clear();
                this.dataGridView1.Columns.Clear();
                this.dataGridView2.Columns.Clear();
                this.Text = "Esperando una expresion regular...";
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "";
                textBox5.Text = "";
                textBox6.Text = "";
                textBox13.Text = "";
                textBox8.Text = "";
                textBox11.Text = "";
                textBox12.Text = "";
                textBox10.Text = "";
                textBox9.Text = "";
                textBox7.Text = "";
                textBox14.Text = "";
            }
        }

        private void loadTable(string posregex)
        {
            this.Text = "Automata de '" + posregex + "'";
            this.dataGridView1.Rows.Clear();
            this.dataGridView2.Rows.Clear();
            Automata a = new Automata(posregex);
            a.SetTransitionsTable(this.dataGridView1);
            textBox1.Text = a.StateCount.ToString();
            textBox2.Text = a.getTransitionsCount().ToString();
            textBox3.Text = a.getEpsilonTransitionsCount().ToString();
            textBox4.Text = a.InitReference.Destination.Name;
            textBox5.Text = "";
            foreach (char c in a.Alphabet)
            {
                textBox5.Text += c + ", ";
            }
            textBox5.Text = textBox5.Text.Substring(0, textBox5.Text.Length - 2);
            textBox6.Text = "";
            foreach (State s in a.FinalStates)
            {
                textBox6.Text += s.Name + ", ";
            }
            textBox6.Text = textBox6.Text.Substring(0, textBox6.Text.Length - 2);
            textBox13.Text = a.FinalStates.Count.ToString();
            a = a.createAFD();
            a.SetTransitionsTable(this.dataGridView2);
            textBox8.Text = a.StateCount.ToString();
            textBox11.Text = a.getTransitionsCount().ToString();
            textBox12.Text = a.getEpsilonTransitionsCount().ToString();
            textBox10.Text = a.InitReference.Destination.Name;
            textBox9.Text = "";
            foreach (char c in a.Alphabet)
            {
                textBox9.Text += c + ", ";
            }
            textBox9.Text = textBox9.Text.Substring(0, textBox9.Text.Length - 2);
            textBox7.Text = "";
            foreach (State s in a.FinalStates)
            {
                textBox7.Text += s.Name + ", ";
            }
            textBox7.Text = textBox7.Text.Substring(0, textBox7.Text.Length - 2);
            textBox14.Text = a.FinalStates.Count.ToString();

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            // copy text of textbox8 to clipboard
            Clipboard.SetText(textBox8.Text);
        }
    }
}
