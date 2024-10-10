namespace KendaAi.Agents.Planer
{
    public interface IState : IPlane
    {
        public string stateName { get; }
        bool isExited { get; set; }

        void Init(StateMachine machine);
        void InternalEnter();
        void Enter();
        void Update();
        void FixedUpdate();
        void Exit();
        void UpdateAnimator();
    }
}