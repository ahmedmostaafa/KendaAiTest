using System;
using UnityEngine;

namespace KabreetGames.UiSystem
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class BindingDirectionAttribute : PropertyAttribute
    {
        public readonly string actionName;
        public BindingDirectionAttribute(string actionName)
        {
            this.actionName = actionName;
        }
    }
}