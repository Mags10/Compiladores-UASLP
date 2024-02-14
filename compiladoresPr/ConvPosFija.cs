using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiladoresPr
{
    public class ConvPosFija
    {
        #region Atributos

        //Declaración de variables
        private Stack<char> pila; // Pila para almacenar los operadores
        private Queue<char> posfija; // Cola para almacenar la salida posfija
        private Dictionary<char, int> precedencia; // Diccionario para almacenar la precedencia de los operadores

        #endregion

        #region Constructores

        // Constructor por defecto
        public ConvPosFija()
        {
            // Inicialización de variables
            pila = new Stack<char>();
            posfija = new Queue<char>();
            precedencia = new Dictionary<char, int>
            {
                { '*', 1 },
                { '?', 1 },
                { '+', 1 },
                { '&', 0 },
                { '|', 0 }
            };
        }

        #endregion

        #region Métodos

        public string ConvertirPosFija(string expression)
        {
            pila.Clear();
            string output = "";
            bool band;
            foreach (char c in expression)
            {
                switch (c)
                {
                    case '(':
                        pila.Push(c);
                        break;
                    case ')':
                        while (pila.Peek() != '(')
                        {
                            output += pila.Pop();
                            if (pila.Count == 0) throw new ArgumentException("Sintaxis incorrecta, paréntesis desbalanceados");
                        }
                        pila.Pop();
                        break;
                    default:
                        if (!precedencia.ContainsKey(c)) output += c;
                        else
                        {
                            band = true;
                            while (band)
                            {
                                if (pila.Count == 0 || pila.Peek() == '(' || precedencia[c] > precedencia[pila.Peek()])
                                {
                                    pila.Push(c);
                                    band = false;
                                }
                                else output += pila.Pop();
                            }
                        }
                        break;
                }
            }
            while (pila.Count > 0)
            {
                char tmp = pila.Pop();
                if (tmp == '(') throw new ArgumentException("Sintaxis incorrecta, paréntesis desbalanceados");
                output += tmp;
            }
            return output;
        }

        public string NormaliceExpresion(string expresion)
        {
            expresion = expresion.Replace(" ", "");
            expresion = RangeNormalizer(expresion);
            expresion = ConcatNormalizer(expresion);
            return expresion;
        }

        private string RangeNormalizer(string expresion)
        {
            if (expresion.Count(x => x == '[') != expresion.Count(x => x == ']'))
            {
                throw new ArgumentException("Sintaxis incorrecta, formato de rango inválido");
            }
            while (expresion.Contains("[") || expresion.Contains("]"))
            {
                string tmp = expresion.Substring(expresion.IndexOf('['), expresion.IndexOf(']') - expresion.IndexOf('[') + 1 );
                string newExp = "(";
                if (tmp.Contains("-"))
                {
                    if (tmp.Count() != 5)
                    {
                        throw new ArgumentException("Sintaxis incorrecta, rango de caracteres inválido " + tmp);
                    }
                    if (tmp[1] > tmp[3])
                    {
                        throw new ArgumentException("Sintaxis incorrecta, rango de caracteres inválido " + tmp);
                    }
                    for (char c = tmp[1]; c <= tmp[3]; c++)
                    {
                        if (precedencia.ContainsKey(c)) throw new ArgumentException("Sintaxis incorrecta, el rango " + tmp + " contene operaciones reservadas.");
                        if (newExp.Count() > 1) newExp += "|";
                        newExp += c;
                    }
                }
                else
                {
                    List<char> lstmp = new List<char>();
                    for (int i = 1; i < tmp.Count() - 1; i++)
                    {
                        if (precedencia.ContainsKey(tmp[i])) throw new ArgumentException("Sintaxis incorrecta, el rango " + tmp + " contene operaciones reservadas.");
                        if (newExp.Count() > 1) newExp += "|";
                        if (!lstmp.Contains(tmp[i])) newExp += tmp[i];
                        else newExp = newExp.Remove(newExp.Count() - 1);
                        lstmp.Add(tmp[i]);
                    }
                }
                newExp += ")";
                expresion = expresion.Replace(tmp, newExp);
            }
            return expresion;
        }

        public string ConcatNormalizer(string expresion)
        {
            List<char> unitOper = new List<char> { '*', '+', '?' }; 
            List<char> binaOper = new List<char> { '&', '|' };
            string output = "";

            while (expresion.Contains("()"))
            {
                // get the first ocurrence of "()"
                int index = expresion.IndexOf("()");
                // remove the ocurrence
                expresion = expresion.Remove(index, 2);
                if (index == expresion.Count()) break;
                if (unitOper.Contains(expresion[index]))
                {
                    expresion = expresion.Remove(index, 1);
                }
                if (index == expresion.Count()) break;
                if (binaOper.Contains(expresion[index]))
                {
                    throw new ArgumentException("Sintaxis incorrecta, operador binario sin operando");
                }
            }

            if (unitOper.Contains(expresion[0]) || binaOper.Contains(expresion[0]))
            {
                throw new ArgumentException("Sintaxis incorrecta, operador binario o unario inválido");
            }

            if (expresion.Contains("[") || expresion.Contains("]"))
            {
                throw new ArgumentException("Sintaxis incorrecta, existen rangos de caracteres no normalizados");
            }

            if (expresion.Count(x => x == '(') != expresion.Count(x => x == ')'))
            {
                throw new ArgumentException("Sintaxis incorrecta, paréntesis desbalanceados");
            }

            for (int i = 0; i < expresion.Count(); i++)
            {
                // funcion interna para encontrar palabras
                string findWord()
                {
                    string wordtmp = "";
                    if (expresion[i] == '(')
                    {
                        i++;
                        int cp = 1;
                        while (cp != 0)
                        {
                            wordtmp += expresion[i];
                            i++;
                            if (expresion[i] == '(') cp++;
                            if (expresion[i] == ')') cp--;
                        }
                        if (wordtmp.Count() > 1) wordtmp = "(" + ConcatNormalizer(wordtmp) + ")";
                    }
                    else
                    {
                        wordtmp += expresion[i];
                    }
                    if (i + 1 != expresion.Count())
                    {
                        if (unitOper.Contains(expresion[i + 1]))
                        {
                            i++;
                            wordtmp += expresion[i];
                        }
                        checkWordIntegrity(0);
                    }
                    return wordtmp;
                }

                void checkWordIntegrity(int c)
                {
                    switch (c)
                    {
                        case 0:
                            if (i + 1 == expresion.Count()) return;
                            if (unitOper.Contains(expresion[i + 1]))
                            {
                                throw new ArgumentException("Sintaxis incorrecta, operador unario no seguido de otro operador unario");
                            }
                            break;
                        case 1:
                            if (i + 1 != expresion.Count())
                            {
                                if (binaOper.Contains(expresion[i + 1]))
                                {
                                    throw new ArgumentException("Sintaxis incorrecta, operador binario no puede ser seguido de otro operador binario");
                                }
                            }
                            else
                            {
                                throw new ArgumentException("Sintaxis incorrecta, operador binario no seguido de operando");
                            }
                        break;
                    }
                }

                string word = findWord();
                
                if (i + 1 != expresion.Count())
                {
                    while (binaOper.Contains(expresion[i + 1]))
                    {
                        i++;
                        word += expresion[i];
                        checkWordIntegrity(1);
                        i++;
                        word += findWord();
                        if (i + 1 == expresion.Count()) break;
                    }
                }
                if (output.Count() > 0) output += "&";
                output += word;
            }
            
            return output;
        }

        #endregion
    }
    public class EvaPosFija
    {
        #region Atributos

        private Stack<char> infija; // Cola para almacenar la evaluacion infija
        private Stack<char> pila; // Pila para almacenar los operadores
        private Stack<char> tempo; // Pila para almacenar la pila temporalmente
        private Dictionary<char, int> preceUna; // Diccionario para almacenar la precedencia de los operadores unarios
        private Dictionary<char, int> preceBin; // Diccionario para almacenar la precedencia de los operadores binarios


        #endregion

        #region Constructores
        public EvaPosFija(String posfija)
        {
            // Inicialización de variables
            pila = new Stack<char>();
            tempo = new Stack<char>();
            infija = new Stack<char>();
            infija.Clear();
            foreach (char c in posfija.Reverse())
            {
                infija.Push(c);
                Console.WriteLine(c);
            }
            preceUna = new Dictionary<char, int>
            {
                { '*', 0 },
                { '?', 1 },
                { '+', 0 },              
            };
            preceBin = new Dictionary<char, int>
            {
                { '&', 0 },
                { '|', 0 },
            };
        }
        #endregion 

        #region Métodos

        public String EvaluarPosFija()
        {
            int cont= 0;
            pila.Clear();
            tempo.Clear();
            foreach (char c in infija)
            {
                tempo.Clear();
                Console.WriteLine("dentro de eva"+c);
                if(!preceBin.ContainsKey(c) && !preceUna.ContainsKey(c))
                {
                    pila.Push(c);
                    Console.WriteLine(pila.Peek());
                }
                else
                {
                    if(preceBin.ContainsKey(c))
                    {
                        char tmp = pila.Pop();
                        pila.Push(c);
                        Console.WriteLine(pila.Peek());
                        pila.Push(tmp);
                    }
                    else if(preceUna.ContainsKey(c))
                    {

                        //char ope = pila.Pop();
                        pila.Push(c);
                        Console.WriteLine(pila.Peek());
                        char tmp1 = pila.Pop();

                        if (!preceUna.ContainsKey(pila.Peek()) && !preceBin.ContainsKey(pila.Peek()) )
                        {
                            
                            foreach (char d in pila.Reverse())
                            {
                                
                                if (preceUna.ContainsKey(d))
                                {
                                    tempo.Push(d);
                                    tempo.Push('(');                            
                                    
            
                                }                                   
                                else if(!preceUna.ContainsKey(d))
                                    tempo.Push(d);
                            }
                            tempo.Push(')');
                            pila.Clear();
                            foreach (char r in tempo.Reverse())
                            {
                                pila.Push(r);
                            }
                            tempo.Clear();
                            pila.Push(tmp1);
                        }
                        else { 
                            pila.Push(tmp1);
                        }
                        
                    }
                }
                foreach (char t in pila.Reverse())
                {
                    cont++;
                    Console.WriteLine("vuelta "+" "+cont +" "+ t);
                }
            }
            string tmp2 = "";
            foreach (char d in pila.Reverse())
            {
                tmp2 += d;
                Console.WriteLine("dentro de ciclo final "+" "+ d);
            }
            if (pila.Count != 0) {

                return tmp2;
            }   
                
            return "No se pudo evaluar la expresion "+" "+tmp2;
        }
        #endregion
    }
}