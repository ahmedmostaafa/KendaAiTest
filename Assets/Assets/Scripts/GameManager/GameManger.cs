using KabreetGames.Utilities;
using KendaAi.Inventory;
using KendaAi.Scripts.Enemy;
using KendaAi.Scripts.Interaction;
using KendaAi.TestProject.PuzzleSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KendaAi.GameManager
{
    public class GameManger : Singleton<GameManger>
    {
        [SerializeField, Required] private Crab crapPrefab;
        [SerializeField, Required] private DamageObject holePrefab;

        [SerializeField, Required] private GameObject vinePrefab;

        [SerializeField, Required] private Puzzle puzzlePrefab;

        [SerializeField, Required] private WinDoor doorPrefab;

        [SerializeField, Required] private Jem jemPrefab;
        
        [SerializeField, Required] private Transform touchPointPrefab;

        [SerializeField, RequiredIn(PrefabKind.InstanceInScene)]
        private Transform[] walls;

        [SerializeField, RequiredIn(PrefabKind.InstanceInScene)]
        private Transform[] cells;


        [Button]
        private void GenerateWorld()
        {
            var worldLength = Random.Range(150, 1000);
            SetUpWorld(worldLength);
        }

        private void SetUpWorld(int worldLength)
        {
            foreach (var wall in walls)
            {
                wall.localScale = new Vector3(worldLength, wall.localScale.y, wall.localScale.z);
                wall.position = new Vector3(wall.position.x, wall.position.y, worldLength / 2f - 30);
            }

            foreach (var cell in cells)
            {
                cell.localScale = new Vector3(cell.localScale.x, worldLength, cell.localScale.z);
                cell.position = new Vector3(cell.position.x, cell.position.y, worldLength / 2f - 30);
            }
        }
    }
}