using UnityEngine;

namespace KendaAi.TestProject.PuzzleSystem.NumberPuzzle.Data
{
    public class Equation
    {
        private NumberInput numberA;
        private NumberInput numberB;
        private NumberInput result;
        public Operation Operation { get; } = (Operation)Random.Range(0, 3);

        public bool Solved
        {
            get
            {
                return Operation switch
                {
                    Operation.Add => numberA.Number + numberB.Number == result.Number,
                    Operation.Subtract => numberA.Number - numberB.Number == result.Number,
                    Operation.Multiply => numberA.Number * numberB.Number == result.Number,
                    Operation.Divide => numberB.Number == 0 || numberA.Number / numberB.Number == result.Number,
                    _ => false
                };
            }
        }

        public void SetNumberA(NumberInput a)
        {
            numberA = a;
        }

        public void SetNumberB(NumberInput b)
        {
            numberB = b;
        }

        public void SetResult(NumberInput r)
        {
            result = r;
        }
    }

    public enum Operation
    {
        Add,
        Subtract,
        Multiply,
        Divide,
    }
}