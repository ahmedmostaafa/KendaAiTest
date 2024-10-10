using KabreetGames.EventBus.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KabreetGames.UiSystem
{
    public static class MenuManager
    {
        public static Type activePanel;
        public static readonly Stack<Type> HistoryStack = new();
        public static Dictionary<Type, IMenuPanel> Panels { get; } = new();

        public static TapGroup activeTapGroup;

        private static UiMenuData uiData;

        public static UiMenuData Data
        {
            get { return uiData ??= Resources.Load<UiMenuData>("UiMenuData"); }

            set => uiData = value;
        }

        private static ModalWindow modalWindow;

        private static ModalWindow ModalWindow
        {
            get
            {
                if (modalWindow != null) return modalWindow;
                if (Panels.TryGetValue(typeof(ModalWindow), out var panel))
                {
                    modalWindow = panel as ModalWindow;
                }

                return modalWindow;
            }
        }

        private static ToolTipPopup toolTipPopup;

        private static ToolTipPopup ToolTipPopup
        {
            get
            {
                if (toolTipPopup != null) return toolTipPopup;
                if (Panels.TryGetValue(typeof(ToolTipPopup), out var panel))
                {
                    toolTipPopup = panel as ToolTipPopup;
                }

                return toolTipPopup;
            }
        }


        static MenuManager()
        {
            Data?.RegisterControls();
        }

        public static void OpenPanel<T>() where T : BaseMenu
        {
            OpenPanel(typeof(T), false, false);
        }

        public static void OpenPanelSilent<T>() where T : BaseMenu
        {
            OpenPanel(typeof(T), false, true);
        }

        public static void OpenPanel<T>(bool closeOther) where T : BaseMenu
        {
            OpenPanel(typeof(T), closeOther, false);
        }

        public static void OpenPanel<T>(T panel) where T : BaseMenu
        {
            OpenPanel(panel.GetType(), false, false);
        }

        public static void OpenPanelSilent<T>(T panel) where T : BaseMenu
        {
            OpenPanel(panel.GetType(), false, true);
        }

        public static void OpenPanel(Type panelType)
        {
            if (panelType == null)
            {
                Debug.LogError("Panel type is null.");
                return;
            }

            if (!typeof(BaseMenu).IsAssignableFrom(panelType))
            {
                Debug.LogError("The provided type is not a BaseMenu.");
                return;
            }

            OpenPanel(panelType, false, false);
        }

        public static void OpenPanel(Type panelType, bool closeOther)
        {
            if (panelType == null)
            {
                Debug.LogError("Panel type is null.");
                return;
            }

            if (!typeof(BaseMenu).IsAssignableFrom(panelType))
            {
                Debug.LogError("The provided type is not a BaseMenu.");
                return;
            }

            OpenPanel(panelType, closeOther, false);
        }

        public static void OpenPanelSilent(Type panelType)
        {
            if (panelType == null)
            {
                Debug.LogError("Panel type is null.");
                return;
            }

            if (!typeof(BaseMenu).IsAssignableFrom(panelType))
            {
                Debug.LogError("The provided type is not a BaseMenu.");
                return;
            }

            OpenPanel(panelType, false, true);
        }


        private static void OpenPanel(Type panelType, bool closeOther, bool silent)
        {
            if (activePanel == panelType)
            {
                return;
            }

            if (activePanel != null && !silent)
            {
                if (HistoryStack.Count == 0 || HistoryStack.Peek() != activePanel)
                {
                    HistoryStack.Push(activePanel);
                }
            }

            if (!silent)
            {
                if (activePanel != null)
                {
                    ClosePanel(activePanel, true);
                }
                activePanel = panelType;
            }

            EventBus<OpenMenuEvent>.Rise(new OpenMenuEvent(closeOther, panelType));
        }

        public static void ClosePanel<T>() where T : BaseMenu
        {
            ClosePanel(typeof(T), false);
        }

        public static void ClosePanel<T>(T panel) where T : BaseMenu
        {
            ClosePanel(panel.GetType(), false);
        }

        public static void ClosePanelSilent<T>() where T : BaseMenu
        {
            ClosePanel(typeof(T), true);
        }

        public static void ClosePanelSilent<T>(T panel) where T : BaseMenu
        {
            ClosePanel(panel.GetType(), true);
        }

        private static void ClosePanel(Type panelType, bool silent)
        {
            if (activePanel != panelType && !silent) return;
            EventBus<CloseMenuEvent>.Rise(new CloseMenuEvent(panelType));

            if (silent) return;
            if (HistoryStack.Count > 0 && HistoryStack.Peek() == panelType)
            {
                activePanel = HistoryStack.Pop();
            }
        }


        public static T GetPanel<T>() where T : BaseMenu
        {
            return GetPanel<T>(typeof(T));
        }

        public static T GetPanel<T>(Type panelType) where T : BaseMenu
        {
            if (Panels.TryGetValue(panelType, out var panel)) return panel as T;
            Debug.LogWarning("Panel not found: " + panelType);
            return null;
        }


        public static void Back()
        {
            if (activePanel == null || HistoryStack.Count == 0)
            {
                Debug.LogWarning("No panel to close.");
                return;
            }

            var panel = GetPanel<BaseMenu>(activePanel);
            if (panel == null || !panel.BackAvailable()) return;
            EventBus<CloseMenuEvent>.Rise(new CloseMenuEvent(activePanel));

            var previousPanel = HistoryStack.Pop();
            activePanel = previousPanel;
            EventBus<OpenMenuEvent>.Rise(new OpenMenuEvent(false, previousPanel));
        }


        public static async Task<int> DisplayModalWindowAsync(string title, string content, Sprite image,
            params string[] options)
        {
            if (ModalWindow == null) return -1;

            var panel = ModalWindow.GetType();

            if (activePanel == panel)
            {
                return -1;
            }

            if (activePanel != null)
            {
                if (HistoryStack.Count == 0 || HistoryStack.Peek() != activePanel)
                {
                    HistoryStack.Push(activePanel);
                }
            }

            var lastSelected = EventSystem.current.currentSelectedGameObject;
            EventBus<DeActiveMenuEvent>.Rise(new DeActiveMenuEvent(activePanel));
            activePanel = panel;
            var result = await ModalWindow.DisplayActionAsync(title, content, image, options);
            activePanel = HistoryStack.Count > 0 ? HistoryStack.Pop() : null;
            EventBus<ActiveMenuEvent>.Rise(new ActiveMenuEvent(activePanel));
            if (lastSelected != null) EventSystem.current.SetSelectedGameObject(lastSelected, null);
            return result;
        }


        public static void RegisterPanel<T>(T panel) where T : IMenuPanel
        {
            if (Panels.ContainsKey(panel.GetType()))
            {
                Panels[panel.GetType()] = panel;
            }
            else
            {
                Panels.Add(panel.GetType(), panel);
            }

            if (!panel.Active) return;
            activePanel = panel.GetType();
        }

        public static void DeregisterPanel<T>(T panel) where T : IMenuPanel
        {
            Panels.Remove(panel.GetType());
        }

        public static void CleanUp()
        {
            Panels.Clear();
            activePanel = null;
            HistoryStack.Clear();
        }

        public static void TapNavigateTo(int direction)
        {
            if (activeTapGroup == null) return;
            activeTapGroup.NavigateTo(direction);
        }


        public static void RegisterActiveTapGroup(TapGroup tapGroup) => activeTapGroup = tapGroup;

        public static void DeregisterActiveTapGroup(TapGroup tapGroup) => activeTapGroup = null;
    }


    public struct OpenMenuEvent : IEvent
    {
        public readonly Type menuType;
        public readonly bool closeOther;

        public OpenMenuEvent(bool closeOther, Type menuType)
        {
            this.closeOther = closeOther;
            this.menuType = menuType;
        }
    }

    public struct CloseMenuEvent : IEvent
    {
        public readonly Type menuType;

        public CloseMenuEvent(Type menuType)
        {
            this.menuType = menuType;
        }
    }

    public struct DeActiveMenuEvent : IEvent
    {
        public readonly Type menuType;

        public DeActiveMenuEvent(Type panel)
        {
            menuType = panel;
        }
    }

    public struct ActiveMenuEvent : IEvent
    {
        public readonly Type menuType;

        public ActiveMenuEvent(Type panel)
        {
            menuType = panel;
        }
    }
}