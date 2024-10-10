using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Eflatun.SceneReference;

namespace KabreetGames.SceneManagement
{
    [Serializable]
    public class SceneGroup
    {
        [TableColumnWidth(60)]
        public string groupName = "New Group";
        [TableList, LabelText("Scenes")]
        public List<SceneData> scenes = new();

        public string GetSceneByName(SceneType sceneType)
        {
            return scenes.Find(x => x.sceneType == sceneType)?.Name; 
        }
    }
    
    [Serializable]
    public class SceneData
    {
        public SceneType sceneType;
        public string Name => reference.Name;
        public SceneReference reference;
    }
    public enum SceneType
    {
        ActiveScene,
        MainMenuScene,
        UserInterfaceScene,
        HUD,
        Cinematic,
        Environment,
        Debug,
    }
    
    public enum ReplaceSceneMode
    {
        LoadUnload,
        UnLoadLoad
    }
}