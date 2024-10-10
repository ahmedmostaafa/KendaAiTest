using UnityEngine;

namespace KabreetGames.TimeSystem
{
    public class CountdownTimer : Timer
    {
        public CountdownTimer(float value, bool unScaledTime = false) : base(value, unScaledTime)
        {
        }

        public override void Tick() => Tick(unScaledTime ? UnityEngine.Time.unscaledDeltaTime : UnityEngine.Time.deltaTime);

        public override void Tick(float deltaTime)
        {
            if (IsRunning && Time > 0)
            {
                Time -= deltaTime;
            }

            if (IsRunning && Time <= 0)
            {
                Stop();
            }
        }

        public override bool IsFinished => Time <= 0;
    }
}