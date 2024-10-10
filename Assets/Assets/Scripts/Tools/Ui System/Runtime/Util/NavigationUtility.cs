using UnityEngine.UI;

namespace KabreetGames.UiSystem
{
    public static class NavigationUtility
    {
        /// <summary>
        /// Sets up the navigation between the provided Selectable elements based on the specified direction.
        /// </summary>
        /// <param name="selectables">Array of Selectable elements to set up navigation for.</param>
        /// <param name="direction">Direction of navigation (Horizontal or Vertical).</param>
        /// <param name="wrapNavigation">Determines if the navigation should wrap around the edges.</param>
        /// <returns>The first Selectable element in the array after setting up navigation.</returns>
        public static Selectable SetupNavigation(Selectable[] selectables,
            NavigationDirection direction = NavigationDirection.Horizontal,
            bool wrapNavigation = true)
        {
            if (selectables == null || selectables.Length == 0)
            {
                return null;
            }

            for (var i = 1; i < selectables.Length; i++)
            {
                if (direction == NavigationDirection.Vertical)
                {
                    // Vertical navigation (Up/Down)
                    var navPrev = selectables[i - 1].navigation;
                    navPrev.mode = Navigation.Mode.Explicit;
                    navPrev.selectOnDown = selectables[i];
                    selectables[i - 1].navigation = navPrev;

                    var navCurrent = selectables[i].navigation;
                    navCurrent.mode = Navigation.Mode.Explicit;
                    navCurrent.selectOnUp = selectables[i - 1];
                    selectables[i].navigation = navCurrent;
                }
                else
                {
                    // Horizontal navigation (Left/Right)
                    var navPrev = selectables[i - 1].navigation;
                    navPrev.mode = Navigation.Mode.Explicit;
                    navPrev.selectOnRight = selectables[i];
                    selectables[i - 1].navigation = navPrev;

                    var navCurrent = selectables[i].navigation;
                    navCurrent.mode = Navigation.Mode.Explicit;
                    navCurrent.selectOnLeft = selectables[i - 1];
                    selectables[i].navigation = navCurrent;
                }
            }

            if (!wrapNavigation) return selectables[0];
            if (direction == NavigationDirection.Vertical)
            {
                // Wrap vertical navigation (Up/Down)
                var navFirst = selectables[0].navigation;
                navFirst.selectOnUp = selectables[^1];
                selectables[0].navigation = navFirst;

                var navLast = selectables[^1].navigation;
                navLast.selectOnDown = selectables[0];
                selectables[^1].navigation = navLast;
            }
            else
            {
                // Wrap horizontal navigation (Left/Right)
                var navFirst = selectables[0].navigation;
                navFirst.selectOnLeft = selectables[^1];
                selectables[0].navigation = navFirst;

                var navLast = selectables[^1].navigation;
                navLast.selectOnRight = selectables[0];
                selectables[^1].navigation = navLast;
            }

            return selectables[0];
        }
        
        public static Selectable[] ConvertToSelectableArray(this Button[] buttons)
        {
            var selectables = new Selectable[buttons.Length];
            for (var i = 0; i < buttons.Length; i++)
            {
                selectables[i] = buttons[i];
            }
            return selectables;
        }
    }

    public enum NavigationDirection
    {
        Horizontal,
        Vertical
    }
}