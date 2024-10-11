using KabreetGames.TimeSystem;
using KendaAi.Agents.Planer;

namespace KendaAi.TestProject.PlayerController.States
{
    public class IdleState : State
    {
        private readonly CountdownTimer idleTimer;
        public IdleState()
        {
            idleTimer = new CountdownTimer(1f);
            idleTimer.OnTimerStop += SetMoveState;
        }

        private void SetMoveState() => Agent.ChangePlane<MoveState>();

        public override void Enter()
        {
            idleTimer.Start();
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
}