using compiladoresPr.Formularios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace compiladoresPr.Algoritmos
{
    public class Gramatica
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
        public Nodo tmp;

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

        public List<String> terminalesString()
        {
            return terminales.Select(t => t.TokenString).ToList();
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
            // Iniializa la tabla
            for (int i = 0; i < noTerminales.Count; i++)
            {
                List<Produccion> row = new List<Produccion>();
                for (int j = 0; j < terminales.Count + 1; j++)
                {
                    row.Add(null);
                }
                resTable.Add(row);
            }
            // Se recorren producciones
            foreach (Produccion produccion in producciones)
            {
                foreach (List<Token> p in produccion.Producciones)
                {
                    // Se obtienen los primeros de la produccion
                    List<Token> sigPrims = primOf(p);
                    log += "|Produccion: " + produccion.Productor + " -> " + string.Join(" ", p) + "\n";
                    log += "|  Primeros(" + string.Join("", p) + ") = {" + string.Join(", ", sigPrims) + "}\n";
                    // Se recorren los primeros
                    if (sigPrims.Count != 0)
                    {
                        bool band = false;
                        // Para cada token (terminal) en los primeros se añade la produccion a la tabla
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
                        // Si # esta en los primeros se añaden los siguientes
                        if (band)
                        {
                            List<Token> sigs = getSiguiente(produccion.Productor);
                            log += "|  Existe # en primeros de la produccion\n";
                            log += "|  Siguientes(" + produccion.Productor + ") = {" + string.Join(", ", sigs) + "}\n";
                            // Para cada token (terminal) en los siguientes se añade la produccion a la tabla
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

        public void analisisSintactico(String cadena, TinyProcessor tinyProcessor)
        {
            //Console.WriteLine("Analisis Sintactico: " + cadena);
            tmp = new Nodo(inicial.Productor);
            //TinyProcessor tinyProcessor = new TinyProcessor("[a-z]+", "[0-9]+");
            List<String> tokens = cadena.Split(' ').ToList();
            int cursor = 0;
            tokens.Add("$");

            Stack<Token> pila = new Stack<Token>();
            pila.Push(new Token("$", true));
            pila.Push(inicial.Productor);

            String a = tokens[cursor];
            Token X = pila.Peek();

            while (X.TokenString != "$")
            {
                int xindex = indexOf(noTerminales, X);
                Console.WriteLine("clasi: " + tinyProcessor.classifyToken(a));
                int aindex = indexOf(terminales, new Token(tinyProcessor.classifyToken(a), true));
                if (aindex == -1 && a == "$")
                {
                    aindex = terminales.Count;
                }
                Console.WriteLine("X: " + X + " A: " + a);
                if (aindex < terminales.Count && X.TokenString == terminales[aindex].TokenString)
                {
                    //Console.WriteLine("Empareja");
                    pila.Pop();
                    cursor++;
                    a = tokens[cursor];

                    while (true)
                    {
                        tmp = tmp.parent;
                        tmp.cursor++;
                        if (tmp.cursor < tmp.hijos.Count)
                        {
                            tmp = tmp.hijos[tmp.cursor];
                            break;
                        }
                    }
                }
                else if (X.Terminal)
                {
                    //Console.WriteLine("Error de sintaxis - terminal");
                    if (tokens[cursor] == "$") cursor--;
                    if (cursor - 1 >= 0)
                        throw new Exception("Error sintactico (Token #" + cursor + "): No se esperaba ' " + tokens[cursor] + " ' despues de ' " + tokens[cursor - 1] + " '");
                    else
                        throw new Exception("Error sintactico (Token #" + cursor + "): No se esperaba ' " + tokens[cursor] + " '");
                }
                else if (resTable[xindex][aindex] == null)
                {
                    //Console.WriteLine("Error de sintaxis - null");
                    if (tokens[cursor] == "$") cursor--;
                    if (cursor-1 >= 0)
                        throw new Exception("Error sintactico (Token #" + cursor + "): No se esperaba ' " + tokens[cursor] + " ' despues de ' " + tokens[cursor - 1] + " '");
                    else
                        throw new Exception("Error sintactico (Token #" + cursor + "): No se esperaba ' " + tokens[cursor] + " '");
                }
                else
                {
                    //Console.WriteLine("SALIDA Produccion: " + resTable[xindex][aindex]);
                    
                    pila.Pop();
                    List<Token> prod = resTable[xindex][aindex].Producciones[0];
                    for (int i = prod.Count - 1; i >= 0; i--)
                    {
                        if (prod[i].TokenString != "#")
                        {
                            pila.Push(prod[i]);
                        }
                    }
                    foreach (Token t in prod)
                    {
                        tmp.addHijo(t);
                    }
                    if (tmp.hijos[tmp.cursor].token.TokenString != "#")
                    {
                        tmp = tmp.hijos[tmp.cursor];
                    }
                    else
                    {
                        while (true && tmp.parent != null)
                        {
                            tmp = tmp.parent;
                            tmp.cursor++;
                            if (tmp.cursor < tmp.hijos.Count && tmp.hijos[tmp.cursor].token.TokenString != "#")
                            {
                                tmp = tmp.hijos[tmp.cursor];
                                break;
                            }
                        }
                    }
                }
                X = pila.Peek();
                //Console.WriteLine("\nPila: " + string.Join(" ", pila) + "\n");
            }

            // Imprimir arbol
            int level = 0;

            //printTree(tmp);

            void printTree(Nodo n)
            {
                for (int i = 0; i < level; i++) Console.Write(" ");
                Console.WriteLine("|" + n.token.TokenString);
                level++;
                foreach (Nodo h in n.hijos)
                {
                    printTree(h);
                }
                level--;
            }

            int indexOf(List<Token> list, Token t)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (Token.CompareToken(list[i], t))
                    {
                        return i;
                    }
                }
                return -1;
            }

        }



        // Completamente inutil
        public void elementosLR()
        {
            Produccion inic = new Produccion(this.inicial.Productor.TokenString + "'");
            inic.addProduccion(".", true, this.inicial.Productor.TokenString, false);

            List<String> header = new List<String>() { "I0" };
            List<List<Produccion>> res = new List<List<Produccion>>();
            List<Produccion> C = Cerradura(this.producciones, new List<Produccion>() { inic });
            
            List<Produccion> Jr = new List<Produccion>();
            Jr.AddRange(C);
            res.Add(Jr);

            List<Token> simbolos = new List<Token>();
            simbolos.AddRange(this.terminales);
            simbolos.AddRange(this.noTerminales);

            bool added = true;
            int k = 0;
            while (added)
            {
                added = false;
                for (; k < res.Count; )
                {
                    foreach (Produccion produccion in res[k])
                    { 
                        foreach (List<Token> prd in produccion.Producciones)
                        {
                            List<Produccion> I = res[k];
                            Console.WriteLine("Lista: " + string.Join(", ", I));
                            Console.WriteLine("Iterado: " + k);
                            foreach (Token X in simbolos)
                            {
                                List<Produccion> J = irA(I, X);
                                if (J.Count != 0 && !ProduccionListContainsList(res, J))
                                {
                                    Console.WriteLine("Añadiendo: " + string.Join(", ", J));
                                    res.Add(J);
                                    added = true;
                                    header.Add("I" + res.IndexOf(J));
                                    Console.WriteLine("Transicion: I" + k + " - (" + X + ") -> I" + res.IndexOf(J));
                                }
                            }
                            k++;
                        }
                    }
                }
            }

            Console.WriteLine("Elementos LR: ");
            for(int i = 0; i < res.Count; i++)
            {
                Console.WriteLine("I" + i + " = {" + string.Join(", ", res[i]) + "}");
            }

            List<Produccion> irA(List<Produccion> I, Token X)
            {
                Console.WriteLine("\n********-------------------********");
                Console.WriteLine("IrA([" + string.Join(", ", I) + "], " + X + ")");
                List<Produccion> resir = new List<Produccion>();
                foreach (Produccion produccion in I)
                {
                    foreach (List<Token> p in produccion.Producciones)
                    {
                        //Console.WriteLine(produccion.Productor + " -> " + string.Join(" ", p));
                        for (int i = 0; i < p.Count; i++)
                        {
                            if (p[i].TokenString == ".")
                            {
                                //Console.WriteLine("Se encontro .");
                                //if (i + 1 < p.Count) Console.WriteLine("Siguiente: " + p[i + 1]);
                                if (i + 1 < p.Count && Token.CompareToken(p[i + 1], X))
                                {
                                    Console.WriteLine("Se encontro X, Intercambiando");
                                    List<Token> tmp2 = new List<Token>();
                                    tmp2.AddRange(p);
                                    tmp2[i] = p[i + 1];
                                    tmp2[i + 1] = new Token(".", true);
                                    Produccion p2 = new Produccion(produccion.Productor.TokenString);
                                    p2.addProduccion(tmp2);
                                    Console.WriteLine("Produccion: " + p2);
                                    // return Cerradura(this.producciones, new List<Produccion>() { p2 });
                                    List<Produccion> tmpr = Cerradura(this.producciones, new List<Produccion>() { p2 }); 
                                    resir.AddRange(tmpr);
                                }
                            }
                        }
                    }
                }
                Console.WriteLine("Res = {" + string.Join(", ", resir) + "}");
                Console.WriteLine("********-------------------********\n");
                return resir;
            }

            List<Produccion> Cerradura(List<Produccion> producciones, List<Produccion> cerradura)
            {
                Console.WriteLine("-------------------");
                Console.WriteLine("Cerradura(" + string.Join(", ", cerradura) + ")");
                Console.WriteLine("Añadiendo: " + string.Join(", ", cerradura));
                List<Produccion> J = new List<Produccion>();
                J.AddRange(cerradura);
                bool band = true;
                while (band)
                {
                    band = false;
                    for (int i = 0; i < J.Count; i++)
                    {
                        Produccion p = J[i];
                        foreach (List<Token> prd in p.Producciones) { 
                            Token B = getBeta(prd);
                            if (B != null && !B.Terminal)
                            {
                                Produccion p2 = getProduction(B);
                                //Console.WriteLine("Produccion: " + p2);

                                if (p2 != null)
                                {
                                    //Console.WriteLine("Beta: " + B);
                                    foreach (List<Token> prd2 in p2.Producciones)
                                    {
                                        // Prd2 tiene la forma B -> gamma
                                        // Se añade B -> .gamma a J
                                        Produccion p3 = new Produccion(B.TokenString);
                                        p3.addProduccion(".", true);
                                        if (prd2[0].TokenString != "#")
                                        {
                                            p3.Producciones[0].AddRange(prd2);
                                        }

                                        if (!ProduccionListContains(J, p3))
                                        {
                                            J.Add(p3);
                                            Console.WriteLine("Añadiendo: " + p3);
                                            band = true;
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
                //Console.WriteLine("Cerradura: " + string.Join(", ", J));
                Console.WriteLine("-------------------");
                return J;
            }

            bool ProduccionListContainsList(List<List<Produccion>> list, List<Produccion> p)
            {
                foreach (List<Produccion> pr in list)
                {
                    foreach (Produccion pr2 in pr)
                    {
                        if (!ProduccionListContains(p, pr2))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }

            bool ProduccionListContains(List<Produccion> list, Produccion p)
            {
                foreach (Produccion pr in list)
                {
                    if (pr.ToString() == p.ToString())
                    {
                        return true;
                    }
                }
                return false;
            }

            Token getBeta(List <Token> prod)
            {
                for (int i = 0; i < prod.Count; i++)
                {
                    if (prod[i].TokenString == ".")
                    {
                        if (i + 1 < prod.Count)
                        {
                            return prod[i + 1];
                        }
                    }
                }
                return null;
            }
        }

    }

    public class Nodo
    {
        public Token token;
        public Nodo parent;
        public List<Nodo> hijos;
        public int cursor = 0;
        public Nodo(Token cab)
        {
            //Console.WriteLine("Creando nodo: " + cab.TokenString);
            this.token = cab;
            this.hijos = new List<Nodo>();
        }
        public Nodo(Token cab, Nodo parent)
        {
            this.token = cab;
            this.parent = parent;
            this.hijos = new List<Nodo>();
        }
        public void addHijo(Token t)
        {
            //Console.WriteLine("Añade hijo a " + this.token.TokenString + ": " + t.TokenString);
            this.hijos.Add(new Nodo(t, this));
        }
    }


    public class Produccion
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

    public class Token
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
            //Console.WriteLine("Comparing: " + t1.TokenString + "=" + t2.TokenString + " & " + t1.Terminal + "=" + t2.Terminal);
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
