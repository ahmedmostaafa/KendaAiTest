using System;
using UnityEngine;

namespace KabreetGames.UiSystem
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class BindingIdAttribute : PropertyAttribute
    {
        public readonly string actionName;
        
        public BindingIdAttribute(string actionName)
        {
            this.actionName = actionName;
        }
    }
}