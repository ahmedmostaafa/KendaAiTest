using System.Collections.Generic;
using System.Threading.Tasks;
using KabreetGames.SceneReferences;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace KabreetGames.UiSystem
{
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class ModalWindow : BaseMenu
    {
        private int result = -1;
        private readonly Stack<Button> generatedButtons = new();

        [SerializeField, Child(Flag.Editable)] private Image displayImage;
        [SerializeField, Child(Flag.Editable)] private TextMeshProUGUI titleText;
        [SerializeField, Child(Flag.Editable)] private TextMeshProUGUI contentText;
        [SerializeField, Child(Flag.Editable)] private Transform buttonContainer;

        [SerializeField, Child(Flag.Editable)] private LayoutElement contentTextLayout;

        private LinkedPool<Button> buttonPool;

        private LinkedPool<Button> ButtonPool => buttonPool ??=
            new LinkedPool<Button>(CreatButton, GetButton, ReleaseButton, DestroyButton);


        protected override void OnEnable()
        {
            Show(false);
            base.OnEnable();
        }

        public override bool BackAvailable() => false;

        private Button CreatButton()
        {
            if (MenuManager.Data != null && MenuManager.Data.actionButtonPrefab != null)
            {
                return Instantiate(MenuManager.Data.actionButtonPrefab.prefab, buttonContainer).GetComponent<Button>();
            }

            var newButton = new GameObject("Button", typeof(RectTransform), typeof(Image), typeof(Button))
                .GetComponent<Button>();
            newButton.transform.SetParent(buttonContainer, false);
            var text = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI))
                .GetComponent<TextMeshProUGUI>();
            text.transform.SetParent(newButton.transform, false);

            text.alignment = TextAlignmentOptions.CenterGeoAligned;
            text.color = Color.gray;

            var buttonImage = newButton.GetComponent<Image>();
            buttonImage.color = Color.white;
            buttonImage.sprite = Resources.Load<Sprite>("Icons/settingsbutton");
            return newButton;
        }

        private static void GetButton(Button button)
        {
            button.gameObject.SetActive(true);
        }

        private static void ReleaseButton(Button button)
        {
            button.onClick.RemoveAllListeners();
            button.gameObject.SetActive(false);
        }

        private static void DestroyButton(Button button)
        {
            Destroy(button.gameObject);
        }


        protected override void HideEffect()
        {
            base.HideEffect();
            foreach (var button in generatedButtons)
            {
                buttonPool.Release(button);
            }

            generatedButtons.Clear();
        }

        public async Task<int> DisplayActionAsync(string title, string content, Sprite image, params string[] options)
        {
            contentText.text = content;
            contentTextLayout.WrapText(80, contentText, titleText);
            titleText.text = title;
            displayImage.sprite = image;
            if (image == null)
            {
                displayImage.transform.parent.gameObject.SetActive(false);
            }

            for (var i = 0; i < options.Length; i++)
            {
                var index = i;
                var newButton = ButtonPool.Get();
                newButton.GetComponentInChildren<TextMeshProUGUI>().text = options[i];
                newButton.onClick.AddListener(() => OnButtonClick(index));
                generatedButtons.Push(newButton);
            }

            Show(true);
            result = -1;
            while (result == -1)
            {
                await Task.Yield();
            }

            Show(false);
            return result;
        }

        [SerializeField] private bool wrapNavigation = true;
        [SerializeField] private NavigationDirection navigationDirection = NavigationDirection.Horizontal;

        protected override void SelectObject()
        {
            if (EventSystem.current == null || generatedButtons.Count == 0)
            {
                EventSystem.current?.SetSelectedGameObject(null);
                return;
            }

            var buttonsSpan = generatedButtons.ToArray().ConvertToSelectableArray();
            var firstSelectable =
                NavigationUtility.SetupNavigation(buttonsSpan, navigationDirection, wrapNavigation);
            EventSystem.current.SetSelectedGameObject(firstSelectable.gameObject);
        }
        private void OnButtonClick(int index)
        {
            result = index;
        }
    }
}