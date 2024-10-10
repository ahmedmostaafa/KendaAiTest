namespace KabreetGames.TimeSystem
{
    public class StopwatchTimer : Timer
    {
        public StopwatchTimer() : base(0)
        {
        }

        public override void Tick() =>
            Tick(unScaledTime ? UnityEngine.Time.unscaledDeltaTime : UnityEngine.Time.deltaTime);

        public override void Tick(float deltaTime)
        {
            if (IsRunning)
            {
                Time += UnityEngine.Time.deltaTime;
            }
        }

        public override bool IsFinished => Time <= 0;

        public override void Reset() => Time = 0;
    }
}