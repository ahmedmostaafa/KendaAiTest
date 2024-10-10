using System;
using System.Collections.Generic;
using System.Linq;
using KendaAi.Agents.InputSystem;
using UnityEngine;

namespace KendaAi.Agents.Planer
{
    public sealed class StateMachine : IPlaner
    {
        public IPlane currentPlane => currentState.State;
        public IAgent Agent { get; set; }
        private StateNode currentState;
        private readonly Dictionary<Type, StateNode> nodes = new();
        private readonly HashSet<Transition> anyTransitions = new();

        public InputReader InputReader { get; private set; }
        public StateMachine(InputReader inputReader, IAgent agent)
        {
            InputReader = inputReader;
            Agent = agent;
        }

        private void ChangeState(IState state)
        {
            if (state == currentState.State) return;
            var oldState = currentState.State;
            var newState = nodes[state.GetType()].State;
            oldState?.Exit();
            if (oldState != null) oldState.isExited = true;
            currentState = nodes[state.GetType()];
            newState?.InternalEnter();
        }
        public void Update()
        {
            var transition = GetTransition();
            if (transition != null)
            {
                ChangeState(transition.To);
            }
            currentState.State?.Update();
            currentState.State?.UpdateAnimator();
        }

        public void FixedUpdate()
        {
            currentState.State?.FixedUpdate();
        }

        public void DrawGizmos()
        {
#if UNITY_EDITOR

            var label = $"Planer: State Machine \n State: {currentState.State.stateName}";
            var style = new GUIStyle()
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                richText = true,
                normal = new GUIStyleState()
                {
                    textColor = Color.white
                }
            };
            UnityEditor.Handles.Label(Agent.Animator.transform.position + Vector3.up * 2.4f, label,style);
#endif
        }

        public void SetState<T>() where T : IState, new()
        {
            SetState(GetOrAddNode<T>().State);
        }
        
        public  void ChangeState<T>() where T : IState, new()
        {
            ChangeState(GetOrAddNode<T>().State);
        }

        private void SetState(IState state)
        {
            currentState = nodes[state.GetType()];
            currentState.State?.InternalEnter();
        }


        private ITransition GetTransition()
        {
            foreach (var transition in anyTransitions.Where(transition => transition.Condition.Evaluate()))
            {
                return transition;
            }

            return currentState.Transitions.FirstOrDefault(transition => transition.Condition.Evaluate());
        }


        public void AddTransition<TFrom, TO>(IPredicate condition) where TFrom : IState, new() where TO : IState, new()
        {
            GetOrAddNode<TFrom>().AddTransition(GetOrAddNode<TO>().State, condition);
        }

        public void AddTransition<TFrom, TO>(Func<bool> condition) where TFrom : IState, new() where TO : IState, new()
        {
            GetOrAddNode<TFrom>().AddTransition(GetOrAddNode<TO>().State, new FuncPredicate(condition));
        }

        public void AddTransition(IState from, IState to, IPredicate condition)
        {
            GetOrAddNode(from).AddTransition(to, condition);
        }

        public void AddAnyTransition(IState to, IPredicate condition)
        {
            anyTransitions.Add(new Transition(GetOrAddNode(to).State, condition));
        }

        public void AddAnyTransition<T>(IPredicate condition) where T : IState, new()
        {
            anyTransitions.Add(new Transition(GetOrAddNode<T>().State, condition));
        }

        public void AddAnyTransition<T>(Func<bool> condition) where T : IState, new()
        {
            anyTransitions.Add(new Transition(GetOrAddNode<T>().State, new FuncPredicate(condition)));
        }

        private StateNode GetOrAddNode<T>() where T : IState, new()
        {
            var node = nodes.GetValueOrDefault(typeof(T));
            if (node != null) return node;
            var state = new T();
            state.Init(this);
            node = new StateNode(state);
            nodes.Add(typeof(T), node);
            return node;
        }

        private StateNode GetOrAddNode(IState from)
        {
            var node = nodes.GetValueOrDefault(from.GetType());
            if (node != null) return node;
            node = new StateNode(from);
            nodes.Add(from.GetType(), node);
            return node;
        }

        private class StateNode
        {
            public IState State { get; }
            public HashSet<ITransition> Transitions { get; }

            public StateNode(IState state)
            {
                this.State = state;
                Transitions = new HashSet<ITransition>();
            }

            public void AddTransition(IState to, IPredicate condition)
            {
                Transitions.Add(new Transition(to, condition));
            }
        }
    }
}