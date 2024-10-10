using System;

namespace KabreetGames.TimeSystem
{
    public class FrequencyTimer : Timer {
        public int TicksPerSecond { get; private set; }
        
        public readonly Action onTick = delegate { };

        private float timeThreshold;

        public FrequencyTimer(int ticksPerSecond) : base(0) {
            CalculateTimeThreshold(ticksPerSecond);
        }
        public override void Tick() => Tick(unScaledTime ? UnityEngine.Time.unscaledDeltaTime : UnityEngine.Time.deltaTime);

        public override void Tick(float deltaTime)
        {
            if (IsRunning && Time >= timeThreshold) {
                Time -= timeThreshold;
                onTick.Invoke();
            }

            if (IsRunning && Time < timeThreshold) {
                Time += deltaTime;
            }
        }

        public override bool IsFinished => !IsRunning;

        public override void Reset() {
            Time = 0;
        }
        
        public void Reset(int newTicksPerSecond) {
            CalculateTimeThreshold(newTicksPerSecond);
            Reset();
        }
        
        void CalculateTimeThreshold(int ticksPerSecond) {
            TicksPerSecond = ticksPerSecond;
            timeThreshold = 1f / TicksPerSecond;
        }
    }
}