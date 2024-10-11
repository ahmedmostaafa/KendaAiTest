using KabreetGames.EventBus.Interfaces;
using KendaAi.Agents.Planer;
using KendaAi.Events;

namespace KendaAi.TestProject.PlayerController.States
{
    public class HitState : State
    {
        public override void Enter()
        {
            Agent.Animator.AnimationState.SetAnimation(0, GetAnimation(), false).Complete += _ =>
            {
                Agent.ChangePlane<MoveState>();
                EventBus<HitEvent>.Rise();
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