using UnityEngine;

namespace KabreetGames.Utilities
{
    public static class SystemLoader
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void LoadSystem()
        {
            var systemObject = Resources.Load("Systems");
            if(systemObject == null) return;
            Object.DontDestroyOnLoad(Object.Instantiate(systemObject));
            if (!Debug.isDebugBuild) return;
            var developmentSystemObject = Resources.Load("Developer System");
            if (developmentSystemObject != null)
                Object.DontDestroyOnLoad(Object.Instantiate(developmentSystemObject));

        }
    }
}