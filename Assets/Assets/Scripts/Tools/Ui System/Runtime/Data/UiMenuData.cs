using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KabreetGames.UiSystem
{
    public class UiMenuData : ScriptableObject
    {
        [SerializeField] private InputActionReference backAction;
        [SerializeField] private InputActionReference tapNavigationAction;
        public GameObjectData actionButtonPrefab;
        public GameObjectData menuPrefab;
        [HideInInspector] public List<GameObjectData> prefabs;


        public void AddPrefab(GameObject prefab)
        {
            if (prefab == null) return;
            if (prefabs.Any(p => p.prefab == prefab)) return;
            prefabs.Add(new GameObjectData
            {
                prefab = prefab
            });
        }

        public void RegisterControls()
        {
            RegisterBack();
            RegisterTapNavigation();
        }

        private void RegisterTapNavigation()
        {
            if (tapNavigationAction == null) return;
            tapNavigationAction.action.Disable();
            tapNavigationAction.action.Enable();
            tapNavigationAction.action.performed -= TapNavigate;
            tapNavigationAction.action.performed += TapNavigate;
        }

        private void TapNavigate(InputAction.CallbackContext obj)
        {
            var direction = obj.ReadValue<float>();
            MenuManager.TapNavigateTo((int) direction);
        }

        private void RegisterBack()
        {
            if (backAction == null) return;
            backAction.action.Disable();
            backAction.action.Enable();
            backAction.action.performed -= Back;
            backAction.action.performed += Back;
        }

        private static void Back(InputAction.CallbackContext obj)
        {
            MenuManager.Back();
        }
    }

    [Serializable]
    public class GameObjectData
    {
        public GameObject prefab;
        public Texture thumbnail;
    }
}