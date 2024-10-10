using System;
using System.Collections.Generic;
using System.Linq;

namespace KabreetGames.EventBus.Interfaces
{
    public class EventBus<T> where T : IEvent, new()
    {
        private static readonly HashSet<IEventBinding<T>> Bindings = new();
        public static void Register(EventBinding<T> binding) => Bindings.Add(binding);
        public static void Register(Action<T> binding) => Bindings.Add(new EventBinding<T>(binding));
        public static void Register(Action binding) => Bindings.Add(new EventBinding<T>(binding));
        public static void Deregister(EventBinding<T> binding) => Bindings.Remove(binding);
        public static void Deregister(Action<T> binding) => Bindings.Remove(Bindings.FirstOrDefault(x => x.Event == binding));
        public static void Deregister(Action binding) => Bindings.Remove(Bindings.FirstOrDefault(x => x.EventNoArg == binding));
        
        public static void Rise(T @event)
        {
            var bindingsCopy = Bindings.ToArray();
            foreach (var binding in bindingsCopy)
            {
                binding.Event.Invoke(@event);
                binding.EventNoArg.Invoke();
            }
        }

        public static void Rise()
        {
            var bindingsCopy = Bindings.ToArray();
            foreach (var binding in bindingsCopy)
            {
                binding.Event.Invoke(new T());
                binding.EventNoArg.Invoke();
            }
        }

        private static void Clear()
        {
            Bindings.Clear();
        }
    }
}