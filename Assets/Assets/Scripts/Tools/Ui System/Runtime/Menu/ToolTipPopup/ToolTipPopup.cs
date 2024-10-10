using KabreetGames.SceneReferences;
using TMPro;
using UnityEngine;

namespace KabreetGames.UiSystem
{
    public class ToolTipPopup : BaseMenu
    {
        [SerializeField, Child(Flag.Editable)] private TextMeshProUGUI titleText;
        [SerializeField, Child(Flag.Editable)] private TextMeshProUGUI contentText;

        protected override void OnEnable()
        {
            Show(false);
            base.OnEnable();
        }
        public override bool BackAvailable() => false;
    }
}