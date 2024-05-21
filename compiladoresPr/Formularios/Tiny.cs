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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace compiladoresPr.Formularios
{
    public partial class Tiny : Form
    {
        Form1 f;
        Gramatica g;
        ClasifTokens clasifTokens;

        public Tiny(Form1 f)
        {
            InitializeComponent();
            this.f = f;
        }

        private void Tiny_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.clasifTokens != null)
            {
                this.clasifTokens.Close();
            }
            this.f.closedTiny();
        }

        private void Tiny_Load(object sender, EventArgs e)
        {
            // set richTextBox1.SelectionTabs to small value to avoid the default 32px tab size
            richTextBox1.SelectionTabs = new int[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            String code = richTextBox1.Text;
            String numeroRegex = toolStripTextBox1.Text;
            String identificadorRegex = toolStripTextBox2.Text;
            TinyProcessor tinyProcessor = new TinyProcessor(identificadorRegex, numeroRegex);

            if (this.clasifTokens != null) {
                if (toolStripTextBox1.Text == "" || toolStripTextBox2.Text == "")
                {
                    return;
                }
                try
                {
                    List<Tuple<String, String>> tokens = tinyProcessor.clasifyTokens(code);
                    // tinyProcessor.printTokens();
                    clasifTokens.dataGridView1.Rows.Clear();
                    clasifTokens.dataGridView1.Columns.Clear();
                    clasifTokens.dataGridView1.Columns.Add("tipo", "lexema");
                    clasifTokens.dataGridView1.Columns.Add("lexema", "tipo");
                    // Auto size the columns
                    clasifTokens.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                    foreach (Tuple<String, String> token in tokens)
                    {
                        clasifTokens.dataGridView1.Rows.Add(token.Item1, token.Item2);
                    }
                }catch (Exception ex)
                {
                    clasifTokens.dataGridView1.Rows.Clear();
                }
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            TablaAS tablaAS = new TablaAS(this.g);
            tablaAS.ShowDialog();
        }

        public void closedClfToken()
        {
            this.clasifTokens = null;
        }

        private void toolStripTextBox2_TextChanged(object sender, EventArgs e)
        {
            this.richTextBox1_TextChanged(sender, e);
        }

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            this.richTextBox1_TextChanged(sender, e);
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (this.clasifTokens == null)
            {
                this.clasifTokens = new ClasifTokens(this);
                this.clasifTokens.Show();
                this.richTextBox1_TextChanged(sender, e);
            }
            else
            {
                this.clasifTokens.Focus();
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
        }
    }
}
