using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace compiladoresPr
{
    public class State
    {
        private bool final;
        private string name;
        private List<Transition> outTransitions;

        public bool Final
        {
            get { return final; }
            set { final = value; }
        }
        public string Name
        {
            get{ return name; }        
            set { name = value; }
        }

        public List<Transition> OutTransitions
        {
            get { return outTransitions; }
        }

        public State(State state)
        {
            name = state.Name;
            final = state.Final;
            outTransitions = state.OutTransitions.ToList();
        }

        public State(bool final)
        {
            this.final = final;
            outTransitions = new List<Transition>();
        }

        public State(string name, bool final)
        {
            this.name = name;
            this.final = final;
            outTransitions = new List<Transition>();
        }

        #region Methods
        public List<Transition> OutTransitionsWith(char value)
        {
            var transitions = new List<Transition>();
            foreach (var transition in outTransitions)
            {
                if (transition.Value == value)
                {
                    transitions.Add(transition);
                }
            }
            return transitions;
        }

        public List<State> OutStatesWith(char value)
        {
            var states = new List<State>();
            foreach (var transition in outTransitions)
            {
                if (transition.Value == value)
                {
                    states.Add(transition.Destination);
                }
            }
            return states;
        }
        
        #endregion
    }

    public class Transition
    {
        private State source;
        private State destination;
        private char transitionValue;

        public State Source 
        {
            get { return source; }
            set { source = value; }
        }
        public State Destination 
        {
            get { return destination; }
            set { destination = value; }
        }
        public char Value 
        {
            get { return transitionValue; }
            set { transitionValue = value; }
        }

        public Transition(State source, State destination, char value)
        {
            Source = source;
            Source.OutTransitions.Add(this);
            Destination = destination;
            Value = value;
        }

        public Transition(State destination,char value)
        {
            Source = null;
            Destination = destination;
            Value = value ;
        }

        public Transition(Transition refe)
        {
            Source = refe.Source;
            Source.OutTransitions.Add(this);
            Destination = refe.Destination;
            Value = refe.Value;
        }

    }

    public class Automata
    {

        private int statescount;
        private List<Transition> transitionsList;
        private List<State> statesList;
        private List<char> alphabet;
        private Transition initReference;
        private State endReference;
        private Stack<Automata> thompsonStack;

        public int StateCount
        {
            get { return statescount; }
        }
        public List<Transition> TransitionsList
        {
            get { return transitionsList; }
        }
        public List<State> States
        {
            get { return statesList; }
        }

        public List<char> Alphabet
        {
            get { return alphabet; }
        }
        public Transition InitReference
        {
            get{return initReference; }
        }
        public State EndReference 
        { 
            get{return endReference; } 
        }
        public Stack<Automata> ThompsonStack
        {
            get { return thompsonStack; }
        }

        public Automata()
        {
            statescount = 0;
            transitionsList = new List<Transition>();
            statesList = new List<State>();
            alphabet = new List<char>() { '#' };
            initReference = null;
            endReference = null;
            thompsonStack = new Stack<Automata>();
        }
        
        public Automata(string regexpos)
        {
            statescount = 0;
            transitionsList = new List<Transition>();
            statesList = new List<State>();
            alphabet = new List<char>() { '#' };
            initReference = null;
            endReference = null;
            thompsonStack = new Stack<Automata>();
            foreach (var character in regexpos)
            {
                AddAsThompson(character);
            }
            if (thompsonStack.Count > 1) throw new Exception("La expresion regular no esta bien formada");
            Automata tmp = thompsonStack.Pop();
            this.statescount = tmp.statescount;
            this.transitionsList = tmp.transitionsList;
            this.statesList = tmp.statesList;
            this.alphabet = tmp.alphabet;
            this.initReference = tmp.initReference;
            this.endReference = tmp.endReference;
            this.thompsonStack = tmp.thompsonStack;

            // rename states
            int i = 0;
            foreach (State state in statesList)
            {
                state.Name = "q" + i;
                i++;
            }

        }

        public Automata(Automata refe)
        {
            statescount = refe.StateCount;
            transitionsList = refe.TransitionsList.ToList();
            statesList = refe.States.ToList();
            alphabet = refe.Alphabet.ToList();
            initReference = refe.InitReference;
            endReference = refe.EndReference;
            thompsonStack = new Stack<Automata>(refe.ThompsonStack);
        }
      
        public void AddAsThompson(char value)
        {
            switch (value)
            {
                case '*':
                    AddThompsonKleeneLock();
                    break;
                case '+':
                    AddThompsonPositiveLock();
                    break;
                case '?':
                    AddThompsonZeroOrOneInt();
                    break;
                case '&':
                    AddThompsonConcatenation();
                    break;
                case '|':
                    AddThompsonAlternativeSelection();
                    break;
                default:
                    AddThompsonBase(value);
                    break;
            }
        }

        private void AddThompsonBase(char value)
        {
            alphabet.Add(value);
            Automata tmp = new Automata();
            State start = new State(false);
            State end = new State(true);
            Transition transition = new Transition(start, end, value);
            Transition init = new Transition(start, '#');
            tmp.AddState(start);
            tmp.AddState(end);
            tmp.AddTransition(transition);
            tmp.AddTransition(init);

            if (!tmp.alphabet.Contains(value)) tmp.alphabet.Add(value);
           
            thompsonStack.Push(tmp);
        }

        private void AddThompsonConcatenation()
        {
            if (thompsonStack.Count < 2) throw new Exception("No hay suficientes elementos en la pila para concatenar (expresion mal formada)");
            Automata second = thompsonStack.Pop();
            Automata first = thompsonStack.Pop();
            first.replaceEndTransitions(second.InitReference.Destination);
            second.deleteInitTransition();
            first.AddTransitions(second.TransitionsList);
            first.AddStates(second.States);
            foreach (char character in second.Alphabet)
            {
                if (!first.Alphabet.Contains(character)) first.Alphabet.Add(character);
            }
            thompsonStack.Push(first);
        }

        private void AddThompsonAlternativeSelection()
        {
            if (thompsonStack.Count < 2) throw new Exception("No hay suficientes elementos en la pila para seleccionar (expresion mal formada)");
            Automata second = thompsonStack.Pop();
            Automata first = thompsonStack.Pop();
            State start = new State(false);
            State end = new State(true);
            first.EndReference.Final = false;
            second.EndReference.Final = false;
            first.initReference.Source = start;
            second.initReference.Source = start;
            Transition init = new Transition(start, '#');
            Transition final1 = new Transition(first.EndReference, end, '#');
            Transition final2 = new Transition(second.EndReference, end, '#');
            first.AddTransition(init);
            first.AddTransition(final1);
            first.AddTransition(final2);
            first.AddState(start);
            first.AddState(end);
            first.AddTransitions(second.TransitionsList);
            first.AddStates(second.States);
            foreach (char character in second.Alphabet)
            {
                if (!first.Alphabet.Contains(character)) first.Alphabet.Add(character);
            }
            thompsonStack.Push(first);
        }

        private void AddThompsonKleeneLock()
        {
            if (thompsonStack.Count < 1) throw new Exception("No hay suficientes elementos en la pila para aplicar el cierre de Kleene (expresion mal formada)");
            Automata tmp = thompsonStack.Pop();
            State start = new State(false);
            State end = new State(true);
            Transition init = new Transition(start, '#');
            Transition mid1 = new Transition(start, end, '#');
            Transition mid2 = new Transition(tmp.EndReference, tmp.InitReference.Destination, '#');
            Transition final = new Transition(tmp.EndReference, end, '#');
            tmp.EndReference.Final = false;
            tmp.initReference.Source = start;
            tmp.AddState(start);
            tmp.AddState(end);
            tmp.AddTransition(init);
            tmp.AddTransition(mid1);
            tmp.AddTransition(mid2);
            tmp.AddTransition(final);
            thompsonStack.Push(tmp);
        }

        private void AddThompsonPositiveLock()
        {
            if (thompsonStack.Count < 1) throw new Exception("No hay suficientes elementos en la pila para aplicar el cierre positivo (expresion mal formada)");
            Automata tmp = thompsonStack.Pop();
            State start = new State(false);
            State end = new State(true);
            Transition init = new Transition(start, '#');
            Transition mid = new Transition(tmp.EndReference, tmp.InitReference.Destination, '#');
            Transition final = new Transition(tmp.EndReference, end, '#');
            tmp.EndReference.Final = false;
            tmp.initReference.Source = start;
            tmp.AddState(start);
            tmp.AddState(end);
            tmp.AddTransition(init);
            tmp.AddTransition(mid);
            tmp.AddTransition(final);
            thompsonStack.Push(tmp);
        }

        private void AddThompsonZeroOrOneInt()
        {
            if (thompsonStack.Count < 1) throw new Exception("No hay suficientes elementos en la pila para aplicar el cierre de 0 o 1 (expresion mal formada)");
            Automata tmp = thompsonStack.Pop();
            State start = new State(false);
            State end = new State(true);
            Transition init = new Transition(start, '#');
            Transition mid = new Transition(start, end, '#');
            Transition final = new Transition(tmp.EndReference, end, '#');
            tmp.EndReference.Final = false;
            tmp.initReference.Source = start;
            tmp.AddState(start);
            tmp.AddState(end);
            tmp.AddTransition(init);
            tmp.AddTransition(mid);
            tmp.AddTransition(final);
            thompsonStack.Push(tmp);
        }

        public void replaceEndTransitions(State state)
        {
            foreach (Transition transition in transitionsList)
            {
                if (transition.Destination == endReference)
                {
                    transition.Destination = state;
                }
            }
            statesList.Remove(endReference);
            statescount--;
            endReference = state;
        }

        public void AddState(State state)
        {
            if (state.Final)
            {
                endReference = state;
            }
            statesList.Add(state);
            statescount++;
        }

        public void AddTransition(Transition transition)
        {
            if (transition.Source == null)
            {
                initReference = transition;
            }
            transitionsList.Add(transition);
        }

        public void AddTransitions(List<Transition> transitions)
        {
            foreach (Transition transition in transitions)
            {
                AddTransition(transition);
            }
        }

        public void AddStates(List<State> states)
        {
            foreach (State state in states)
            {
                AddState(state);
            }
        }

        public void deleteInitTransition()
        {
            initReference.Destination.OutTransitions.Remove(initReference);
            transitionsList.Remove(initReference);
        }

        public void printAutomata()
        {
            Console.WriteLine("---------------------------------------------------");
            Console.WriteLine("Automata: ");
            Console.WriteLine("Estados: " + statescount);
            Console.WriteLine("Transiciones: " + getTransitionsCount());
            Console.WriteLine("Transiciones epsilon: " + getEpsilonTransitionsCount());
            Console.WriteLine("---------------------------------------------------");
        }

        public int getTransitionsCount()
        {
            return transitionsList.Count - 1;
        }

        public int getEpsilonTransitionsCount()
        {
            int epsilonTransitions = 0;
            foreach (Transition transition in transitionsList)
            {
                if (transition.Value == '#' && transition.Source != null)
                {
                    epsilonTransitions++;
                }
            }
            return epsilonTransitions;
        }

        public List<List<List<string>>> GetTransitionsTable()
        {
            List<List<List<string>>> transitions = new List<List<List<string>>>();
            for (int i = 0; i < statesList.Count; i++)
            {
                List<List<string>> tmp2 = new List<List<string>>();
                for (int j = 0; j < alphabet.Count; j++)
                {
                    List<string> tmp = new List<string>();
                    foreach (Transition transition in transitionsList)
                    {
                        if (transition.Source == statesList[i] && transition.Value == alphabet[j])
                        {
                            tmp.Add(transition.Destination.Name);
                        }
                    }
                    tmp2.Add(tmp);
                }
                transitions.Add(tmp2);
            }
            return transitions;
        }

        public void SetTransitionsTable(DataGridView dv)
        {
            List<List<List<string>>> transitions = GetTransitionsTable();
            dv.ColumnCount = alphabet.Count + 1;
            dv.Columns[0].Name = "Estados";
            for (int i = 0; i < alphabet.Count; i++)
            {
                if (alphabet[i] == '#') dv.Columns[i + 1].Name = "ε";
                else dv.Columns[i + 1].Name = alphabet[i].ToString();
            }
            for (int i = 0; i < statesList.Count; i++)
            {
                dv.Rows.Add();
                dv.Rows[i].Cells[0].Value = statesList[i].Name;
                for (int j = 0; j < alphabet.Count; j++)
                {
                    string tmp = "";
                    if (transitions[i][j].Count == 0)
                    {
                        dv.Rows[i].Cells[j + 1].Value = "Ø";
                        continue;
                    }
                    foreach (string name in transitions[i][j])
                    {
                        tmp += name;
                        tmp += ", ";
                    }
                    tmp = tmp.Remove(tmp.Length - 2);
                    tmp = "{" + tmp + "}";
                    dv.Rows[i].Cells[j + 1].Value = tmp;
                }
            }
        }


    }
}