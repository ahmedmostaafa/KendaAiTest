using KabreetGames.EventBus.Interfaces;
using KendaAi.Agents.Planer;
using KendaAi.Events;

namespace KendaAi.TestProject.PlayerController.States
{
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
}