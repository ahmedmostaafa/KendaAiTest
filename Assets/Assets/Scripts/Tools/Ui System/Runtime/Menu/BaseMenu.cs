using System;
using KabreetGames.SceneReferences;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KabreetGames.UiSystem
{
    public abstract class BaseMenu : ValidatedMonoBehaviour, IMenuPanel
    {
        [field: SerializeField, Self] public CanvasGroup Group { get; protected set; }
        public bool Active { get; private set; }

        public Action<bool> OnShowChanged { get; set; }

        [SerializeField, Child(Flag.Optional | Flag.Editable)]
        private Selectable firstSelected;

        public virtual void Show(bool show)
        {
            if (show == Active)
            {
                SelectObject();
                return;
            }

            if (show)
            {
                ShowEffect();
            }
            else
            {
                HideEffect();
            }

            ChangeActiveState(show);
            OnShowChanged?.Invoke(show);
        }

        protected virtual void ChangeActiveState(bool active)
        {
            Group.interactable = active;
            Group.blocksRaycasts = active;
            Active = active;
            if (active) SelectObject();
        }


        protected virtual void ShowEffect()
        {
            Group.alpha = 1;
        }

        protected virtual void HideEffect()
        {
            Group.alpha = 0;
        }

        protected virtual void OnEnable()
        {
            Active = InitActiveState();
            MenuManager.RegisterPanel(this);
            if (Active && EventSystem.current != null && firstSelected != null)
            {
                EventSystem.current.firstSelectedGameObject = firstSelected.gameObject;
            }
        }

        protected virtual bool InitActiveState()
        {
            return Group.alpha > 0;
        }

        protected virtual void SelectObject()
        {
            if (!Active || EventSystem.current == null) return;
            if (firstSelected != null)
            {
                firstSelected.Select();
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }

        protected virtual void OnDisable() => MenuManager.DeregisterPanel(this);

        [HorizontalGroup, Button(SdfIconType.EyeFill)]
        public virtual void Show() => Show(true);

        [HorizontalGroup, Button(SdfIconType.EyeSlash), TableColumnWidth(60)]
        public virtual void Hide() => Show(false);

        protected virtual void OnValidate()
        {
            if (Group == null) Group = GetComponent<CanvasGroup>();
            Active = Group.alpha > 0;
        }

        public virtual bool BackAvailable() => true;
    }
}