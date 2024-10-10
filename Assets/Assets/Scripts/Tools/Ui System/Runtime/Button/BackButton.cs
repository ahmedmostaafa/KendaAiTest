using KabreetGames.SceneReferences;
using UnityEngine;
using UnityEngine.UI;

namespace KabreetGames.UiSystem
{
    public class BackButton : ValidatedMonoBehaviour
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

        private void OnClick()
        {
            MenuManager.Back();
        }
    }
}