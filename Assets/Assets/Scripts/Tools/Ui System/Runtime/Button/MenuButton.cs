using System;
using KabreetGames.SceneReferences;
using UnityEngine;
using UnityEngine.UI;

namespace KabreetGames.UiSystem
{
    [RequireComponent(typeof(Button))]
    public class MenuButton : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private Button button;

        [SerializeField] private MenuPanelProperty menuPanel;

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
            MenuManager.OpenPanel(menuPanel.MenuTypeValue);
        }
    }

    [Serializable]
    public struct MenuPanelProperty
    {
        public string displayName;
        public string menuType;
        public Type MenuTypeValue => Type.GetType(menuType);
    }
}