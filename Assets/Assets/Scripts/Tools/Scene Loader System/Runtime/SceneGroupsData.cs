using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KabreetGames.SceneManagement
{
    public class SceneGroupsData : ScriptableObject
    {
        [TableList(ShowIndexLabels = true), ListDrawerSettings(ShowFoldout = true)]
        public SceneGroup[] sceneGroups;

        public SceneGroup GetGroupByName(SceneGroupNames sceneGroupName)
        {
            return sceneGroups
                .FirstOrDefault(x => Name(x.groupName).Equals(sceneGroupName.ToString()));
        }

        private static string Name(string groupName)
        {
            return groupName.Replace(" ", "").Replace("-", "");
        }
    }
}