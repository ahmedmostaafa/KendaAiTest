using KabreetGames.SceneManagement;
using KabreetGames.SceneReferences;
using KabreetGames.UiSystem;
using KendaAi.Agents.Planer;
using KendaAi.Inventory.Interface;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KendaAi.Inventory
{
    public class WinDoor : ValidatedMonoBehaviour, IInteractable
    {
        [SerializeField, Child] private CinemachineCamera cinemachineCamera;

        private void ShowCamera()
        {
            cinemachineCamera.Priority.Value = 10;
        }

        public async void Interact(IAgent interactor)
        {
            ShowCamera();
            var value = await MenuManager.DisplayModalWindowAsync("Congratulations", "You won!", null, "Restart",
                "MainMenu");

            switch (value)
            {
                case 0:
                    await SceneLoadingManager.Restart(loadSceneMode : LoadSceneMode.Single);
                    break;
                case 1:
                    await SceneLoadingManager.ReplaceScene(SceneGroupNames.MainMenu, SceneGroupNames.GamePlay, loadSceneMode: LoadSceneMode.Single);
                    break;
            }
        }

        public bool CanInteract(IAgent interactor)
        {
            return interactor.IsAlive;
        }
    }
}