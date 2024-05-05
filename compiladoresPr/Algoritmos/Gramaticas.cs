using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiladoresPr.Algoritmos
{
    internal class Gramatica
    {
        private List<Token> terminales;
        private List<Token> noTerminales;
        private List<Produccion> producciones;
        List<SigPrim> primeros;
        List<SigPrim> siguientes;
        private Produccion inicial = null;

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

            void auxAdder(Token t) {
                if (t.TokenString == "#") return;
                if (t.Terminal && !Token.TokenListContains(terminales, t)) terminales.Add(t);
                else if (!t.Terminal && !Token.TokenListContains(noTerminales, t)) noTerminales.Add(t);
            }
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

            Console.WriteLine(log);

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
                    log += tmplog + "Produccion: " + p.Productor + " -> " + string.Join(" ", prod) + "\n";

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

        public void calcSiguientes()
        {

            string log = "";

            siguientes = new List<SigPrim>();
            List<SigPrim> calculando = new List<SigPrim>();

            foreach (Token t in noTerminales)
            {
                SigPrim s = new SigPrim(t);
                if (Token.CompareToken(t, inicial.Productor))
                {
                    s.TokenList.Add(new Token("$", true));
                    log += "Añadiendo $ a siguientes de " + t + "\n";
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

            void calcSiguienteAux(SigPrim sg)
            {

                Console.WriteLine("****************************************");
                // Print
                List<Tuple<Token, List<Token>>> tmp = getProdsAux(sg.Token);
                Console.WriteLine("Producciones donde aparece: " + sg.Token);
                foreach (Tuple<Token, List<Token>> tup in tmp)
                {
                    Console.Write(tup.Item1 + " -> ");
                    foreach (Token tok in tup.Item2)
                    {
                        Console.Write(tok + " ");
                    }

                    List<Token> beta = getBeta(tup, sg.Token);

                    Console.Write(" <-> Beta: ");
                    foreach (Token tok in beta)
                    {
                        Console.Write(tok + " ");
                    }

                    Console.Write(" = {");

                    List<Token> primerosBeta = getPrimeros(beta);

                    foreach (Token tok in primerosBeta)
                    {
                        Console.Write(tok + " ");
                    }

                    Console.WriteLine("}");

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
                                Console.WriteLine("Calculando siguientes de: " + s.Token);
                                calcSiguienteAux(s);
                            }
                        }

                        foreach (Token t in s.TokenList)
                        {
                            if (!Token.TokenListContains(sg.TokenList, t))
                            {
                                sg.TokenList.Add(t);
                                //Console.Write(t + ", ");
                                //Console.WriteLine("Añadiendo " + t + " a siguientes de " + sg.Token);
                            }
                        }

                        //Console.WriteLine("}");
                    }
                    sg.calculated = true;
                    Console.WriteLine();
                }

                Console.Write("Sig(" + sg.Token + ") = {");
                foreach (Token t in sg.TokenList)
                {
                    Console.Write(t + ", ");
                }
                Console.WriteLine("}");
                Console.WriteLine("----------------------------------------");
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
                                }
                            }
                        }
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
            List<List<Produccion>> resTable = new List<List<Produccion>>();
            // Create table NxM where N = noTerminales.Count and M = terminales.Count
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
                Console.WriteLine("Produccion: " + produccion);
                foreach (List<Token> p in produccion.Producciones)
                {
                    Console.WriteLine("Produccion: " + string.Join(" ", p));
                    List<Token> sigPrims = primOf(p);
                    Console.WriteLine("Primeros(" + string.Join("", p) + ") = {" + string.Join(", ", sigPrims) + "}");
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
                                Console.WriteLine("Tabla[" + produccion.Productor + ", " + s + "] = " + resTable[row][col]);
                            }
                            else
                            {
                                band = true;
                            }
                        }
                        if (band)
                        {
                            List<Token> sigs = getSiguiente(produccion.Productor);
                            Console.WriteLine("Siguientes(" + produccion.Productor + ") = {" + string.Join(", ", sigs) + "}");
                            foreach (Token s in sigs)
                            {
                                int row = getRow(produccion.Productor);
                                int col = getCol(s);
                                Console.WriteLine("row: " + row + " col: " + col);
                                resTable[row][col] = new Produccion(produccion.Productor.TokenString, p);
                                Console.WriteLine("Tabla[" + produccion.Productor + ", " + s + "] = " + resTable[row][col]);
                            }
                        }
                    }
                }
            }

            Console.WriteLine("Tabla: ");
            // Print table with headers and nice format with tabs
            Console.Write("\t");
            foreach (Token t in terminales)
            {
                Console.Write(t + "\t");
            }
            Console.WriteLine("$");
            for (int i = 0; i < noTerminales.Count; i++)
            {
                Console.Write(noTerminales[i] + "\t");
                for (int j = 0; j < terminales.Count + 1; j++)
                {
                    Console.Write(resTable[i][j] + "\t");
                }
                Console.WriteLine();
            }



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
                //Console.WriteLine("\tList: " + string.Join(" ", list));
                List<Token> res = new List<Token> { };   
                foreach (Token t in list)
                {
                    //Console.WriteLine("\tToken: " + t);
                    foreach (SigPrim tup in primeros)
                    {
                        if (Token.CompareToken(tup.Token, t))
                        {
                            bool band = Token.TokenListContains(tup.TokenList, new Token("#", true));
                            foreach (Token tok in tup.TokenList)
                            {
                                if (!Token.TokenListContains(res, tok) && tok.TokenString != "#")
                                {
                                    //Console.WriteLine("\t\t\tAdding: " + tok);
                                    res.Add(tok);
                                }
                            }
                            if (!band)
                            {
                                //Console.WriteLine("\t\t\tReturning: " + string.Join(" ", res));
                                return res;
                            }
                        }
                    }
                }
                // Add #
                //Console.WriteLine("\tAdding: #");
                res.Add(new Token("#", true));
                //Console.WriteLine("\tReturning: " + string.Join(" ", res));
                return res;
            }
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
