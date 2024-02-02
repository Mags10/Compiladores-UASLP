using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace compiladoresPr
{
    public partial class Form1 : Form
    {
        //Declaración de variables
        private Stack<char> pila; // Pila para almacenar los operadores
        private Queue<char> posfija; // Cola para almacenar la salida posfija
        private Dictionary<char, int> precedencia; // Diccionario para almacenar la precedencia de los operadores

        public Form1()
        { 
            InitializeComponent();
            // Inicialización de variables
            pila = new Stack<char>();
            posfija = new Queue<char>();
            precedencia = new Dictionary<char, int>();
            // Asignación de la precedencia de los operadores
            //Defincion de prioridad de operadores
            precedencia.Add('(', 0);
            //precedencia.Add(')', 3);
            precedencia.Add('+', 1);
            precedencia.Add('-', 1);
            precedencia.Add('*', 2);
            precedencia.Add('/', 2);
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            // Obtener la expresión regular del textbox
            string expresion = textExpresion.Text;
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
            textPosfija.Text = string.Join("", posfija.ToArray());
        }


        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
