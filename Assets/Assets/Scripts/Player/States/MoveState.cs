using KabreetGames.EventBus.Interfaces;
using KendaAi.Agents.Planer;
using KendaAi.Events;

namespace KendaAi.TestProject.PlayerController.States
{
    public class MoveState : State
    {
        private float xInput;

        public override void Enter()
        {
            Agent.Animator.AnimationState.SetAnimation(0, GetAnimation(), true);
        }

        public override void Update()
        {
            xInput = Input.HorizontalMove;
            if (xInput != 0 && lookingLeft != xInput > 0) SwitchDirection();
        }

        private void SwitchDirection()
        {
            lookingLeft = xInput > 0;
            Agent.Animator.AnimationState.SetAnimation(0, GetAnimation(), true);
        }

        public override void FixedUpdate()
        {
            Agent.Move(xInput);
        }

        public override void Exit()
        {
        }
    }

    public class SolvePuzzleState : State
    {
        public override void Enter()
        {
            EventBus<PuzzleStartEvent>.Rise();
            Agent.Animator.AnimationState.SetAnimation(0, GetAnimation(), true);
        }

        public override void Update()
        {
        }

        public override void FixedUpdate()
        {
        }

        public override void Exit()
        {
        }
    }

    public class HitState : State
    {
        public override void Enter()
        {
            EventBus<HitEvent>.Rise();
            Agent.Animator.AnimationState.SetAnimation(0, GetAnimation(), false).Complete += _ =>
            {
                Agent.ChangePlane<MoveState>();
            };
        }

        public override void Update()
        {
        }

        public override void FixedUpdate()
        {
            Agent.MoveHit();
        }

        public override void Exit()
        {
        }
    }
}