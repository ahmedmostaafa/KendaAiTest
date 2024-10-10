using KabreetGames.SceneReferences;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KabreetGames.UiSystem
{
    [RequireComponent(typeof(Image))]
    public class Tap : ValidatedMonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private bool isOn;
        [SerializeField, Self] private Image graphic;
        public bool IsOn => isOn;
        public TapGroup tapGroup;
        public TapMenu tapMenu;
        public UnityEvent<bool> onValueChanged = new();

        protected void OnEnable()
        {
            if (tapGroup != null) tapGroup.RegisterTap(this);
        }

        protected void OnDisable()
        {
            if (tapGroup != null) tapGroup.DeRegisterTap(this);
        }

        public void OnSelect()
        {
            ApplyActivation(true);
        }

        public void OnDeselect()
        {
            ApplyActivation(false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            HandelActivation();
        }

        private void HandelActivation()
        {
            if (tapGroup != null)
            {
                tapGroup.OnTapClicked(this);
                return;
            }

            ApplyActivation(!isOn);
        }

        private void ApplyActivation(bool value)
        {
            if (value != isOn)
            {
                isOn = value;
                onValueChanged.Invoke(isOn);
            } 
            HandelMenu(isOn);
            HandleVisuals();
        }

        private void HandleVisuals()
        {
            if (graphic == null) return;
            graphic.CrossFadeColor(isOn ? Color.white : Color.gray, 0.1f, true, true);
        }
        
        private void HandelMenu(bool value)
        {
            if (tapMenu == null) return;
            if (value)
            {
                tapMenu.Open();
            }
            else
            {
                tapMenu.Close();
            }
        }


        private void OnValidate()
        {
            if (graphic == null) graphic = GetComponent<Image>();
            HandleVisuals();
        }
    }
}