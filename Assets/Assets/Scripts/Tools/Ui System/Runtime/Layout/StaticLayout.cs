using KabreetGames.SceneReferences;
using UnityEngine;
using UnityEngine.UI;

namespace KabreetGames.UiSystem
{
    [RequireComponent(typeof(LayoutGroup))]
    public class StaticLayout : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private LayoutGroup layoutGroup;

        [SerializeField, Self] private RectTransform rectTransform;

        private void OnEnable()
        {
            layoutGroup.enabled = false;
        }
    }
}