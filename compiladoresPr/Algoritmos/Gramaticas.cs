using compiladoresPr.Formularios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace compiladoresPr.Algoritmos
{
    internal class Gramatica
    {
        private List<Token> terminales;
        private List<Token> noTerminales;
        private List<Produccion> producciones;
        private List<List<Produccion>> resTable = new List<List<Produccion>>();
        List<SigPrim> primeros;
        List<SigPrim> siguientes;
        private Produccion inicial = null;
        private String primLog = "";
        private String sigLog = "";
        private String tablaLog = "";

        public String PrimLog
        {
            get { return primLog.Replace('#', 'ε'); }
            set { primLog = value; }
        }
        public String SigLog
        {
            get { return sigLog.Replace('#', 'ε'); }
            set { sigLog = value; }
        }
        public String TablaLog
        {
            get { return tablaLog.Replace('#', 'ε'); }
            set { tablaLog = value; }
        }

        public Gramatica()
        {
            terminales = new List<Token>();
            noTerminales = new List<Token>();
            producciones = new List<Produccion>();
        }

        public void addProduccion(Produccion produccion)
        {
            if (inicial == null) inicial = produccion;

            producciones.Add(produccion);

            auxAdder(produccion.Productor);

            foreach (List<Token> p in produccion.Producciones) foreach (Token t in p) auxAdder(t);

            if (!Token.TokenListContains(noTerminales, produccion.Productor)) noTerminales.Add(produccion.Productor);

            void auxAdder(Token t) {
                if (t.TokenString == "#") return;
                if (t.Terminal && !Token.TokenListContains(terminales, t)) terminales.Add(t);
                //else if (!t.Terminal && !Token.TokenListContains(noTerminales, t)) noTerminales.Add(t);
            }
        }

        public String primString()
        {
            String res = "";
            foreach (SigPrim t in primeros)
            {
                res += "Primeros(" + t.Token + ") = {" + string.Join(", ", t.TokenList).Replace('#', 'ε') + "}\n";
            }
            return res;
        }

        public String sigString()
        {
            String res = "";
            foreach (SigPrim t in siguientes)
            {
                res += "Siguientes(" + t.Token + ") = {" + string.Join(", ", t.TokenList).Replace('#', 'ε') + "}\n";
            }
            res.Replace('#', 'ε');
            return res;
        }

        public override string ToString()
        {
            string gram = "\n\n";
            gram += "-------------------------------------";
            gram += "\nProducciones: \n" + string.Join("\n", producciones);
            gram += "\n\nInicial: {" + inicial.Productor + "}";
            gram += "\n\nTerminales: {" + string.Join(", ", terminales) + "}";
            gram += "\n\nNo Terminales: {" + string.Join(", ", noTerminales) + "}";
            gram += "\n\nPrimeros: \n";
            foreach (SigPrim t in primeros)
                gram += "Primero(" + t.Token + ") = {" + string.Join(", ", t.TokenList) + "}\n";
            gram += "\nSiguientes: \n";
            foreach (SigPrim s in siguientes)
                gram += "Siguiente(" + s.Token + ") = {" + string.Join(", ", s.TokenList) + "}\n";
            return gram;
        }

        public void calcPrimeros()
        {

            int level = 0;

            primeros = new List<SigPrim>();
            string log = "";

            foreach (Token t in terminales) primeros.Add(new SigPrim(t, t));

            foreach (Produccion p in producciones) getPrimeros(p);

            this.primLog = log;
            //Console.WriteLine(log);

            List<Token> getPrimeros(Produccion p)
            {
                // insert \t level times
                string tmplog = "";
                for (int i = 0; i < level; i++) tmplog += "\t";
                tmplog += "|";
                log += tmplog + "=====================================\n";
                List<Token> res = getPrimerosAux(p.Productor);
                log += tmplog + "Calculando primeros de: " + p.Productor + "\n";
                if (res == null)
                {
                    res = new List<Token>();
                }
                else
                {
                    log += tmplog + "Ya se calcularon los primeros de: " + p.Productor + "\n";
                    return res;
                }
                bool endBand = false;
                foreach (List<Token> prod in p.Producciones)
                {
                    log += tmplog + "Produccion: " + p.Productor + " → " + string.Join(" ", prod) + "\n";

                    foreach (Token t in prod)
                    {
                        List<Token> preres = getPrimerosAux(t);
                        if (preres == null)
                        {
                            preres = new List<Token> { new Token("#", true) };
                            if (t.TokenString != "#")
                            {
                                level++;
                                Produccion p2 = getProduction(t);
                                preres = getPrimeros(p2);
                                level--;
                            }
                        }

                        log += tmplog + "Token (" + t + ") añade: {" + string.Join(" ", preres) + "}\n";

                        foreach (Token tok in preres) if (!Token.TokenListContains(res, tok)) res.Add(tok);
                        endBand = !Token.DeleteToken(res, new Token("#", true));
                        if (endBand) break;
                    }
                    log += tmplog;
                    log += (endBand)? "No se añade #\n" : "Se añade #\n";
                    if (!endBand) res.Add(new Token("#", true));
                }
                primeros.Add(new SigPrim(p.Productor, res));
                log += tmplog + "Primeros(" + p.Productor + ") = {" + string.Join(", ", res) + "}\n";
                log += tmplog + "=====================================\n";
                return res;
            }

            List<Token> getPrimerosAux(Token token)
            {
                foreach (SigPrim tup in primeros)
                    if (Token.CompareToken(tup.Token, token))
                        return tup.TokenList;
                return null;
            }
        }

        private Produccion getProduction(Token productor)
        {
            foreach (Produccion p in producciones)
            {
                if (p.compareProductor(productor))
                {
                    return p;
                }
            }
            return null;
        }
        public List<List<Produccion>> getProductions()
        {
            return resTable;
        }
        public void calcSiguientes()
        {
            int level = 0;
            string log = "|==========================================================================\n";

            siguientes = new List<SigPrim>();
            List<SigPrim> calculando = new List<SigPrim>();

            foreach (Token t in noTerminales)
            {
                SigPrim s = new SigPrim(t);
                if (Token.CompareToken(t, inicial.Productor))
                {
                    s.TokenList.Add(new Token("$", true));
                    log += "|Añadiendo $ a simbolo inicial " + t + "\n";
                }
                siguientes.Add(s);
            }

            foreach (SigPrim sg in siguientes)
            {
                if (!sg.calculated)
                {
                    calculando.Add(sg);
                    calcSiguienteAux(sg);
                }
            }

            log += "|==========================================================================\n";
            this.sigLog = log;

            void calcSiguienteAux(SigPrim sg)
            {
                // insert \t level times
                string tmplog = "";
                for (int i = 0; i < level; i++) tmplog += "\t";
                tmplog += "|";
                log += tmplog + "==========================================================================\n";
                log += tmplog + "No terminal: " + sg.Token + "\n";
                List<Tuple<Token, List<Token>>> tmp = getProdsAux(sg.Token);
                if (tmp.Count == 0) log += tmplog + "  No aparece en ninguna produccion\n";
                else log += tmplog + "  Producciones donde aparece: \n";
                foreach (Tuple<Token, List<Token>> tup in tmp)
                {
                    List<Token> beta = getBeta(tup, sg.Token);
                    List<Token> primerosBeta = getPrimeros(beta);

                    log += tmplog + "    " + tup.Item1 + " → " + string.Join(" ", tup.Item2) + "\n";
                    if (beta.Count != 0)
                    {
                        log += tmplog + "\t   - Beta: " + string.Join(" ", beta) + "\n";
                        log += tmplog + "\t   - Primeros(" + string.Join(" ", beta) + ") = {" + string.Join(", ", primerosBeta) + "}\n";
                    }

                    //Console.Write(" Sig(" + sg.Token + ") = {");
                    bool band = false;
                    if (beta.Count != 0) {
                        foreach(Token tk in primerosBeta)
                        {
                            if (tk.TokenString != "#")
                            {
                                if (!Token.TokenListContains(sg.TokenList, tk))
                                    sg.TokenList.Add(tk);
                                //Console.Write(tk + ", ");
                            } else {
                                band = true;
                                log += tmplog + "\t   - Existe # en primeros de beta\n";
                            }
                        }
                    }
                    if (band || beta.Count == 0)
                    {
                        //Console.Write("Sig(" + sg.Token + ") = {");
                        SigPrim s = getSiguiente(tup.Item1);
                        
                        if (!SigPrim.SiguienteListContains(calculando, s) && !s.calculated)
                        {
                            // Si s en segundoslist de sg entonces hay un ciclo, no calcular
                            if (!Token.TokenListContains(sg.TokenList, s.Token))
                            {
                                log += tmplog + "Calculando siguientes de: " + s.Token + "\n";
                                level++;
                                calcSiguienteAux(s);
                                level--;
                            }
                        }
                        log += tmplog + "\t   - Siguientes(" + tup.Item1 + ") = {" + string.Join(", ", s.TokenList) + "}\n";
                        foreach (Token t in s.TokenList)
                        {
                            if (!Token.TokenListContains(sg.TokenList, t))
                            {
                                sg.TokenList.Add(t);
                            }
                        }
                    }
                }
                sg.calculated = true;
                log += tmplog + "  Siguientes(" + sg.Token + ") = {" + string.Join(", ", sg.TokenList) + "}\n";
            }

            SigPrim getSiguiente(Token t)
            {
                foreach (SigPrim s in siguientes)
                {
                    if (Token.CompareToken(s.Token, t))
                    {
                        return s;
                    }
                }
                return null;
            }

            List<Token> getPrimeros(List<Token> list)
            {
                List<Token> res = new List<Token>();
                foreach (Token t in list)
                {
                    bool band = false;
                    foreach (SigPrim tup in primeros)
                    {
                        if (Token.CompareToken(tup.Token, t))
                        {
                            //res.AddRange(tup.Item2); // Sin repetir
                            foreach (Token tok in tup.TokenList)
                            {
                                if (!Token.TokenListContains(res, tok))
                                {
                                    res.Add(tok);
                                    // Check if token is #
                                    if (tok.TokenString == "#")
                                    {
                                        band = true;
                                    }
                                }
                               
                            }
                            // End foreach
                            break;
                        }
                    }
                    if (!band)
                    {
                        break;
                    }
                }
                return res;
            }

            List<Token> getBeta(Tuple<Token, List<Token>> tup, Token cab)
            {
                List<Token> beta = new List<Token>();
                bool band = false;
                foreach (Token t in tup.Item2)
                {
                    if (Token.CompareToken(t, cab))
                    {
                        band = true;
                        continue;
                    }
                    if (band)
                    {
                        beta.Add(t);
                    }
                }
                return beta;
            }

            List<Tuple<Token,List<Token>>> getProdsAux(Token cab)
            {
                List<Tuple<Token, List<Token>>> res = new List<Tuple<Token, List<Token>>>();
                foreach (Produccion p in producciones)
                {
                    foreach (List<Token> prod in p.Producciones)
                    {
                        foreach (Token t in prod)
                        {
                            if (Token.CompareToken(t, cab))
                            {
                                res.Add(new Tuple<Token, List<Token>>(p.Productor, prod));
                            }
                        }
                    }
                }
                return res;
            }

        }

        public void calcTabla()
        {
            string log = "|==========================================================================\n";
            for (int i = 0; i < noTerminales.Count; i++)
            {
                List<Produccion> row = new List<Produccion>();
                for (int j = 0; j < terminales.Count + 1; j++)
                {
                    row.Add(null);
                }
                resTable.Add(row);
            }
            foreach (Produccion produccion in producciones)
            {
                foreach (List<Token> p in produccion.Producciones)
                {
                    List<Token> sigPrims = primOf(p);
                    log += "|Produccion: " + produccion.Productor + " -> " + string.Join(" ", p) + "\n";
                    log += "|  Primeros(" + string.Join("", p) + ") = {" + string.Join(", ", sigPrims) + "}\n";
                    if (sigPrims.Count != 0)
                    {
                        bool band = false;
                        foreach (Token s in sigPrims)
                        {
                            if (s.TokenString != "#")
                            {
                                int row = getRow(produccion.Productor);
                                int col = getCol(s);
                                resTable[row][col] = new Produccion(produccion.Productor.TokenString, p);
                                log += "|   - Tabla[" + produccion.Productor + ", " + s + "] = " + resTable[row][col] + "\n";
                            }
                            else
                            {
                                band = true;
                            }
                        }
                        if (band)
                        {
                            List<Token> sigs = getSiguiente(produccion.Productor);
                            log += "|  Existe # en primeros de la produccion\n";
                            log += "|  Siguientes(" + produccion.Productor + ") = {" + string.Join(", ", sigs) + "}\n";
                            foreach (Token s in sigs)
                            {
                                int row = getRow(produccion.Productor);
                                int col = getCol(s);
                                resTable[row][col] = new Produccion(produccion.Productor.TokenString, p);
                                log += "|   - Tabla[" + produccion.Productor + ", " + s + "] = " + resTable[row][col] + "\n";
                            }
                        }
                    }
                    log += "|==========================================================================\n";
                }
            }

            this.tablaLog = log;

            int getCol(Token t)
            {
                for (int i = 0; i < terminales.Count; i++)
                {
                    if (Token.CompareToken(terminales[i], t))
                    {
                        return i;
                    }
                }
                if (t.TokenString == "$") return terminales.Count;
                return -1;
            }

            int getRow(Token t)
            {
                for (int i = 0; i < noTerminales.Count; i++)
                {
                    if (Token.CompareToken(noTerminales[i], t))
                    {
                        return i;
                    }
                }
                return -1;
            }

            List<Token> getSiguiente(Token t)
            {
                foreach (SigPrim s in siguientes)
                {
                    if (Token.CompareToken(s.Token, t))
                    {
                        return s.TokenList;
                    }
                }
                return new List<Token>();
            }

            List<Token> primOf(List<Token> list)
            {
                List<Token> res = new List<Token> { };   
                foreach (Token t in list)
                {
                    foreach (SigPrim tup in primeros)
                    {
                        if (Token.CompareToken(tup.Token, t))
                        {
                            bool band = Token.TokenListContains(tup.TokenList, new Token("#", true));
                            foreach (Token tok in tup.TokenList)
                            {
                                if (!Token.TokenListContains(res, tok) && tok.TokenString != "#")
                                {
                                    res.Add(tok);
                                }
                            }
                            if (!band)
                            {
                                return res;
                            }
                        }
                    }
                }
                res.Add(new Token("#", true));
                return res;
            }
            
        }
        public DataGridView TablaAS(Form fm)
        {
            // Altura de formulario 20*noTerminales.Count+20
            fm.Height = 20 * noTerminales.Count + 44;
            DataGridView dataGridView1 = ((TablaAS)fm).TabAS;
            dataGridView1.ColumnCount = terminales.Count + 1;
            dataGridView1.Columns[0].Name = "";
            
            for (int i = 0; i < terminales.Count; i++)
            {
                dataGridView1.Columns[i].Name = terminales[i].TokenString;                
            }
            dataGridView1.Columns[terminales.Count].Name = "$";

            
            for (int i = 0; i < noTerminales.Count; i++)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridView1);
                row.Cells[0].Value = noTerminales[i].TokenString;
                for (int j = 0; j < terminales.Count + 1; j++)
                {
                    row.Cells[j].Value = resTable[i][j]?.ToString().Replace('#', 'ε') ?? "";
                }
                dataGridView1.Rows.Add(row);
                dataGridView1.Rows[i].HeaderCell.Value = noTerminales[i].TokenString;
            }
            return dataGridView1;

        }

    }

    internal class Produccion
    {
        private Token productor;
        private List<List<Token>> producciones;

        public Produccion(Token productor, List<List<Token>> producciones)
        {
            this.productor = productor;
            this.producciones = producciones;
        }

        public Produccion(String productor, List<Token> produccion)
        {
            this.productor = new Token(productor, false);
            this.producciones = new List<List<Token>> { produccion };
        }

        public Produccion(String productor)
        {
            this.productor = new Token(productor, false);
            this.producciones = new List<List<Token>>();
        }

        public void addProduccion(List<Token> produccion)
        {
            this.producciones.Add(produccion);
        }

        public void addProduccion(params object[] tokens)
        {
            this.producciones.Add(Token.createProduction(tokens));
        }

        public List<List<Token>> Producciones
        {
            get { return producciones; }
            set { producciones = value; }
        }

        public Token Productor
        {
            get { return productor; }
            set { productor = value; }
        }

        public bool compareProductor(Token t)
        {
            // Check string and terminal
            return productor.TokenString == t.TokenString && productor.Terminal == t.Terminal;
        }

        public override string ToString()
        {
            string prod = "" + productor + " -> ";
            foreach (List<Token> p in producciones)
            {
                foreach (Token t in p)
                {
                    prod += t.ToString() + " ";
                }
                if (p != producciones.Last()) prod += "| ";
            }
            return prod;
        }

    }

    internal class SigPrim
    {
        public bool calculated = false;
        public Token Token;
        public List<Token> TokenList;
        
        public SigPrim(Token token)
        {
            Token = token;
            TokenList = new List<Token>();
        }

        public SigPrim(Token token, List<Token> tokenList)
        {
            Token = token;
            TokenList = tokenList;
        }

        // recive token and a indefine list of Tokens
        public SigPrim(Token token, params object[] tokens)
        {
            Token = token;
            TokenList = new List<Token>();
            foreach (object t in tokens)TokenList.Add((Token)t);
        }


        public static bool CompareSiguiente(SigPrim s1, SigPrim s2)
        {
            return Token.CompareToken(s1.Token, s2.Token);
        }

        public static bool SiguienteListContains(List<SigPrim> list, SigPrim s)
        {
            foreach (SigPrim sg in list)
            {
                if (CompareSiguiente(sg, s))
                {
                    return true;
                }
            }
            return false;
        }
    }

    internal class Token
    {
        private string token;
        private bool terminal;

        public Token(string token, bool terminal)
        {
            this.token = token;
            this.terminal = terminal;
        }

        public string TokenString
        {
            get { return token; }
            set { token = value; }
        }

        public bool Terminal
        {
            get { return terminal; }
            set { terminal = value; }
        }

        public override string ToString()
        {
            return token;
        }

        // From a n number of string and bool, create a list of tokens
        public static List<Token> createProduction(params object[] tokens)
        {
            if (tokens.Length % 2 != 0) throw new ArgumentException("The number of arguments must be even");
            List<Token> list = new List<Token>();
            for (int i = 0; i < tokens.Length; i += 2)
            {
                list.Add(new Token((string)tokens[i], (bool)tokens[i + 1]));
            }
            return list;
        }

        public static bool CompareToken(Token t1, Token t2)
        {
            return t1.TokenString == t2.TokenString && t1.Terminal == t2.Terminal;
        }

        public static bool TokenListContains(List<Token> list, Token t)
        {
            foreach (Token tok in list)
            {
                if (CompareToken(tok, t))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool DeleteToken(List<Token> list, Token t)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (CompareToken(list[i], t))
                {
                    list.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

    }
}
