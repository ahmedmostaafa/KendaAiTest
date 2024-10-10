using KabreetGames.SceneReferences;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace KabreetGames.UiSystem
{
    public class StaticImageDisplayAction : ValidatedMonoBehaviour
    {
        [SerializeField] private InputActionReference action;
        [SerializeField, Child] private Image iconImage;

        [SerializeField, Child] private TextMeshProUGUI text;

        [SerializeField, BindingId(nameof(action))]
        private string binding;

        [SerializeField, Required] private BindingsAssets bindingsAssets;

        [Button]
        private void UpdateBindingDisplay()
        {
            var displayString = string.Empty;
            var deviceLayoutName = default(string);
            var controlPath = default(string);

            // Get display string from action.
            var inputAction = action?.action;
            if (inputAction == null) return;
            var bindingIndex = inputAction.bindings.IndexOf(x => x.id.ToString() == binding);
            if (bindingIndex != -1)
                displayString = inputAction.GetBindingDisplayString(bindingIndex, out deviceLayoutName,
                    out controlPath, InputBinding.DisplayStringOptions.IgnoreBindingOverrides);
            text.text = displayString;
            OnUpdateBindingDisplay(deviceLayoutName, controlPath);
        }

        private void OnUpdateBindingDisplay(string deviceLayoutName, string controlPath)
        {
            if (string.IsNullOrEmpty(deviceLayoutName) || string.IsNullOrEmpty(controlPath))
                return;

            var icon = default(Sprite);
            if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName,
                    "DualShockGamepad"))
                icon = bindingsAssets.ps4.GetSprite(controlPath);
            else if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName,
                         "Gamepad"))
                icon = bindingsAssets.xbox.GetSprite(controlPath);

            if (icon != null)
            {
                text.gameObject.SetActive(false);
                iconImage.sprite = icon;
            }
            else
            {
                iconImage.sprite = bindingsAssets.keyboard;
                text.gameObject.SetActive(true);
            }
        }
    }
}