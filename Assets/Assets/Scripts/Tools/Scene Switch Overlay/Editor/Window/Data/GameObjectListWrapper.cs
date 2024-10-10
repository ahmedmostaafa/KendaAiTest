using System.Collections.Generic;
using UnityEngine;

namespace KabreetGames.Editor
{
    public class GameObjectListWrapper : ScriptableObject
    {
        public List<GameObject> selectedPrefabs = new();
    }
}