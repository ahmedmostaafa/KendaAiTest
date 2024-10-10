using KabreetGames.SceneReferences;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace KabreetGames.UiSystem
{
    [RequireComponent(typeof(Button))]
    public class InputActionButton : ValidatedMonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private InputActionReference inputActionReference;
        [SerializeField, Self] private Button button;

        [SerializeField, BindingDirection(nameof(inputActionReference))]
        private string direction;
        
        public UnityEvent onClick;
        public UnityEvent onClickDown;
        public UnityEvent onClickUp;
        [SerializeField, Parent] private BaseMenu menu;

        private void OnEnable()
        {
            if (menu == null) return;
            if (menu.Active) Activate();
            menu.OnShowChanged += MenuOnShowChanged;
        }

        private void OnDisable()
        {
            Deactivate();
            if (menu == null) return;
            menu.OnShowChanged -= MenuOnShowChanged;
        }

        private void MenuOnShowChanged(bool active)
        {
            if (active)
            {
                Activate();
            }
            else
            {
                Deactivate();
            }
        }

        private void Activate()
        {
            button.onClick.AddListener(OnClick);
            // inputActionReference.action.Enable();
            // inputActionReference.action.performed += OnClick;
        }


        private void Deactivate()
        {
            // inputActionReference.action.Disable();
            button.onClick.RemoveListener(OnClick);
            // inputActionReference.action.performed -= OnClick;
        }

        private void OnClick(InputAction.CallbackContext obj)
        {
            // OnClick();
        }

        private void OnClick()
        {
            inputActionReference.action?.PerformInteractiveRebinding().WithTargetBinding(0).Dispose();
            onClick.Invoke();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            onClickDown.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            onClickUp.Invoke();
        }
    }
}