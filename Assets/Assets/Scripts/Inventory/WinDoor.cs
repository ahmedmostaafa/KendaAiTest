using KabreetGames.SceneReferences;
using KendaAi.Agents.Planer;
using KendaAi.Inventory.Interface;
using Unity.Cinemachine;
using UnityEngine;

namespace KendaAi.Inventory
{
    public class WinDoor:ValidatedMonoBehaviour, IInteractable
    {
        [SerializeField, Child] private CinemachineCamera cinemachineCamera;

        private void ShowCamera()
        {
            cinemachineCamera.Priority.Value = 10;
        }
        public void Interact(IAgent interactor)
        {
            ShowCamera();
        }

        public bool CanInteract(IAgent interactor)
        {
            return interactor.IsAlive;
        }
    }
}