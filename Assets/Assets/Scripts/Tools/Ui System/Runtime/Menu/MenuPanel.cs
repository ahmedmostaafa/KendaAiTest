using UnityEngine;
using KabreetGames.EventBus.Interfaces;
using Sirenix.OdinInspector;

namespace KabreetGames.UiSystem
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class MenuPanel : BaseMenu
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            EventBus<OpenMenuEvent>.Register(ShowMenu);
            EventBus<DeActiveMenuEvent>.Register(DeActiveMenu);
            EventBus<ActiveMenuEvent>.Register(ActiveMenu);
            EventBus<CloseMenuEvent>.Register(HideMenu);
        }


        protected override void OnDisable()
        {
            base.OnDisable();
            EventBus<OpenMenuEvent>.Deregister(ShowMenu);
            EventBus<DeActiveMenuEvent>.Deregister(DeActiveMenu);
            EventBus<ActiveMenuEvent>.Deregister(ActiveMenu);
            EventBus<CloseMenuEvent>.Deregister(HideMenu);
        }

        private void ShowMenu(OpenMenuEvent obj)
        {
            if (obj.menuType == GetType())
            {
                Show(true);
            }
            else
            {
                if (obj.closeOther) Show(false);
            }
        }


        private void HideMenu(CloseMenuEvent obj)
        {
            if (obj.menuType == GetType())
            {
                Show(false);
            }
        }

        private void DeActiveMenu(DeActiveMenuEvent obj)
        {
            if (obj.menuType == GetType())
            {
                ChangeActiveState(false);
            }
        }

        private void ActiveMenu(ActiveMenuEvent obj)
        {
            if (obj.menuType == GetType())
            {
                ChangeActiveState(true);
            }
        }


        [HorizontalGroup, Button(SdfIconType.EyeFill)]
        public override void Show()
        {
            if (Application.isPlaying)
            {
                MenuManager.OpenPanel(this);
            }
            else
            {
                base.Show();
            }
        }

        [HorizontalGroup, Button(SdfIconType.EyeSlash), TableColumnWidth(60)]
        public override void Hide()
        {
            if (Application.isPlaying)
            {
                MenuManager.ClosePanel(this);
            }
            else
            {
                base.Hide();
            }
        }
    }

    public interface IMenuPanel
    {
        public CanvasGroup Group { get; }
        public bool Active { get; }

        public void Show(bool show);

        public void Show();

        public void Hide();
    }
}