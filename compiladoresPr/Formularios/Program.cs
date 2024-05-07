using compiladoresPr.Algoritmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace compiladoresPr
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            
            /*
        Gramatica g = new Gramatica();

        Produccion p;

        /* Ejemplo 1 
        // A -> Ba | d
        p = new Produccion("A");
        p.addProduccion("B", false, "a", true);
        p.addProduccion("d", true);
        g.addProduccion(p);
       

            /*
                S -> BDS'
                S' -> a | c | ε
                B -> b | ε
                D -> dD' | ε
                D' -> ε | D
            

            p = new Produccion("A");
            p.addProduccion("B", false);
            g.addProduccion(p);

            p = new Produccion("B");
            p.addProduccion("C", false, "B'", false);
            g.addProduccion(p);

            p = new Produccion("B'");
            p.addProduccion(";", true, "C", false, "B'", false);
            p.addProduccion("#", true);
            g.addProduccion(p);

            p = new Produccion("C");
            p.addProduccion("D", false);
            p.addProduccion("E", false);
            p.addProduccion("F", false);
            p.addProduccion("G", false);
            p.addProduccion("H", false);
            g.addProduccion(p);

            p = new Produccion("D");
            p.addProduccion("if", true, "I", false, "then", true, "B", false, "D'", false);
            g.addProduccion(p);

            p = new Produccion("D'");
            p.addProduccion("end", true);
            p.addProduccion("else", true, "B", false, "end", true);
            g.addProduccion(p);

            p = new Produccion("E");
            p.addProduccion("repeat", true, "B", false, "until", true, "I", false);
            g.addProduccion(p);

            p = new Produccion("F");
            p.addProduccion("identificador", true, ":=", true, "I", false);
            g.addProduccion(p);

            p = new Produccion("G");
            p.addProduccion("read", true, "identificador", true);
            g.addProduccion(p);

            p = new Produccion("H");
            p.addProduccion("write", true, "I", false);
            g.addProduccion(p);

            p = new Produccion("I");
            p.addProduccion("K", false, "I'", false);
            g.addProduccion(p);

            p = new Produccion("I'");
            p.addProduccion("J", false, "K", false);
            p.addProduccion("#", false);
            g.addProduccion(p);

            p = new Produccion("J");
            p.addProduccion("<", true);
            p.addProduccion(">", true);
            p.addProduccion("=", true);
            g.addProduccion(p);

            p = new Produccion("K");
            p.addProduccion("M", false, "K'", false);
            g.addProduccion(p);

            p = new Produccion("K'");
            p.addProduccion("L", false, "M", false, "K'", false);
            p.addProduccion("#", true);
            g.addProduccion(p);

            p = new Produccion("L");
            p.addProduccion("+", true);
            p.addProduccion("-", true);
            g.addProduccion(p);

            p = new Produccion("M");
            p.addProduccion("O", false, "M'", false);
            g.addProduccion(p);

            p = new Produccion("M'");
            p.addProduccion("N", false, "O", false, "M'", false);
            p.addProduccion("#", true);
            g.addProduccion(p);

            p = new Produccion("N");
            p.addProduccion("*", true);
            p.addProduccion("/", true);
            g.addProduccion(p);

            p = new Produccion("O");
            p.addProduccion("(", true, "I", false, ")", true);
            p.addProduccion("numero", true);
            p.addProduccion("identificador", true);
            g.addProduccion(p);

            g.calcPrimeros();

            g.calcSiguientes();

            //Console.WriteLine(g);

            g.calcTabla();
            */
        }
    }
}   
