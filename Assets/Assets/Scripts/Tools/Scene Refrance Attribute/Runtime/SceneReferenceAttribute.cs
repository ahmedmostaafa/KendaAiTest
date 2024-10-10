using System;
using UnityEngine;

namespace KabreetGames.SceneReferences
{
    public enum ReferencesLocation
    {
        /// <summary>
        /// Anywhere will only validate the reference isn't null, but relies on you to 
        /// manually assign the reference yourself.
        /// </summary>
        Anywhere = -1,

        /// <summary>
        /// Self looks for the reference on the same game object as the attributed component
        /// using GetComponent(s)()
        /// </summary>
        Self = 0,

        /// <summary>
        /// Parent looks for the reference on the parent hierarchy of the attributed components game object
        /// using GetComponent(s)InParent()
        /// </summary>
        Parent = 1,

        /// <summary>
        /// Child looks for the reference on the child hierarchy of the attributed components game object
        /// using GetComponent(s)InChildren()
        /// </summary>
        Child = 2,

        /// <summary>
        /// Scene looks for the reference anywhere in the scene
        /// using GameObject.FindAnyObjectByType() and GameObject.FindObjectsOfType()
        /// </summary>
        Scene = 4,

        /// <summary>
        /// SelfOrChild looks for the reference on the same game object as the attributed component or on the child hierarchy
        /// using GetComponent(s)() then using GetComponent(s)InChildren()
        /// </summary>
        SelfOrChild = 8,
        
        /// <summary>
        ///  
        /// </summary>
        SelfOrParent = 16
    }

    [Flags]
    public enum Flag
    {
        /// <summary>
        /// Default behaviour.
        /// </summary>
        None = 0,

        /// <summary>
        /// Allow empty (or null in the case of non-array types) results.
        /// </summary>
        Optional = 1 << 0,

        /// <summary>
        /// Include inactive components in the results (only applies to Child and Parent). 
        /// </summary>
        IncludeInactive = 1 << 1,

        /// <summary>
        /// Allows the user to override the automatic selection. Will still validate that
        /// the field location (self, child, etc) matches as expected.
        /// </summary>
        Editable = 1 << 2,

        /// <summary>
        /// By Default, the attribute will hide in the inspector if it's not null flag it with this to show it even if it has a value.
        /// </summary>
        ShowInInspector = 1 << 3
    }


    [AttributeUsage(AttributeTargets.Field)]
    public class SceneReferenceAttribute : PropertyAttribute
    {
        public ReferencesLocation Location { get; }
        public Flag Flags { get; }

        public SceneRefFilter Filter
        {
            get
            {
                if (filterType == null)
                    return null;
                return (SceneRefFilter)Activator.CreateInstance(this.filterType);
            }
        }

        private readonly Type filterType;

        internal SceneReferenceAttribute(
            ReferencesLocation location,
            Flag flags,
            Type filter
        )
        {
            Location = location;
            Flags = flags;
            filterType = filter;
        }

        public bool HasFlags(Flag flags)
            => (Flags & flags) == flags;
    }

    /// <summary>
    /// Anywhere will only validate the reference isn't null, but relies on you to 
    /// manually assign the reference yourself.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class AnywhereAttribute : SceneReferenceAttribute
    {
        public AnywhereAttribute(Flag flags = Flag.None, Type filter = null)
            : base(ReferencesLocation.Anywhere, flags, filter)
        {
        }
    }

    /// <summary>
    /// Self looks for the reference on the same game object as the attributed component
    /// using GetComponent(s)()
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class SelfAttribute : SceneReferenceAttribute
    {
        public SelfAttribute(Flag flags = Flag.None, Type filter = null)
            : base(ReferencesLocation.Self, flags, filter)
        {
        }
    }

    /// <summary>
    /// Parent looks for the reference on the parent hierarchy of the attributed components game object
    /// using GetComponent(s)InParent()
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ParentAttribute : SceneReferenceAttribute
    {
        public ParentAttribute(Flag flags = Flag.None, Type filter = null)
            : base(ReferencesLocation.Parent, flags, filter)
        {
        }
    }

    /// <summary>
    /// Child looks for the reference on the child hierarchy of the attributed components game object
    /// using GetComponent(s)InChildren()
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ChildAttribute : SceneReferenceAttribute
    {
        public ChildAttribute(Flag flags = Flag.None, Type filter = null)
            : base(ReferencesLocation.Child, flags, filter)
        {
        }
    }

    /// <summary>
    /// Child looks for the reference on the child hierarchy of the attributed components game object
    /// using GetComponent(s)InChildren()
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class SelfOrChildAttribute : SceneReferenceAttribute
    {
        public SelfOrChildAttribute(Flag flags = Flag.None, Type filter = null)
            : base(ReferencesLocation.SelfOrChild, flags, filter)
        {
        }
    }  
    
    /// <summary>
    /// Child looks for the reference on the child hierarchy of the attributed components game object
    /// using GetComponent(s)InChildren()
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class SelfOrParentAttribute : SceneReferenceAttribute
    {
        public SelfOrParentAttribute(Flag flags = Flag.None, Type filter = null)
            : base(ReferencesLocation.SelfOrParent, flags, filter)
        {
        }
    }

    /// <summary>
    /// Scene looks for the reference anywhere in the scene
    /// using GameObject.FindAnyObjectByType() and GameObject.FindObjectsOfType()
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class SceneAttribute : SceneReferenceAttribute
    {
        public SceneAttribute(Flag flags = Flag.None, Type filter = null)
            : base(ReferencesLocation.Scene, flags, filter)
        {
        }
    }
}