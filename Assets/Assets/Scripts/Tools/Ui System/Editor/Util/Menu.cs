using System.Reflection;
using System;
using UnityEngine;

namespace KabreetGames.UiSystem
{
    public static class Menu
    {
        private static readonly MethodInfo AddMenuItemMethod;
        private static readonly MethodInfo RemoveMenuItemMethod;
        private static readonly MethodInfo MenuItemExistsMethod;

        static Menu()
        {
            var menuType = typeof(UnityEditor.Menu);

            AddMenuItemMethod = menuType.GetMethod(
                "AddMenuItem",
                BindingFlags.Static | BindingFlags.NonPublic
            );


            RemoveMenuItemMethod = menuType.GetMethod(
                "RemoveMenuItem",
                BindingFlags.Static | BindingFlags.NonPublic,
                null,
                new Type[] { typeof(string) },
                null
            );

            MenuItemExistsMethod = menuType.GetMethod(
                "MenuItemExists",
                BindingFlags.Static | BindingFlags.NonPublic,
                null,
                new Type[] { typeof(string) },
                null
            );

            if (AddMenuItemMethod == null)
                Debug.LogError("Failed to get AddMenuItem method.");
            if (RemoveMenuItemMethod == null)
                Debug.LogError("Failed to get RemoveMenuItem method.");
            if (MenuItemExistsMethod == null)
                Debug.LogError("Failed to get MenuItemExists method.");
        }

        public static void AddMenuItem(string name, string shortcut, bool isChecked, int priority, Action execute,
            Func<bool> validate)
        {
            if (AddMenuItemMethod == null)
            {
                Debug.LogError("AddMenuItem method is not available.");
                return;
            }

            AddMenuItemMethod.Invoke(
                null,
                new object[] { name, shortcut, isChecked, priority, execute, validate }
            );
        }

        public static void RemoveMenuItem(string name)
        {
            if (RemoveMenuItemMethod == null)
            {
                Debug.LogError("RemoveMenuItem method is not available.");
                return;
            }

            RemoveMenuItemMethod.Invoke(null, new object[] { name });
        }

        public static bool MenuItemExists(string name)
        {
            if (MenuItemExistsMethod != null) return (bool)MenuItemExistsMethod.Invoke(null, new object[] { name });
            Debug.LogError("MenuItemExists method is not available.");
            return false;
        }
    }
}