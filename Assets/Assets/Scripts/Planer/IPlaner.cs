namespace KendaAi.Agents.Planer
{
    public interface IPlaner
    {
        IPlane currentPlane { get; }
        IAgent Agent { get; set; }
        void Update();
        void FixedUpdate();
        void DrawGizmos();
    }
    
    public interface IPlane
    {
    }
}