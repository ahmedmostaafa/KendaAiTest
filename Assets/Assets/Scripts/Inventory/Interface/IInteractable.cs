using KendaAi.Agents.Planer;

namespace KendaAi.Inventory.Interface
{
    public interface IInteractable 
    {
        public void Interact(IAgent interactor );
        bool CanInteract(IAgent interactor);
    }
    
    public interface IInteractor
    {
        bool IsAlive { get; }
    }
}