using KabreetGames.SceneReferences;
using UnityEngine;

namespace KabreetGames.UiSystem
{
    [RequireComponent(typeof(SupMenuPanel))]
    public class TapMenu : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private SupMenuPanel baseMenu;
        public void Open()
        {
            MenuManager.OpenPanelSilent(baseMenu);
        }

        public void Close()
        {
            MenuManager.ClosePanelSilent(baseMenu);
        }
    }
}