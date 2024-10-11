using KabreetGames.SceneManagement;
using KabreetGames.SceneReferences;
using KabreetGames.UiSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.UI.MainMenu
{
    public class MainMenuPanel : MenuPanel
    {
        [SerializeField, Child(Flag.Editable)] private Button playButton;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            playButton.onClick.AddListener(OnPlayButton);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            playButton.onClick.RemoveListener(OnPlayButton);
        }

        private async void OnPlayButton()
        {
            await SceneLoadingManager.ReplaceScene(SceneGroupNames.GamePlay, SceneGroupNames.MainMenu, loadSceneMode: LoadSceneMode.Single);
        }
    }
}