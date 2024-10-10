using UnityEngine;

namespace KabreetGames.Utilities
{
    /// <summary>
    /// This Script is the generic Singleton Base to use on the managers 
    /// </summary>
    [DefaultExecutionOrder(-10)]
    public abstract class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T instance;
        protected static bool instantiateIfNull;
        private static bool errorShowed;
        public static bool Quitting { get; set; }

        // ReSharper disable once StaticMemberInGenericType
        private static readonly object Lock = new();
        
        public static T Instance
        {
            get
            {
                if (Quitting)
                {
                    return null;
                }

                lock (Lock)
                {
                    if (instance != null) return instance;
                    instance = FindObjectOfType<T>();
                    if (instance != null) return instance;
                    if (instantiateIfNull)
                    {
                        var obj = new GameObject
                        {
                            name = typeof(T).Name
                        };
                        instance = obj.AddComponent<T>();
                    }
                    else
                    {
                        if (errorShowed) return instance;
                        Debug.LogWarning($"You called a {typeof(T).Name} instance and it is null");
                        errorShowed = true;
                    }
                    return instance;
                }
            }
        }

        private void Awake()
        {
            Quitting = false;
            lock (Lock)
            {
                if (instance == null)
                {
                    instance = this as T;
                }
                else
                {
                    if (instance != this) Destroy(gameObject);
                }
            }

            OnAwake();
        }
        protected virtual void OnAwake() { }


        protected virtual void OnApplicationQuit()
        {
            Quitting = true;
        }
    }
}