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
    public partial class Tiny : Form
    {
        Form1 f;
        public Tiny(Form1 f)
        {
            InitializeComponent();
            this.f = f;
        }

        private void Tiny_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.f.closedTiny();
        }

        private void Tiny_Load(object sender, EventArgs e)
        {
            // set richTextBox1.SelectionTabs to small value to avoid the default 32px tab size
            richTextBox1.SelectionTabs = new int[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox5.Text == "")
            {
                return;
            }
            String code = richTextBox1.Text;
            String numeroRegex = textBox1.Text;
            String identificadorRegex  = textBox5.Text;
            try
            {
                TinyProcessor tinyProcessor = new TinyProcessor(code, identificadorRegex, numeroRegex);
                List<Tuple<String, String>> tokens = tinyProcessor.clasifyTokens();
                // tinyProcessor.printTokens();
                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();
                dataGridView1.Columns.Add("tipo", "lexema");
                dataGridView1.Columns.Add("lexema", "tipo");
                // Auto size the columns
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                foreach (Tuple<String, String> token in tokens)
                {
                    dataGridView1.Rows.Add(token.Item1, token.Item2);
                }
            }catch (Exception ex)
            {
                dataGridView1.Rows.Clear();
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            this.richTextBox1_TextChanged(sender, e);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.richTextBox1_TextChanged(sender, e);
        }
    }
}
