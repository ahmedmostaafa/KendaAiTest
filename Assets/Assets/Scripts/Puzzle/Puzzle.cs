using DG.Tweening;
using KabreetGames.SceneReferences;
using KendaAi.Agents.Planer;
using KendaAi.Inventory.Interface;
using KendaAi.TestProject.PlayerController.States;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;

namespace KendaAi.TestProject.PuzzleSystem
{
    public class Puzzle : ValidatedMonoBehaviour, IInteractable
    {
        [SerializeField, Child] private CinemachineCamera cinemachineCamera;
        [SerializeField, Child] private PuzzleMenuPanel puzzleMenuPanel;
        [SerializeField, Child(Flag.Editable)] private Renderer wall;
        private IAgent agent;

        private MaterialPropertyBlock wallMaterialPropertyBlock;
        private static readonly int Open = Shader.PropertyToID("_Open");

        private void Awake()
        {
            wallMaterialPropertyBlock = new MaterialPropertyBlock();
            wall.GetPropertyBlock(wallMaterialPropertyBlock);
            UpdateWallValue(0f);
            puzzleMenuPanel.SetupUi(this);
        }

        private void UpdateWallValue(float open)
        {
            wallMaterialPropertyBlock.SetFloat(Open, open);
            wall.SetPropertyBlock(wallMaterialPropertyBlock);
        }

        [Button]
        private void EnablePuzzle()
        {
            ShowCamera();
            puzzleMenuPanel.Show(true);
        }

        [Button]
        public void SolvePuzzle()
        {
            puzzleMenuPanel.Show(false);
            ResetCamera();
            DOVirtual.Float(0f, 1f, 1f, UpdateWallValue).SetEase(Ease.OutExpo).onComplete += () =>
            {
                agent.ChangePlane<MoveState>();
            };
        }

        private void ShowCamera()
        {
            cinemachineCamera.Priority.Value = 10;
        }

        private void ResetCamera()
        {
            cinemachineCamera.Priority.Value = 0;
        }

        public void Interact(IAgent interactor)
        {
            agent = interactor;
            EnablePuzzle();
            interactor.ChangePlane<SolvePuzzleState>();
        }

        public bool CanInteract(IAgent interactor)
        {
            return interactor.IsAlive;
        }
    }
}