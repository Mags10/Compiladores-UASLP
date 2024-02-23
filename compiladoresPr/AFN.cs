using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiladoresPr
{
    public class State
    {
        private bool final;
        public bool Final
        {
            get { return final; }

            set { final = value; }
        }
        private string name;
        public string Name
        {
            get{return name; }            
        }
        private List<Transition> outTransitions;
        public List<Transition> OutTransitions
        {
            get { return outTransitions; }
        }
        #region Methods
        public List<State> OutTransitionsWith(char value)
        {
            return outTransitions.FindAll(t => t.Value == value).ConvertAll(t => t.Destination);
        }
        public State(string name, bool final)
        {
            this.name = name;
            this.final = final;
            outTransitions = new List<Transition>();
        }
        public List<Transition> OutStatesWith()
        {
            //No esta implementado
            return outTransitions;
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
        
        public Automata(char value)
        {
            statescount = 2;
            transitionsList = new List<Transition>();
            statesList = new List<State>();
            alphabet = new List<char>();
            thompsonStack = new Stack<Automata>();
            State start = new State("q0", false);
            State end = new State("q1", true);
            statesList.Add(start);
            statesList.Add(end);
            AddTransition(start, end, value);
            alphabet.Add(value);
            initReference = transitionsList[0];
            endReference = end;
        }
        public Automata(Transition refe)
        {
            statescount = 2;
            transitionsList = new List<Transition>();
            statesList = new List<State>();
            alphabet = new List<char>();
            thompsonStack = new Stack<Automata>();
            State start = new State("q0", false);
            State end = new State("q1", true);
            statesList.Add(start);
            statesList.Add(end);
            AddTransition(start, end, refe.Value);
            alphabet.Add(refe.Value);
            initReference = transitionsList[0];
            endReference = end;
        }
        public Automata(char value, bool final)
        {
            statescount = 2;
            transitionsList = new List<Transition>();
            statesList = new List<State>();
            alphabet = new List<char>();
            thompsonStack = new Stack<Automata>();
            State start = new State("q0", false);
            State end = new State("q1", final);
            statesList.Add(start);
            statesList.Add(end);
            AddTransition(start, end, value);
            alphabet.Add(value);
            initReference = transitionsList[0];
            endReference = end;
        }
        public void AddAsThompson(char a)
        {
           //Todavia no esta implementado
        }
        private char AddThompsonBase()
        {
            //Todavia no esta implementado
            return ' ';
        }
        private void AddThompsonConcatenation()
        {
            //Todavia no esta implementado
        }
        private void AddThompsonAlternativeSelection()
        {
            //Todavia no esta implementado
        }
        private void AddThompsonKleeneLock()
        {
            //Todavia no esta implementado
        }
        private void AddThompsonPositiveLock()
        {
            //Todavia no esta implementado
        }
        private void AddThompsonZeroOrOneIntance()
        {
            //Todavia no esta implementado
        }
        /*
        public void GetTransitions(List<List<List<char>>> a)
        {
            
        }*/
        public void AddTransition(State source, State destination, char value)
        {
            var transition = new Transition(source, destination, value);
            TransitionsList.Add(transition);
            source.OutTransitions.Add(transition);
        }
    }


}


