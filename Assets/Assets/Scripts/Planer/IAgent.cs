using KendaAi.BlackboardSystem;
using KendaAi.Inventory.Interface;
using Spine.Unity;

namespace KendaAi.Agents.Planer
{
    public interface IAgent : IInteractor
    {
        public IPlaner Planer { get; set; }
        public Blackboard Blackboard { get; }
        public SkeletonAnimation Animator { get; }
        public void RemovePlaner()
        {
            Planer = null;
        }
        public void Move(float input);
        public void MoveHit();
        void ChangePlane<T>() where T : IState, new();
    }
}