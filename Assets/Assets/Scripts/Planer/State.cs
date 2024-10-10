using System;
using KendaAi.Agents.InputSystem;
using KendaAi.TestProject.PlayerController;
using KendaAi.TestProject.PlayerController.States;

namespace KendaAi.Agents.Planer
{
    public abstract class State : IState
    {
        private StateMachine machine;
        protected IAgent Agent => machine.Agent;
        protected InputReader Input => machine.InputReader;

        protected bool lookingLeft;

        protected string GetAnimation()
        {
            return this switch
            {
                HitState =>
                    lookingLeft ? nameof(AnimationClips.hitLeft) : nameof(AnimationClips.hitRight),
                MoveState => lookingLeft
                    ? nameof(AnimationClips.runningLeft)
                    : nameof(AnimationClips.runningRight),
                SolvePuzzleState => nameof(AnimationClips.idle),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public void Init(StateMachine m)
        {
            machine = m;
            stateName = GetType().Name;
        }

        public string stateName { get; private set; }
        public bool isExited { get; set; }

        public void InternalEnter()
        {
            Enter();
        }

        public abstract void Enter();
        public abstract void Update();
        public abstract void FixedUpdate();
        public abstract void Exit();

        public void UpdateAnimator()
        {
        }
    }
}