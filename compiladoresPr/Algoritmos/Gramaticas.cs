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
        List<Tuple<Token, List<Token>>> primeros;
        List<Siguiente> siguientes;
        private Produccion inicial = null;

        public Gramatica()
        {
            terminales = new List<Token>();
            noTerminales = new List<Token>();
            producciones = new List<Produccion>();
        }

        public void addProduccion(Produccion produccion)
        {
            if (inicial == null)
            {
                inicial = produccion;
            }

            producciones.Add(produccion);

            auxAdder(produccion.Productor);

            foreach (List<Token> p in produccion.Producciones)
            {
                foreach (Token t in p)
                {
                    auxAdder(t);
                }
            }
            void auxAdder(Token t) {
                if (t.TokenString == "#") return;
                if (t.Terminal)
                {
                    if (!Token.TokenListContains(terminales, t))
                    {
                        //Console.WriteLine(t.TokenString + " Terminal: SI");
                        terminales.Add(t);
                    }
                }
                else
                {
                    if (!Token.TokenListContains(noTerminales, t))
                    {
                        //Console.WriteLine(t.TokenString + " Terminal: NO");
                        noTerminales.Add(t);
                    }
                }
            }
        }

        public override string ToString()
        {
            string gram = "\n\n";
            gram += "-------------------------------------";
            gram += "\nProducciones: \n";
            foreach (Produccion p in producciones)
            {
                gram += p + "\n";
            }
            
            gram += "\nInicial: " + inicial.Productor;


            gram += "\n\nTerminales: ";

            foreach (Token t in terminales)
            {
                gram += t + " ";
            }

            gram += "\n\nNo Terminales: ";

            foreach (Token t in noTerminales)
            {
                gram += t + " ";
            }

            gram += "\n\nPrimeros: \n";
            foreach (Tuple<Token, List<Token>> t in primeros)
            {
                if (t.Item1 == null || t.Item2 == null) continue;
                gram += "Primero(" + t.Item1 + ") = {";
                foreach (Token tok in t.Item2)
                {
                    if (tok != t.Item2.Last())
                        gram += tok + ", ";
                    else
                        gram += tok;
                }
                gram += "}\n";
            }

            gram += "\nSiguientes: \n";
            foreach (Siguiente s in siguientes)
            {
                gram += "Siguiente(" + s.Token + ") = {";
                foreach (Token t in s.segundos)
                {
                    if (t != s.segundos.Last())
                        gram += t + ", ";
                    else
                        gram += t;
                }
                gram += "}\n";
            }

            return gram;
        }

        public void calcPrimeros()
        {
            primeros = new List<Tuple<Token, List<Token>>>();
            foreach (Token t in terminales)
            {
                List<Token> tmp = new List<Token> { t };
                primeros.Add(new Tuple<Token, List<Token>>(t, tmp));
            }
            foreach (Produccion p in producciones)
            {
                getPrimeros(p);
                //tmp.AddRange(getPrimeros(p));
                //primeros.Add(new Tuple<Token, List<Token>>(t, tmp));
            }

            List<Token> getPrimeros(Produccion p)
            {
                Console.WriteLine("=====================================");
                List<Token> res =  getPrimerosAux(p.Productor);
                if (res == null)
                {
                    Console.WriteLine("Calculando primeros de: " + p.Productor);
                    res = new List<Token>();
                }
                else
                {
                    Console.WriteLine("Ya se calcularon los primeros de: " + p.Productor);
                    return res;
                }
                bool endBand = false;
                foreach (List<Token> prod in p.Producciones)
                {
                    Console.Write("\nProduccion: ");
                    foreach (Token t in prod)
                    {
                        Console.Write(t + " ");
                    }
                    Console.WriteLine();

                    foreach(Token t in prod)
                    {
                        Console.WriteLine("Primeros de: " + t);
                        string strmp = "Token (" + t + ") añade: {";
                        List<Token> preres = getPrimerosAux(t);

                        if (preres == null)
                        {
                            Produccion p2 = getProduction(t);
                            if (p2 != null)
                            {
                                preres = getPrimeros(p2);
                            }
                            else
                            {
                                Exception e = new Exception("No se encontro la produccion");
                            }
                        }

                        if (preres == null)
                        {
                            preres = new List<Token>();
                            preres.Add(new Token("#", true));
                        }

                        //res.AddRange(preres);
                        // Sin repetir
                        foreach (Token tok in preres)
                        {
                            if (!Token.TokenListContains(res, tok))
                            {
                                res.Add(tok);
                            }
                        }

                        endBand = !Token.DeleteToken(res, new Token("#", true));

                        foreach (Token tok in preres)
                        {
                            strmp += tok + " ";
                        }

                        strmp += "}";
                        Console.WriteLine(strmp);

                        if (endBand) break;
                    }
                    if (!endBand)
                    {
                        res.Add(new Token("#", true));
                        Console.WriteLine("Se añade #");
                    }
                    else
                    {
                        Console.WriteLine("No se añade #");
                    }
                }
                primeros.Add(new Tuple<Token, List<Token>>(p.Productor, res));
                Console.WriteLine("=====================================");

                return res;
            }

            List<Token> getPrimerosAux(Token token)
            {
                foreach (Tuple<Token, List<Token>> tup in primeros)
                {
                    if (Token.CompareToken(tup.Item1, token))
                    {
                        return tup.Item2;
                    }
                }
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

            siguientes = new List<Siguiente>();
            List<Siguiente> calculando = new List<Siguiente>();

            foreach (Token t in noTerminales)
            {
                Siguiente s = new Siguiente(t);
                if (Token.CompareToken(t, inicial.Productor))
                {
                    s.segundos.Add(new Token("$", true));
                }
                siguientes.Add(s);
            }

            foreach (Siguiente sg in siguientes)
            {
                if (!sg.calculated)
                {
                    calculando.Add(sg);
                    calcSiguienteAux(sg);
                }
            }


            void calcSiguienteAux(Siguiente sg)
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
                                if (!Token.TokenListContains(sg.segundos, tk))
                                    sg.segundos.Add(tk);
                                //Console.Write(tk + ", ");
                            } else {
                                band = true;
                            }
                        }
                    }
                    if (band || beta.Count == 0)
                    {
                        //Console.Write("Sig(" + sg.Token + ") = {");
                        Siguiente s = getSiguiente(tup.Item1);
                        
                        if (!Siguiente.SiguienteListContains(calculando, s) && !s.calculated)
                        {
                            // Si s en segundoslist de sg entonces hay un ciclo, no calcular
                            if (!Token.TokenListContains(sg.segundos, s.Token))
                            {
                                Console.WriteLine("Calculando siguientes de: " + s.Token);
                                calcSiguienteAux(s);
                            }
                        }

                        foreach (Token t in s.segundos)
                        {
                            if (!Token.TokenListContains(sg.segundos, t))
                            {
                                sg.segundos.Add(t);
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
                foreach (Token t in sg.segundos)
                {
                    Console.Write(t + ", ");
                }
                Console.WriteLine("}");
                Console.WriteLine("----------------------------------------");
            }

            Siguiente getSiguiente(Token t)
            {
                foreach (Siguiente s in siguientes)
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
                    foreach (Tuple<Token, List<Token>> tup in primeros)
                    {
                        if (Token.CompareToken(tup.Item1, t))
                        {
                            //res.AddRange(tup.Item2); // Sin repetir
                            foreach (Token tok in tup.Item2)
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

    internal class Siguiente
    {
        public bool calculated = false;
        public Token Token;
        public List<Token> segundos;
        
        public Siguiente(Token token)
        {
            Token = token;
            segundos = new List<Token>();
        }

        public static bool CompareSiguiente(Siguiente s1, Siguiente s2)
        {
            return Token.CompareToken(s1.Token, s2.Token);
        }

        public static bool SiguienteListContains(List<Siguiente> list, Siguiente s)
        {
            foreach (Siguiente sg in list)
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
