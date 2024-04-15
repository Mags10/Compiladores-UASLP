using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiladoresPr.Algoritmos
{
    public class TinyProcessor
    {
        private string code;
        private Automata identifierA, numberA;
        private String identifier, number;
        private List<String> tokens;

        // Palabras reservadas de Tiny
        private List<String> reservedWords = new List<String> { "if", "then", "else", "end", "repeat", "until", "read", "write" };
        // Simbolos de Tiny
        private List<String> symbols = new List<String> { "+", "-", "*", "/", "=", "<", ">", "(", ")", ";", ":=" };


        public TinyProcessor(String code, String identifier, String number)
        {
            this.code = code;
            sanitizeCode();
            this.identifier = identifier;
            this.number = number;
            ConvPosFija c = new ConvPosFija();
            identifierA = new Automata(c.fixExpression(identifier));
            identifierA.transformToDeterministic();
            numberA = new Automata(c.fixExpression(number));
            numberA.transformToDeterministic();
            tokens = this.code.Split(new char[] { ' ' }).ToList();
            for (int i = 0; i < tokens.Count;)
            {
                if (tokens[i] == "")
                {
                    tokens.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }

        private void sanitizeCode()
        {
            // Eliminar espacios en blanco y saltos de linea
            code = code.Replace("\n", " ");
            code = code.Replace("\r", " ");
            code = code.Replace("\t", " ");
            code = code.Replace("  ", " ");
            code = code.Trim();
        }

        public List<Tuple<String, String>> clasifyTokens()
        {
            List<Tuple<String, String>> result = new List<Tuple<String, String>>();
            foreach (String token in tokens)
            {
                if (reservedWords.Contains(token) || symbols.Contains(token)) {
                    result.Add(new Tuple<String, String>(token, token));
                }
                else if (identifierA.isAccepted(token)) {
                    result.Add(new Tuple<String, String>("Identificador", token));
                }
                else if (numberA.isAccepted(token)) {
                    result.Add(new Tuple<String, String>("Número", token));
                }
                else {
                    result.Add(new Tuple<String, String>("Error léxico", token));
                }
            }
            return result;
        }

        public void printTokens()
        {
            Console.WriteLine("Identifier regex: " + identifier);
            Console.WriteLine("Number regex: " + number);
            Console.WriteLine("Code: " + code);
            Console.WriteLine("Tokens:");
            List<Tuple<String, String>> tokens = clasifyTokens();
            foreach (Tuple<String, String> token in tokens)
            {
                // Console.WriteLine(token.Item1 + " : " + token.Item2); formated:
                Console.WriteLine(String.Format("{0,-19} : {1}", token.Item1, token.Item2));
            }
        }


    }
}
