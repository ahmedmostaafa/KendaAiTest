using KabreetGames.SceneReferences;
using UnityEngine;
using UnityEngine.UI;

namespace KabreetGames.UiSystem
{
    public class QuitButton : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private Button button;

        private void OnEnable()
        {
            button.onClick.AddListener(OnClick);
        }
        
        private void OnDisable()
        {
            button.onClick.RemoveListener(OnClick);
        }
        
        private async void OnClick()
        {
            var result =
                await MenuManager.DisplayModalWindowAsync("Quit", "Are you sure you want to quit?", null, "Yes", "No");
            switch (result)
            {
                case -1:
                    Debug.LogWarning("Result in the quit button was -1 which is not a valid result, check if the modal window was configured correctly, the game will quit");
                    Quit();
                    break;
                case 0:
                    Quit();
                    break;
                case 1:
                    //Do nothing
                    break;
            }
        }


        private static void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
    }
}