using DG.Tweening;
using KabreetGames.SceneReferences;
using KabreetGames.UiSystem;
using KendaAi.TestProject.PuzzleSystem.NumberPuzzle;
using KendaAi.TestProject.PuzzleSystem.NumberPuzzle.Data;
using UnityEngine;
using UnityEngine.UI;

namespace KendaAi.TestProject.PuzzleSystem
{
    public class PuzzleMenuPanel : MenuPanel
    {
        [SerializeField,Child(Flag.Editable)] private Button doneButton;
        [SerializeField,Child(Flag.Editable)] private OperationView operationView;
        [SerializeField,Child(Flag.Editable)] private NumberInput numberInput;
        [SerializeField,Child(Flag.Editable)] private NumberInput numberInput2;
        [SerializeField,Child(Flag.Editable)] private NumberInput resultInput;

        private Equation equation;

        private Puzzle puzzle;
        protected override void OnEnable()
        {
            base.OnEnable();
            if (doneButton == null) Debug.Log("doneButton is null");
            doneButton.onClick.AddListener(OnDone);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            doneButton.onClick.RemoveListener(OnDone);
        }

        protected override void ShowEffect()
        {
            Group.DOFade(1, 1.5f).SetEase(Ease.OutExpo);
        }

        protected override void HideEffect()
        {
            Group.DOFade(0, 0.1f).SetEase(Ease.OutExpo);
        }

        private void OnDone()
        {
            if (!equation.Solved) return;
            puzzle.SolvePuzzle();
        }

        public void SetupUi(Puzzle pz)
        {
            puzzle = pz;
            equation = new Equation();
            equation.SetNumberA(numberInput);
            equation.SetNumberB(numberInput2);
            equation.SetResult(resultInput);
            operationView.SetOperation(equation.Operation);
        }
    }
}