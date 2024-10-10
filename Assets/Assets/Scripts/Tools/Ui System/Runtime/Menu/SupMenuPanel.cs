using KabreetGames.SceneReferences;
using UnityEngine;

namespace KabreetGames.UiSystem
{
    public class SupMenuPanel : MenuPanel
    {
        [SerializeField, Parent] private MenuPanel menuPanel;

        protected override bool InitActiveState()
        {
            return base.InitActiveState() && menuPanel.Active;
        }
    }
}