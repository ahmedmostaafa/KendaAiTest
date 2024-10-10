using KabreetGames.EventBus.Interfaces;
using UnityEngine;

namespace KendaAi.Events
{
    public struct HitEvent : IEvent { }
    public struct DeathEvent : IEvent { }
    public struct PuzzleSolvedEvent : IEvent { }
    public struct PuzzleStartEvent : IEvent { }

    public struct CoinCollectedEvent : IEvent
    {
        public readonly Sprite sprite;
        public readonly Vector3 position;

        public CoinCollectedEvent(Sprite sprite, Vector3 position)
        {
            this.sprite = sprite;
            this.position = position;
        }
    }
}