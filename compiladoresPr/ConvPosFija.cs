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

        public string Convertir(string expresion)
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

        #endregion
    }
}