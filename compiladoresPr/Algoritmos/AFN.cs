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
            //destination.OutTransitions.Add(this);
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
        private List<State> finalStates;
        private bool isDeterministic = false;

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
        public List<State> FinalStates
        {
            get { return finalStates; }
        }

        public bool Deterministic
        {
            get { return isDeterministic; }
            set { isDeterministic = value; }
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
            finalStates = new List<State>();
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
            finalStates = new List<State>();
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
            this.finalStates.Add(this.endReference);
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
            finalStates = refe.finalStates.ToList();
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
            start.OutTransitions.Add(first.initReference);
            start.OutTransitions.Add(second.initReference);
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
            start.OutTransitions.Add(tmp.InitReference);
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
            start.OutTransitions.Add(tmp.InitReference);
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
            start.OutTransitions.Add(tmp.InitReference);
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
                finalStates.Add(state);
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
            Console.WriteLine("Simbolos: " + alphabet.Count);
            foreach (char character in alphabet)
            {
                Console.WriteLine("Simbolo: " + character);
            }
            Console.WriteLine("\nEstados: " + statescount);
            foreach (State state in statesList)
            {
                Console.WriteLine("Estado: " + state.Name + " Final: " + state.Final);
            }
            Console.WriteLine("\nTransiciones: " + getTransitionsCount());
            foreach (Transition transition in transitionsList)
            {
                if (transition.Source != null) Console.WriteLine("Transicion: " + transition.Source.Name + " - (" + transition.Value + ") > " + transition.Destination.Name);
                else Console.WriteLine("Transicion de inicio: " + " - (" + transition.Value + ") > " + transition.Destination.Name);
            }
            Console.WriteLine("\nTransiciones epsilon: " + getEpsilonTransitionsCount());
            foreach (Transition transition in transitionsList)
            {
                if (transition.Value == '#' && transition.Source != null)
                {
                    Console.WriteLine("Transicion epsilon: " + transition.Source.Name + " - (" + transition.Value + ") > " + transition.Destination.Name);
                }
            }
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

        public void SetTransitionsTable(DataGridView dv, bool conj)
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
                    if (conj) tmp = "{" + tmp + "}";  
                    dv.Rows[i].Cells[j + 1].Value = tmp;
                }
            }
        }

        //Mando llamar el metodo EpsilonClosure en el metodo EpsilonTransitions
        private List<State> EpsilonClosure(State state)
        {
            List<State> closure = new List<State> { state };
            List<Transition> tmp = state.OutTransitionsWith('#');
            if (tmp.Count == 0) return closure;
            aux(state);
            return closure;

            void aux (State tmpState)
            {
                List<Transition> tmpTrsLst = tmpState.OutTransitionsWith('#');
                if (tmpTrsLst.Count == 0) return;
                foreach (Transition transition in tmpTrsLst)
                {
                    if (!closure.Contains(transition.Destination))
                    {
                        closure.Add(transition.Destination);
                        aux(transition.Destination);
                    }
                }
            }
        }

        public Automata createAFD()
        {
            Automata automata = new Automata();

            // Alfabeto del AFD (sin epsilon)
            List<char> alphabetFinal = new List<char>(this.alphabet);
            alphabetFinal.Remove('#');

            // Lista de estados del AFD
            List<List<State>> states = new List<List<State>>();
            List<State> realStates = new List<State>();

            List<State> initial = EpsilonClosure(this.initReference.Destination);
            State origin = addStateAFD(initial, states, realStates, automata);

            Transition init = new Transition(origin, '#');
            automata.AddTransition(init);

            for (int i = 0; i < states.Count; i++)
            {
                initial = states[i];
                origin = realStates[i];
                foreach (Char c in alphabetFinal)
                {
                    List<State> tmp = new List<State>();
                    foreach (State state in initial)
                    {
                        tmp.AddRange(state.OutStatesWith(c));
                    }
                    List<State> tmpEpsilonClosure = new List<State>();
                    foreach (State state in tmp)
                    {
                        List<State> tmpAuxEpsilonClosure = EpsilonClosure(state);
                        foreach (State state2 in tmpAuxEpsilonClosure)
                        {
                            if (!tmpEpsilonClosure.Contains(state2))
                            {
                                tmpEpsilonClosure.Add(state2);
                            }
                        }
                    }
                    if (tmpEpsilonClosure.Count == 0) continue;
                    State destination = addStateAFD(tmpEpsilonClosure, states, realStates, automata);
                    Transition transition = new Transition(origin, destination, c);
                    if (!automata.alphabet.Contains(c)) automata.alphabet.Add(c);
                    automata.AddTransition(transition);
                }
            }
            automata.alphabet.Remove('#');
            automata.Deterministic = true;
            return automata;
        }

        private State addStateAFD(List<State> stateList, List<List<State>> states, List<State> realStates, Automata automata)
        {
            State origin = null;
            for (int i = 0; i < states.Count; i++)
            {
                if (isListEqual(states[i], stateList))
                {
                    origin = realStates[i];
                    return origin;
                }
            }
            origin = new State(false);
            origin.Name = intToChar(automata.StateCount);
            foreach (State s in stateList)
            {
                if (s.Final)
                {
                    origin.Final = true;
                    break;
                }
            }
            realStates.Add(origin);
            states.Add(stateList);
            automata.AddState(origin);
            return origin;
        }

        private bool isListEqual(List<State> list1, List<State> list2)
        {
            return list1.Count == list2.Count && list1.All(list2.Contains);
        }

        // Convert int to ASCII from A to Z, if the number is greater than 25, 25 will be a A, etc
        private string intToChar(int number)
        {
            if (number > 25)
            {
                return ((char)(number % 25 + 64)).ToString();
            }
            return ((char)(number + 65)).ToString();
        }

        public bool isAccepted(string word)
        {
            if (!isDeterministic) throw new Exception("El automata no es determinista");
            State state = this.initReference.Destination;
            foreach (char c in word)
            {
                List<Transition> tmp = state.OutTransitionsWith(c);
                if (tmp.Count > 1) throw new Exception("El automata no es determinista");
                if (tmp.Count == 0) return false;
                state = tmp[0].Destination;
            }
            return state.Final;
        }
    }
}