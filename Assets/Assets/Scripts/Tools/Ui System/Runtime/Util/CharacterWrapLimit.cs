using System.Linq;
using TMPro;
using UnityEngine.UI;

namespace KabreetGames.UiSystem
{
    public static class CharacterWrapLimit
    {
        public static bool WrapText(this LayoutElement layout, int limit, params TextMeshProUGUI[] texts)
        {
            var wrapped = texts.Any(x => x.text.Length > (limit));
            layout.enabled = wrapped;
            return wrapped;
        }
    }
}