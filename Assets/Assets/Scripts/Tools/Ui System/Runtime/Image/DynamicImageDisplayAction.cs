using KabreetGames.SceneReferences;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace KabreetGames.UiSystem
{
    public class DynamicImageDisplayAction : ValidatedMonoBehaviour
    {
        [SerializeField] private InputActionReference action;
        [SerializeField, Child] private Image iconImage;
        [SerializeField, Child] private TextMeshProUGUI text;

        [SerializeField, BindingDirection(nameof(action))]
        private string direction;

        private InputDevice lastDevice;
        [SerializeField, Required] private BindingsAssets bindingsAssets;

        private void OnEnable()
        {
            if (!action) return;
            InputSystem.onActionChange += OnActionChange;

        }

        private void OnDisable()
        {
            if (!action) return;
            InputSystem.onActionChange -= OnActionChange;
        }

        private void OnActionChange(object actionObj, InputActionChange change)
        {
            if (change != InputActionChange.ActionPerformed) return;
            if (actionObj is not InputAction inputAction) return;
            
            var activeControl = inputAction.activeControl;
            var inputDevice = activeControl.device;
            if (ReferenceEquals(lastDevice, inputDevice)) return;

            var binding = inputAction.GetBindingForControl(activeControl);
            if (!binding.HasValue || string.IsNullOrEmpty(binding.Value.groups)) return;
            
            var bindingGroups = binding.Value.groups.Replace(";", "");
            var bindingIndex = GetBindingIndex(inputDevice, bindingGroups);
            
            if (bindingIndex == -1) return;
            var displayString = action.action.GetBindingDisplayString(bindingIndex, out var deviceLayoutName,
                out var controlPath, InputBinding.DisplayStringOptions.IgnoreBindingOverrides);
            text.text = displayString;
            OnUpdateBindingDisplay(deviceLayoutName, controlPath);
            lastDevice = inputDevice;
        }

        private int GetBindingIndex(InputDevice inputDevice, string inputBindingGroups)
        {
            var bindings = action.action.bindings;

            for (var i = 0; i < bindings.Count; i++)
            {
                var binding = bindings[i];
                if (binding.name == direction && binding.groups == inputBindingGroups)
                    return i;
            }

            for (var i = 0; i < bindings.Count; i++)
            {
                var binding = bindings[i];
                if (binding.name != direction) continue;

                if (InputControlPath.TryFindControl(inputDevice, binding.effectivePath) != null)
                    return i;
            }

            return -1;
        }

        private void OnUpdateBindingDisplay(string deviceLayoutName, string controlPath)
        {
            if (string.IsNullOrEmpty(deviceLayoutName) || string.IsNullOrEmpty(controlPath))
                return;

            var icon = default(Sprite);
            if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "DualShockGamepad"))
                icon = bindingsAssets.ps4.GetSprite(controlPath);
            else if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "Gamepad"))
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