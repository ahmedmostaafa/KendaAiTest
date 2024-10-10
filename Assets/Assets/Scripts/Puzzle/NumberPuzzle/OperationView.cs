using KabreetGames.SceneReferences;
using KendaAi.TestProject.PuzzleSystem.NumberPuzzle.Data;
using TMPro;
using UnityEngine;

namespace KendaAi.TestProject.PuzzleSystem.NumberPuzzle
{
    public class OperationView : ValidatedMonoBehaviour
    {
        [SerializeField, Child] private TextMeshProUGUI text;
        public void SetOperation(Operation operation)
        {
            text.text = operation switch
            {
                Operation.Add => "+",
                Operation.Subtract => "-",
                Operation.Multiply => "x",
                Operation.Divide => "/",
                _ => ""
                
            };
        }
    }
}