using System;
using System.Collections.Generic;
using UnityEngine;

namespace KabreetGames.TimeSystem
{
    public static class TimeManager
    {
        private static readonly HashSet<Timer> Timers = new();
        private static readonly Stack<CountdownTimer> TimersPool = new();
        private static readonly Stack<Sequence> SequencesPool = new();
        private static readonly HashSet<Timer> TimersToRemove = new();
        private static readonly HashSet<Timer> TimersToAdd = new();
        private static readonly int UnscaledTime = Shader.PropertyToID("_UnscaledTime");


        internal static void RegisterTimer(Timer timer)
        {
            TimersToAdd.Add(timer);
        }

        internal static void DeregisterTimer(Timer timer)
        {
            TimersToRemove.Add(timer);
        }

        internal static void UpdateTimeTic()
        {
            foreach (var timer in TimersToRemove)
            {
                Timers.Remove(timer);
            }

            foreach (var timer in TimersToAdd)
            {
                Timers.Add(timer);
            }

            Shader.SetGlobalFloat(UnscaledTime, Time.unscaledTime);
            TimersToRemove.Clear();
            TimersToAdd.Clear();
            foreach (var timer in Timers) timer.Tick();
        }

        internal static void ClearTimers()
        {
            Timers.Clear();
            TimersToRemove.Clear();
            TimersToAdd.Clear();
            TimersPool.Clear();
        }

        public static IReadOnlyCollection<Timer> GetActiveTimers() => Timers;

        public static IReadOnlyCollection<Timer> GetTimersPool() => TimersPool;

        public static IReadOnlyCollection<Timer> GetTimesToAdd() => TimersToAdd;

        public static IReadOnlyCollection<Timer> GetTimesToRemove() => TimersToRemove;

        public static IReadOnlyCollection<Sequence> GetSequencePool() => SequencesPool;

        internal static CountdownTimer GetTimerFromStack(float delay, bool unscaledTime)
        {
            if (TimersPool.TryPop(out var timer))
            {
                timer.Reset(delay,unscaledTime);
            }
            else
            {
                timer = new CountdownTimer(delay,unscaledTime);
            }

            timer.ClearActions();
            timer.OnTimerStop += () => TimersPool.Push(timer);
            return timer;
        }

        private static Sequence GetSequenceFromStack()
        {
            if (!SequencesPool.TryPop(out var sequence))
            {
                sequence = new Sequence();
            }

            return sequence;
        }


        public static CountdownTimer Delay(Action action, float delay, bool unscaledTime = false)
        {
            var timer = GetTimerFromStack(delay, unscaledTime);
            timer.OnTimerStop += action;
            timer.Start();
            return timer;
        }

        public static CountdownTimer Delay<T>(Action<T> action, T value, float delay, bool unscaledTime = false)
        {
            var timer = GetTimerFromStack(delay, unscaledTime);
            timer.OnTimerStop += () => action(value);
            timer.Start();
            return timer;
        }

        public static CountdownTimer Delay<T1, T2>(Action<T1, T2> action, T1 value1, T2 value2, float delay,
            bool unscaledTime = false)
        {
            var timer = GetTimerFromStack(delay, unscaledTime);
            timer.OnTimerStop += () => action(value1, value2);
            timer.Start();
            return timer;
        }

        public static CountdownTimer Delay<T1, T2, T3>(Action<T1, T2, T3> action, T1 value1, T2 value2, T3 value3,
            float delay, bool unscaledTime = false)
        {
            var timer = GetTimerFromStack(delay, unscaledTime);
            timer.OnTimerStop += () => action(value1, value2, value3);
            timer.Start();
            return timer;
        }

        public static CountdownTimer Delay<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 value1, T2 value2,
            T3 value3,
            T4 value4, float delay, bool unscaledTime = false)
        {
            var timer = GetTimerFromStack(delay, unscaledTime);
            timer.OnTimerStop += () => action(value1, value2, value3, value4);
            timer.Start();
            return timer;
        }


        public static Sequence Sequence()
        {
            return GetSequenceFromStack();
        }

        internal static void ReturnSequence(Sequence sequence)
        {
            SequencesPool.Push(sequence);
        }
    }
}