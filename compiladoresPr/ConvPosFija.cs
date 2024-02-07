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
                // Asignación de la precedencia de los operadores
                //Defincion de prioridad de operadores
                { '(', 0 },
                //precedencia.Add(')', 3);
                { '+', 1 },
                { '-', 1 },
                { '*', 2 },
                { '/', 2 }
            };


            // test concat normalizer

            string test = "aasd*dasd|bb+asf";
            string test2 = "ab|c";
            string test3 = "ab|cd";

            Console.WriteLine("Test 1: " + test);
            Console.WriteLine(ConcatNormalizer(test));
            Console.WriteLine("Test 2: " + test2);
            Console.WriteLine(ConcatNormalizer(test2));
            Console.WriteLine("Test 3: " + test3);
            Console.WriteLine(ConcatNormalizer(test3));
        }

        // Constructor con parámetros
        public ConvPosFija(Dictionary<char, int> precedencia)
        {
            if (precedencia == null)
            {
                throw new ArgumentNullException("La precedencia es nula");
            }
            if (precedencia.Count == 0)
            {
                throw new ArgumentException("La precedencia está vacía");
            }
            // Inicialización de variables
            pila = new Stack<char>();
            posfija = new Queue<char>();
            this.precedencia = precedencia;
        }

        #endregion

        #region Métodos

        public string ConvertirPosFija(string expresion)
        {
            // Excepciones
            if (expresion == null)
            {
                throw new ArgumentNullException("La expresión es nula");
            }
            if (expresion.Length == 0)
            {
                throw new ArgumentException("La expresión está vacía");
            }

            // Limpiar la pila y la cola
            pila.Clear();
            posfija.Clear();
            // Recorrer cada caracter de la expresión
            foreach (char c in expresion)
            {
                // Si es un operando, lo agregamos a la cola
                //funcion para checar que c no se encuentre en la lista de precedencia

                if (!precedencia.ContainsKey(c))
                {
                    posfija.Enqueue(c);
                }
                /*// Si es un paréntesis izquierdo, lo agregamos a la pila
                else if (c == '(' || c ==')')
                {
                    pila.Push(c);
                }
                // Si es un paréntesis derecho, desapilamos hasta encontrar el paréntesis izquierdo
                else if (c == ')')
                {
                    while (pila.Peek() != '(')
                    {
                        posfija.Enqueue(pila.Pop());
                    }
                    pila.Pop(); // Eliminamos el paréntesis izquierdo
                }*/
                // Si es un operador, lo comparamos con el tope de la pila
                else if (precedencia.ContainsKey(c))
                {
                    // Si la pila está vacía o el operador tiene mayor precedencia que el tope, lo apilamos
                    if (pila.Count == 0 || precedencia[c] > precedencia[pila.Peek()])
                    {
                        pila.Push(c);
                    }
                    // Si el operador tiene menor o igual precedencia que el tope, desapilamos hasta que se cumpla lo contrario y luego apilamos el operador
                    else
                    {
                        while (pila.Count > 0 && precedencia[c] <= precedencia[pila.Peek()])
                        {
                            posfija.Enqueue(pila.Pop());
                        }
                        pila.Push(c);
                    }
                }
            }
            // Al terminar de recorrer la expresión, desapilamos todo lo que queda en la pila y lo agregamos a la cola
            while (pila.Count > 0)
            {
                posfija.Enqueue(pila.Pop());
            }
            // Mostramos la salida posfija en el textbox correspondiente
            return string.Join("", posfija.ToArray());
        }

        public string ConvertirPosFija2(string expression)
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

        private string RangeNormalizer(string expresion)
        {
            if (expresion.Count(x => x == '[') != expresion.Count(x => x == ']'))
            {
                throw new ArgumentException("Sintaxis incorrecta, formato de rango inválido");
            }
            while (expresion.Contains("[") || expresion.Contains("]"))
            {
                string tmp = expresion.Substring(expresion.IndexOf('['), expresion.IndexOf(']') - expresion.IndexOf('[') + 1 );
                if (tmp.Count() != 5)
                {
                    throw new ArgumentException("Sintaxis incorrecta, rango de caracteres inválido");
                }
                if (tmp[1] >= tmp[3])
                {
                    throw new ArgumentException("Sintaxis incorrecta, rango de caracteres inválido");
                }
                string newExp = "(";
                for (char c = tmp[1]; c <= tmp[3]; c++)
                {
                    if (newExp.Count() > 1) newExp += "|";
                    newExp += c;
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
            string tmp = "";

            if (unitOper.Contains(expresion[0]) || binaOper.Contains(expresion[0]))
            {
                throw new ArgumentException("Sintaxis incorrecta, operador binario o unario inválido");
            }

            bool band = false;

            for (int i = 0; i < expresion.Count(); i++)
            {
                string word = "";
                if (expresion[i] == '(')
                {
                    word += "(";
                    while (expresion[i] != ')')
                    {
                        word += expresion[i];
                        i++;
                    }
                    word += ")";
                }
                else
                {
                    word += expresion[i];
                }
                if (i + 1 == expresion.Count())
                {
                    i--;
                    band = true;
                }
                i++;
                if (unitOper.Contains(expresion[i]))
                {
                    word += expresion[i];
                }
                else if (binaOper.Contains(expresion[i]))
                {
                    word += expresion[i];
                    i++;
                    if (expresion[i] == '(')
                    {
                        word += "(";
                        while (expresion[i] != ')')
                        {
                            word += expresion[i];
                            i++;
                        }
                        word += ")";
                    }
                    else
                    {
                        word += expresion[i];
                    }
                }
                else
                {
                    i--;
                }
                if (output.Count() > 0) output += "&";
                output += word;
                if (band) break;
            }
            return output;
        }

        #endregion
    }
}