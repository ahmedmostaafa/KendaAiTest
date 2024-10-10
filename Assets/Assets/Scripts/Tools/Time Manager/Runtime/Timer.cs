using System;
using UnityEngine;

namespace KabreetGames.TimeSystem
{
    public abstract class Timer : IDisposable
    {
        protected float initialTime;
        protected float Time { get; set; }
        public bool IsRunning { get; private set; }

        public float Progress => Mathf.Clamp01(Time / initialTime);

        public event Action OnTimerStart = delegate { };
        public event Action OnTimerStop = delegate { };


        protected bool unScaledTime;
        private bool disposed;

        protected Timer(float value , bool unScaledTime = false)
        {
            initialTime = value;
            IsRunning = false;
            this.unScaledTime = unScaledTime;
        }

        ~Timer()
        {
            Dispose(false);
        }

        public void ClearActions()
        {
            OnTimerStart = delegate { };
            OnTimerStop = delegate { };
        }

        public void Start(bool manualTick = false)
        {
            Time = initialTime;
            if (IsRunning) return;
            IsRunning = true;
            OnTimerStart.Invoke();
            if (!manualTick) TimeManager.RegisterTimer(this);
        }

        public void Stop()
        {
            if (!IsRunning) return;
            IsRunning = false;
            OnTimerStop.Invoke();
            TimeManager.DeregisterTimer(this);
        }     

        public void Resume() => IsRunning = true;
        public void Pause() => IsRunning = false;

        public abstract void Tick();
        public abstract void Tick(float deltaTime);

        public abstract bool IsFinished { get; }
        public virtual void Reset() => Time = initialTime;

        public virtual void Reset(float newTime, bool unscaledTime = false)
        {
            initialTime = newTime;
            unScaledTime = unscaledTime;
            Reset();
        }

        public float GetTime() => Time;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;
            disposed = true;
            if (disposing)
            {
                TimeManager.DeregisterTimer(this);
            }
        }
    }
}