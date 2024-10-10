using System;
using UnityEngine;

namespace KabreetGames.SceneManagement
{
    public class LoadingProgress : IProgress<float>
    {
        public event Action<float> OnProgressChanged = delegate { };
        public event Action OnProgressDone = delegate { };

        private const float MaxProgress = 1f;
        public void Report(float value)
        {
            OnProgressChanged?.Invoke(value / MaxProgress);
            if(Mathf.Approximately(value, 1f)) OnProgressDone?.Invoke();
        }
    }
}