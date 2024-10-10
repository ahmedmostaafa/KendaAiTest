using KabreetGames.EventBus.Interfaces;
using KabreetGames.SceneReferences;
using KendaAi.Agents.Planer;
using KendaAi.Events;
using KendaAi.Inventory.Interface;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KendaAi.Inventory
{
    public class Jem : ValidatedMonoBehaviour, IInteractable
    {
      [SerializeField, Required] private Sprite sprite;

        public void Interact(IAgent interactor)
        {
            EventBus<CoinCollectedEvent>.Rise(new CoinCollectedEvent(sprite, transform.position));
            Destroy(gameObject);
        }

        public bool CanInteract(IAgent interactor) => true;
    }
}