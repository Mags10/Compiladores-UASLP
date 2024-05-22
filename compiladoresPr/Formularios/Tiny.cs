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
        ClasifTokens clasifTokens;
        ArbolAnSn arbolAn;
        TinyProcessor tinyProcessor;

        public Tiny(Form1 f)
        {
            InitializeComponent();
            this.f = f;
            String numeroRegex = toolStripTextBox1.Text;
            String identificadorRegex = toolStripTextBox2.Text;
            tinyProcessor = new TinyProcessor(identificadorRegex, numeroRegex);
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

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            TablaAS tablaAS = new TablaAS(this.tinyProcessor.g);
            tablaAS.ShowDialog();
        }

        public void closedClfToken()
        {
            this.clasifTokens = null;
        }

        public void closedArbolAn()
        {
            this.arbolAn = null;
        }

        private void toolStripTextBox2_TextChanged(object sender, EventArgs e)
        {
            if (toolStripTextBox2.Text == "")
            {
                this.richTextBox2.Text = "La expresión regular de los identificadores no puede ser vacía";
                toolStripTextBox2.Focus();
                return;
            }
            try
            {
                this.tinyProcessor.changeIdentifierRegex(toolStripTextBox2.Text);
                this.regexIdentifiers = true;
                //this.richTextBox1_TextChanged(sender, e);
            }
            catch (Exception ex)
            {
                this.regexIdentifiers = false;
                if (this.clasifTokens != null)
                {
                    this.clasifTokens.dataGridView1.Rows.Clear();
                }
                this.richTextBox2.Text = ex.Message;
            }
        }

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (toolStripTextBox1.Text == "")
            {
                this.richTextBox2.Text = "La expresión regular de los números no puede ser vacía";
                toolStripTextBox1.Focus();
                return;
            }
            try {
                this.tinyProcessor.changeNumberRegex(toolStripTextBox1.Text);
                this.regexNumbers = true;
                //this.richTextBox1_TextChanged(sender, e);
            } 
            catch (Exception ex) { 
                this.regexNumbers = false;
                if (this.clasifTokens != null)
                {
                    this.clasifTokens.dataGridView1.Rows.Clear();
                }
                this.richTextBox2.Text = ex.Message;
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (this.clasifTokens == null)
            {
                this.clasifTokens = new ClasifTokens(this);
                this.clasifTokens.Show();
                //this.richTextBox1_TextChanged(sender, e);
            }
            else
            {
                this.clasifTokens.Focus();
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (this.arbolAn == null)
            {
                this.arbolAn = new ArbolAnSn(this);
                this.arbolAn.Show();
            }
            else
            {
                this.arbolAn.Focus();
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (!regexIdentifiers || !regexNumbers)
            {
                richTextBox2.Text = "Es necesario que las expresiones regulares de los identificadores y los números sean válidas";
                return;
            }

            // Revisa todo
            if (tinyProcessor.sanitizeCode(richTextBox1.Text) == "")
            {
                if (this.clasifTokens != null)
                {
                    this.clasifTokens.dataGridView1.Rows.Clear();
                }
                richTextBox2.Text = "No se ha ingresado código";
                return;
            }

            // move cursor to the start and save the selection start
            int realSelectionStart = richTextBox1.SelectionStart;
            richTextBox1.SelectionStart = 0;

            richTextBox2.Text = "";
            String code = richTextBox1.Text;
            this.richTextBox2.Text = "";
            bool hasError = false;

            List<Tuple<String, String>> tokens = tinyProcessor.clasifyTokens(code);
            int i = 0;
            foreach (Tuple<String, String> token in tokens)
            {
                if (token.Item1 == "Error léxico")
                {
                    // find the index of the token in the code by i
                    int index = code.IndexOf(token.Item2, i);
                    int line = richTextBox1.GetLineFromCharIndex(index);
                    int column = index - richTextBox1.GetFirstCharIndexFromLine(line);
                    int selectionStart = richTextBox1.SelectionStart;
                    richTextBox2.Text += "Error léxico en línea " + (line + 1) + " y columna " + (column + 1) + ": No se reconoce el token ' " + token.Item2 + " '\n";
                    /*richTextBox1.Select(index, token.Item2.Length);
                    richTextBox1.SelectionColor = Color.Red;
                    */
                    // Color red
                    richTextBox1.Select(index, token.Item2.Length);
                    richTextBox1.SelectionColor = Color.Red;
                    // Inmediatly after the token back to black
                    richTextBox1.Select(index + token.Item2.Length, 0);
                    richTextBox1.SelectionColor = Color.Black;

                    richTextBox1.SelectionStart = selectionStart;
                    i = index + token.Item2.Length;
                    hasError = true;
                }
                else
                {
                    // Select from current to back and find a space or \n or \r
                    // to set it to black
                    int currentpos = richTextBox1.SelectionStart;
                    int j = currentpos - 1;
                    while (j >= 0 && code[j] != ' ' && code[j] != '\n' && code[j] != '\r')
                    {
                        j--;
                    }
                    j++;
                    int selectionStart = richTextBox1.SelectionStart;
                    richTextBox1.Select(j, currentpos - j);
                    richTextBox1.SelectionColor = Color.Black;
                    richTextBox1.SelectionStart = selectionStart;
                }
            }

            if (this.clasifTokens != null)
            {
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
            }

            if(!hasError)
            {
                richTextBox2.Text = "No se encontraron errores léxicos";
                String tmpCode = "";
                // recorre linea por linea 
                foreach (String line in code.Split('\n'))
                {
                    tmpCode += line + "\t";
                }
                try
                {
                    tinyProcessor.analisisSintactico(tmpCode);

                    Nodo nodo = tinyProcessor.g.tmp;
                    if (this.arbolAn != null)
                    {
                        /*
                            //printTree(tmp);

                            void printTree(Nodo n)
                            {
                                for (int i = 0; i < level; i++) Console.Write("\t");
                                Console.WriteLine(n.token.TokenString);
                                level++;
                                foreach (Nodo h in n.hijos)
                                {
                                    printTree(h);
                                }
                                level--;
                            }
                         */
                        // In treeview
                        arbolAn.treeView1.Nodes.Clear();
                        TreeNode root = new TreeNode(nodo.token.TokenString);
                        arbolAn.treeView1.Nodes.Add(root);
                        printTree(root, nodo);

                        void printTree(TreeNode parent, Nodo n)
                        {
                            foreach (Nodo h in n.hijos)
                            {
                                TreeNode child = new TreeNode(h.token.TokenString.Replace('#', 'ε'));
                                parent.Nodes.Add(child);
                                printTree(child, h);
                            }
                        }

                        // Expand all nodes
                        arbolAn.treeView1.ExpandAll();
                    }


                    richTextBox2.Text += "\nNo se encontraron errores sintácticos";
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("#")){
                        // get number of text #number)
                        int ini = ex.Message.IndexOf("#");
                        int fin = ex.Message.IndexOf(")");
                        int number = int.Parse(ex.Message.Substring(ini + 1, fin - ini - 1));
                        // find the index of the token in the code by i
                        int index = -1;
                        for (int j = 0; j <= number; j++)
                        {
                            index = code.IndexOf(tokens[j].Item2, index + 1);
                        }
                        int line = richTextBox1.GetLineFromCharIndex(index);
                        int column = index - richTextBox1.GetFirstCharIndexFromLine(line);
                        // Obtener error despues de los dos puntos
                        string error = ex.Message.Substring(ex.Message.IndexOf(":") + 1);
                        richTextBox2.Text = "Error sintáctico en línea " + (line + 1) + " y columna " + (column + 1) + ": " + error;
                        int selectionStart = richTextBox1.SelectionStart;
                        richTextBox1.Select(index, tokens[number].Item2.Length);
                        richTextBox1.SelectionColor = Color.Red;
                        richTextBox1.SelectionStart = selectionStart;
                        richTextBox2.Select(richTextBox2.SelectionStart, 0);
                    }
                    //richTextBox2.Text += ex.Message;
                }
            }

            // move cursor to the original position
            richTextBox1.SelectionStart = realSelectionStart;
            richTextBox1.SelectionLength = 0;
            band = true;
        }

        private bool band = false;
        private bool regexIdentifiers = true;
        private bool regexNumbers = true;

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (!band) return;
            band = false;
            if (this.clasifTokens != null)
            {
                this.clasifTokens.dataGridView1.Rows.Clear();
            }
            //this.richTextBox2.Text = "";
            int selectionStart = richTextBox1.SelectionStart;
            // Set all the text to black
            richTextBox1.SelectAll();
            richTextBox1.SelectionColor = Color.Black;

            // Select from current 0
            richTextBox1.Select(selectionStart, 0);

        }
    }
}
