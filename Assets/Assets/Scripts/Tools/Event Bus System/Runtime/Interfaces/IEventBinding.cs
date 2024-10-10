using System;
namespace KabreetGames.EventBus.Interfaces
{
    internal interface IEventBinding<T>
    {
        public Action<T> Event { get; set; }
        public Action EventNoArg { get; set; }
    }
    
    public class EventBinding<T> : IEventBinding<T> where T : IEvent
    {
        public Action<T> Event { get; set; } = _ => {};

        public Action EventNoArg { get; set; } = () => { };

        public EventBinding(Action<T> onAction) => Event = onAction;

        public EventBinding(Action onAction) => EventNoArg = onAction;

        public void Add(Action action) => EventNoArg += action;
        public void Remove(Action action) => EventNoArg -= action;  
        
        public void Add(Action<T> action) => Event += action;
        public void Remove(Action<T> action) => Event -= action;
    }
}