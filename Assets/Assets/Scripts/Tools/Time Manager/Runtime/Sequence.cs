using System;
using System.Collections.Generic;
using System.Linq;

namespace KabreetGames.TimeSystem
{
    public class Sequence
    {
        private readonly Queue<(float delay, Action action)> schedule = new();
        private CountdownTimer timer;
        private bool unScaledTime;
        internal Sequence(){}
        
        public Sequence Delay(float delay)
        {
            schedule.Enqueue((delay, null));
            return this;
        }

        public Sequence Call(Action action)
        {
            schedule.Enqueue((0f, action));
            return this;
        }

        public Sequence Call<T>(Action<T> action, T value)
        {
            return Call(() => action(value));
        }

        public Sequence Call<T1, T2>(Action<T1, T2> action, T1 value1, T2 value2)
        {
            return Call(() => action(value1, value2));
        }

        public Sequence Call<T1, T2, T3>(Action<T1, T2, T3> action, T1 value1, T2 value2, T3 value3)
        {
            return Call(() => action(value1, value2, value3));
        }

        public Sequence Call<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 value1, T2 value2, T3 value3, T4 value4)
        {
            return Call(() => action(value1, value2, value3, value4));
        }

        public void Start(bool unscaledTime = false)
        {
            if (schedule.Count == 0)
            {
                Dispose();
                return;
            }
            unScaledTime = unscaledTime;
            ProcessNext();
        }

        private void OnTimerComplete()
        {
            ProcessNext();
        }

        private void ProcessNext()
        {
            while (true)
            {
                if (schedule.Count == 0)
                {
                    Dispose();
                    return;
                }

                var (delay, action) = schedule.Dequeue();
                var allDelays = schedule.All(x => x.action == null);
                if (delay <= 0f)
                {
                    action?.Invoke();
                    continue;
                }
                if (allDelays)
                {
                    Dispose();
                    return;
                }

                timer = TimeManager.GetTimerFromStack(delay, unScaledTime);
                timer.OnTimerStop += OnTimerComplete;
                timer.Start();
                break;
            }
        }

        private void Dispose()
        {
            timer = null;
            schedule.Clear();
            TimeManager.ReturnSequence(this);
        }
    }
}