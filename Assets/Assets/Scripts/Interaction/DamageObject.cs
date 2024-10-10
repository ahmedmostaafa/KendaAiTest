using KabreetGames.SceneReferences;
using KendaAi.Agents.Planer;
using KendaAi.Inventory.Interface;
using KendaAi.TestProject.PlayerController.States;

namespace KendaAi.Scripts.Interaction
{
    public class DamageObject : ValidatedMonoBehaviour, IInteractable
    {
        public void Interact(IAgent interactor)
        {
            interactor.ChangePlane<HitState>();
        }

        public bool CanInteract(IAgent interactor)
        {
            return interactor.IsAlive;
        }
    }
}