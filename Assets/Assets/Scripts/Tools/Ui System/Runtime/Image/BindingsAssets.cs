using UnityEngine;

namespace KabreetGames.UiSystem
{
    [CreateAssetMenu(fileName = "Bindings", menuName = "Bindings", order = 0)]
    public class BindingsAssets : ScriptableObject
    {
        public GamepadIcons xbox;
        public GamepadIcons ps4;
        public Sprite keyboard;
    }
}