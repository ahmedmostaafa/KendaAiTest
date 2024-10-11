using KendaAi.Agents.Planer;

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
}