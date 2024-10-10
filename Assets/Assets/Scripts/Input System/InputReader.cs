using System;
using KabreetGames.EventBus.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace KendaAi.Agents.InputSystem
{
    [CreateAssetMenu(fileName = "Input Reader", menuName = "Input System/Input Reader", order = 0)]
    public class InputReader : ScriptableObject, ActionMap.IPlayerActions
    {
        [field: SerializeField]
        public PlayerInputDevice ActiveDevice { get; private set; } = PlayerInputDevice.Keyboard;
        private ActionMap inputActions;
        public ActionMap Action => inputActions;
        
        public float HorizontalMove => inputActions.Player.Move.ReadValue<float>();
        public void EnablePlayerActions()
        {
            inputActions = new ActionMap();
            InputUser.onChange += OnUserChange;
            inputActions.Enable();
            inputActions.Player.SetCallbacks(this);
        }
        public void DisablePlayerActions()
        {
            if (inputActions != null)
            {
                inputActions.Disable();
                inputActions.Player.RemoveCallbacks(this);
            }
            InputUser.onChange -= OnUserChange;
        }
        public void OnMove(InputAction.CallbackContext context) { }

        public void OnInteract(InputAction.CallbackContext context) { }
        
        private void OnUserChange(InputUser user, InputUserChange change, InputDevice device)
        {
            switch (change)
            {
                case InputUserChange.DeviceLost:
                case InputUserChange.DeviceRegained:
                    switch (change)
                    {
                        case InputUserChange.DeviceLost:
                            Debug.Log(device);
                            break;
                        case InputUserChange.DeviceRegained:
                            Debug.Log(device);
                            break;
                    }
                    break;
        
                case InputUserChange.ControlsChanged:
                    if (user.controlScheme != null)
                    {
                        var scheme = user.controlScheme.Value;
                        var inputDevice = scheme.name.Equals("Gamepad")
                            ? PlayerInputDevice.GamePad
                            : PlayerInputDevice.Keyboard;
                        ActiveDevice = inputDevice;
                        EventBus<ControlsChangedEvent>.Rise(new ControlsChangedEvent()
                        {
                            inputDevice = inputDevice
                        });
                    }
                    break;
                case InputUserChange.ControlSchemeChanged:
                    break;
                case InputUserChange.Added:
                    break;
                case InputUserChange.Removed:
                    break;
                case InputUserChange.DevicePaired:
                    break;
                case InputUserChange.DeviceUnpaired:
                    break;
                case InputUserChange.AccountChanged:
                    break;
                case InputUserChange.AccountNameChanged:
                    break;
                case InputUserChange.AccountSelectionInProgress:
                    break;
                case InputUserChange.AccountSelectionCanceled:
                    break;
                case InputUserChange.AccountSelectionComplete:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(change), change, null);
            }
        }
    }
    public enum PlayerInputDevice
    {
        Keyboard,
        GamePad,
    }
    public struct ControlsChangedEvent : IEvent
    {
        public PlayerInputDevice inputDevice;
    }
}