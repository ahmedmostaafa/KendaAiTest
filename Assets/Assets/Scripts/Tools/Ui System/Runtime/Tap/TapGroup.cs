using System.Collections.Generic;
using System.Linq;
using KabreetGames.SceneReferences;
using UnityEngine;
using UnityEngine.Events;

namespace KabreetGames.UiSystem
{
    public class TapGroup : ValidatedMonoBehaviour
    {
        [SerializeField] private bool allowSwitchOff = true;
        [SerializeField] private bool allowWrapping = true;
        public UnityEvent<Tap> onTapActiveChanged = new();
        private Tap activeTap;
        private int activeTapIndex = -1;
        private Tap activeTapBeforeMenu;

        [SerializeField, SelfOrParent] private BaseMenu tapMenu;

        [SerializeField] private List<Tap> taps = new();

        private void OnEnable()
        {
            if (tapMenu == null) return;
            tapMenu.OnShowChanged += OnBaseMenuShow;
            if(tapMenu.Active) OnBaseMenuShow(true); 
        }

        private void OnDisable()
        {
            if (tapMenu == null) return;
            tapMenu.OnShowChanged -= OnBaseMenuShow;
        }

        private void OnBaseMenuShow(bool show)
        {
            if (show)
            {
                MenuManager.RegisterActiveTapGroup(this);
                if (activeTap == null) return;
                activeTap.OnSelect();
                activeTapBeforeMenu = activeTap;
            }
            else
            {
                MenuManager.DeregisterActiveTapGroup(this);
                RestToDefault();
            }
        }

        private void RestToDefault()
        {
            if (activeTap == null) return;
            activeTap.OnDeselect();
            activeTap = activeTapBeforeMenu;
            activeTapIndex = taps.IndexOf(activeTap);
        }

        public void OnTapClicked(Tap tap)
        {
            if (activeTap == tap && allowSwitchOff)
            {
                if (activeTap == null) return;
                activeTap.OnDeselect();
                activeTap = null;
                onTapActiveChanged.Invoke(null);
                return;
            }

            activeTap = tap;
            onTapActiveChanged.Invoke(activeTap);
            foreach (var t in taps.Where(t => t != activeTap))
            {
                t.OnDeselect();
            }

            if (activeTap != null) activeTap.OnSelect();
            activeTapIndex = taps.IndexOf(activeTap);
        }

        public void RegisterTap(Tap tap)
        {
            if (taps.Contains(tap)) return;
            taps.Add(tap);
            if (activeTap != null || !tap.IsOn) return;
            activeTap = tap;
            activeTap.OnSelect();
            onTapActiveChanged.Invoke(tap);
            activeTapIndex = taps.Count - 1;
        }

        public void DeRegisterTap(Tap tap) => taps.Remove(tap);


        public void NavigateTo(int direction)
        {
            if (activeTap == null) return;
            var index = activeTapIndex + direction;
            if (allowWrapping) index = (index + taps.Count) % taps.Count;
            if (index < 0 || index >= taps.Count || index == activeTapIndex) return;
            OnTapClicked(taps[index]);
        }
    }
}