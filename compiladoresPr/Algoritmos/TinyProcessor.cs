using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiladoresPr.Algoritmos
{
    public class TinyProcessor
    {
        private Automata identifierA, numberA;
        private List<String> tokens;
        public Gramatica g;

        // Palabras reservadas de Tiny
        //private List<String> reservedWords = new List<String> { "if", "then", "else", "end", "repeat", "until", "read", "write" };
        // Simbolos de Tiny
        //private List<String> symbols = new List<String> { "+", "-", "*", "/", "=", "<", ">", "(", ")", ";", ":=" };
        private List<String> reservedWords;

        public TinyProcessor(String identifier, String number)
        {
            this.initializeLL();
            ConvPosFija c = new ConvPosFija();
            identifierA = new Automata(c.fixExpression(identifier));
            identifierA.transformToDeterministic();
            numberA = new Automata(c.fixExpression(number));
            numberA.transformToDeterministic();
        }

        private void initializeLL()
        {
            g = new Gramatica();
            Produccion p;

            #region Producciones

            /*
                programa -> secuencia-set 
                secuencia-set -> sentencia secuencia-set' 
                secuencia-set' -> ; sentencia secuencia-set' | # 
                sentencia -> sent-if | sent-repeat | sent-assign | sent-read | sent-write 
                sent-if -> if exp then secuencia-set sent-if' 
                sent-if' -> end | else secuencia-set end 
                sent-repeat -> repeat secuencia-set until exp 
                sent-assign -> identificador := exp 
                sent-read -> read identificador 
                sent-write -> write exp 
                exp -> exp-simple exp' 
                exp' -> op-comp exp-simple | # 
                op-comp -> < | > | = 
                exp-simple -> term exp-simple' 
                exp-simple' -> opsuma term exp-simple' | # 
                opsuma -> + | - 
                term -> factor term' 
                term' -> opmult factor term' | # 
                opmult -> * | / 
                factor -> ( exp ) | numero | identificador
            */

            p = new Produccion("programa");
            p.addProduccion("secuencia-set", false);
            g.addProduccion(p);

            p = new Produccion("secuencia-set");
            p.addProduccion("sentencia", false, "secuencia-set'", false);
            g.addProduccion(p);

            p = new Produccion("secuencia-set'");
            p.addProduccion(";", true, "sentencia", false, "secuencia-set'", false);
            p.addProduccion("#", true);
            g.addProduccion(p);

            p = new Produccion("sentencia");
            p.addProduccion("sent-if", false);
            p.addProduccion("sent-repeat", false);
            p.addProduccion("sent-assign", false);
            p.addProduccion("sent-read", false);
            p.addProduccion("sent-write", false);
            g.addProduccion(p);

            p = new Produccion("sent-if");
            p.addProduccion("if", true, "exp", false, "then", true, "secuencia-set", false, "sent-if'", false);
            g.addProduccion(p);

            p = new Produccion("sent-if'");
            p.addProduccion("end", true);
            p.addProduccion("else", true, "secuencia-set", false, "end", true);
            g.addProduccion(p);

            p = new Produccion("sent-repeat");
            p.addProduccion("repeat", true, "secuencia-set", false, "until", true, "exp", false);
            g.addProduccion(p);

            p = new Produccion("sent-assign");
            p.addProduccion("identificador", true, ":=", true, "exp", false);
            g.addProduccion(p);

            p = new Produccion("sent-read");
            p.addProduccion("read", true, "identificador", true);
            g.addProduccion(p);

            p = new Produccion("sent-write");
            p.addProduccion("write", true, "exp", false);
            g.addProduccion(p);

            p = new Produccion("exp");
            p.addProduccion("exp-simple", false, "exp'", false);
            g.addProduccion(p);

            p = new Produccion("exp'");
            p.addProduccion("op-comp", false, "exp-simple", false);
            p.addProduccion("#", false);
            g.addProduccion(p);

            p = new Produccion("op-comp");
            p.addProduccion("<", true);
            p.addProduccion(">", true);
            p.addProduccion("=", true);
            g.addProduccion(p);

            p = new Produccion("exp-simple");
            p.addProduccion("term", false, "exp-simple'", false);
            g.addProduccion(p);

            p = new Produccion("exp-simple'");
            p.addProduccion("opsuma", false, "term", false, "exp-simple'", false);
            p.addProduccion("#", true);
            g.addProduccion(p);

            p = new Produccion("opsuma");
            p.addProduccion("+", true);
            p.addProduccion("-", true);
            g.addProduccion(p);

            p = new Produccion("term");
            p.addProduccion("factor", false, "term'", false);
            g.addProduccion(p);

            p = new Produccion("term'");
            p.addProduccion("opmult", false, "factor", false, "term'", false);
            p.addProduccion("#", true);
            g.addProduccion(p);

            p = new Produccion("opmult");
            p.addProduccion("*", true);
            p.addProduccion("/", true);
            g.addProduccion(p);

            p = new Produccion("factor");
            p.addProduccion("(", true, "exp", false, ")", true);
            p.addProduccion("numero", true);
            p.addProduccion("identificador", true);
            g.addProduccion(p);

            #endregion

            g.calcPrimeros();
            g.calcSiguientes();
            g.calcTabla();
            //g.analisisSintactico("read x ; repeat x := x + 1 ; write x until x < 10");
            //Console.WriteLine(g);
            this.reservedWords = g.terminalesString();
        }

        public void analisisSintactico(String code)
        {
            String tmpCode = sanitizeCode(code);
            if (tmpCode == "") throw new Exception("No se ha ingresado código");
            g.analisisSintactico(tmpCode, this);
        }

        public void changeNumberRegex(String number)
        {
            ConvPosFija c = new ConvPosFija();
            numberA = new Automata(c.fixExpression(number));
            numberA.transformToDeterministic();
        }

        public void changeIdentifierRegex(String identifier)
        {
            ConvPosFija c = new ConvPosFija();
            this.identifierA = new Automata(c.fixExpression(identifier));
            this.identifierA.transformToDeterministic();

        }

        public String sanitizeCode(String code)
        {
            // Eliminar espacios en blanco y saltos de linea
            code = code.Replace("\n", " ");
            code = code.Replace("\r", " ");
            code = code.Replace("\t", " ");
            while (code.Contains("  "))
            {
                code = code.Replace("  ", " ");
            }
            if (code.StartsWith(" "))
            {
                code = code.Substring(1);
            }
            code = code.Trim();
            return code;
        }

        public List<Tuple<String, String>> clasifyTokens(String code)
        {
            String tmpCode = sanitizeCode(code);
            tokens = tmpCode.Split(new char[] { ' ' }).ToList();
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
            List<Tuple<String, String>> result = new List<Tuple<String, String>>();
            foreach (String token in tokens)
            {
                if (reservedWords.Contains(token)) {
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

        public String classifyToken(String token)
        {
            if (reservedWords.Contains(token))
            {
                return token;
            }
            else if (identifierA.isAccepted(token))
            {
                return "identificador";
            }
            else if (numberA.isAccepted(token))
            {
                return "numero";
            }
            return "error";
        }


    }
}
