using KabreetGames.SceneReferences;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KendaAi.TestProject.PuzzleSystem.NumberPuzzle
{
    public class NumberInput : ValidatedMonoBehaviour
    {
        [SerializeField] private bool isLocked;
        [SerializeField] private Button increaseButton;
        [SerializeField] private Button decreaseButton;
        [SerializeField, Child] private TextMeshProUGUI text;
        [SerializeField, Self] private CanvasGroup group;
        public int Number { get; private set; }
        private void OnEnable()
        {
            increaseButton.onClick.AddListener(OnIncrease);
            decreaseButton.onClick.AddListener(OnDecrease);
            SetNumber(Random.Range(0, 10));
            group.interactable = !isLocked;
            group.alpha = isLocked ? 0.8f : 1f;
        }

        private void OnDisable()
        {
            increaseButton.onClick.RemoveListener(OnIncrease);
            decreaseButton.onClick.RemoveListener(OnDecrease);
        }


        private void SetNumber(int number)
        {
            Number = number;
            UpdateView();
        }

        private void OnIncrease()
        {
            Number++;
            if (Number > 9) Number = 0;
            UpdateView();
        }

        private void OnDecrease()
        {
            Number--;
            if (Number < 0) Number = 9;
            UpdateView();
        }

        private void UpdateView() => text.text = Number.ToString();
    }
}