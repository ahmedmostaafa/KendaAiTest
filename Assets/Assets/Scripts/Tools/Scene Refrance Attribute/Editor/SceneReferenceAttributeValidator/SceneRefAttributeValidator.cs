using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Collections;
using UnityEditor;

namespace KabreetGames.SceneReferences.Editor
{
    public static class SceneRefAttributeValidator
    {
        private static readonly IList<ReflectionUtil.AttributedField<SceneReferenceAttribute>> AttributedFieldsCache =
            new List<ReflectionUtil.AttributedField<SceneReferenceAttribute>>();


        /// <summary>
        /// Validate all references for every script and every game object in the scene.
        /// </summary>
        [MenuItem("Kabreet Games/Validate All Refs")]
        private static void ValidateAllRefs()
        {
            var scripts = MonoImporter.GetAllRuntimeMonoScripts();
            foreach (var runtimeMonoScript in scripts)
            {
                var scriptType = runtimeMonoScript.GetClass();
                if (scriptType == null)
                {
                    continue;
                }

                try
                {
                    ReflectionUtil.GetFieldsWithAttributeFromType(
                        scriptType,
                        AttributedFieldsCache,
                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance
                    );
                    if (AttributedFieldsCache.Count == 0)
                    {
                        continue;
                    }

#if UNITY_2020_OR_NEWER
                    Object[] objects = Object.FindObjectsOfType(scriptType, true);
#else
                    var objects = Object.FindObjectsOfType(scriptType);
#endif

                    if (objects.Length == 0)
                    {
                        continue;
                    }

                    Debug.Log(
                        $"Validating {AttributedFieldsCache.Count} field(s) on {objects.Length} {objects[0].GetType().Name} instance(s)");
                    foreach (var t in objects)
                    {
                        Validate(t as MonoBehaviour, AttributedFieldsCache, false);
                    }
                }
                finally
                {
                    AttributedFieldsCache.Clear();
                }
            }
        }

        /// <summary>
        /// Validate a single components references, attempting to assign missing references
        /// and logging errors as necessary.
        /// </summary>
        [MenuItem("CONTEXT/Component/Validate References")]
        private static void ValidateRefs(MenuCommand menuCommand)
            => Validate(menuCommand.context as Component);

        /// <summary>
        /// Clean and validate a single components references. Useful in instances where (for example) Unity has
        /// incorrectly serialized a scene reference within a prefab.
        /// </summary>
        [MenuItem("CONTEXT/Component/Clean and Validate References")]
        private static void CleanValidateRefs(MenuCommand menuCommand)
            => CleanValidate(menuCommand.context as Component);


        /// <summary>
        /// Validate a single components references, attempting to assign missing references
        /// and logging errors as necessary.
        /// </summary>
        public static void ValidateRefs(this Component c, bool updateAtRuntime = false)
            => Validate(c, updateAtRuntime);

        /// <summary>
        /// Validate a single components references, attempting to assign missing references
        /// and logging errors as necessary.
        /// </summary>
        private static void Validate(Component component, bool updateAtRuntime = false)
        {
            try
            {
                ReflectionUtil.GetFieldsWithAttributeFromType(
                    component.GetType(),
                    AttributedFieldsCache,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance
                );
                Validate(component, AttributedFieldsCache, updateAtRuntime);
            }
            finally
            {
                AttributedFieldsCache.Clear();
            }
        }

        /// <summary>
        /// Clean and validate a single components references. Useful in instances where (for example) Unity has
        /// incorrectly serialized a scene reference within a prefab.
        /// </summary>
        private static void CleanValidate(Component c, bool updateAtRuntime = false)
        {
            try
            {
                ReflectionUtil.GetFieldsWithAttributeFromType(
                    c.GetType(),
                    AttributedFieldsCache,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance
                );
                Clean(c, AttributedFieldsCache);
                Validate(c, AttributedFieldsCache, updateAtRuntime);
            }
            finally
            {
                AttributedFieldsCache.Clear();
            }
        }

        private static void Validate(
            Component component,
            ICollection<ReflectionUtil.AttributedField<SceneReferenceAttribute>> requiredFields,
            bool updateAtRuntime
        )
        {
            if (requiredFields.Count == 0)
            {
                return;
            }

            var isUninstantiatedPrefab = component.gameObject.scene.rootCount == 0;
            foreach (var attributedField in requiredFields)
            {
                var attribute = attributedField.attribute;
                var field = attributedField.fieldInfo;

                if (field.FieldType.IsInterface)
                {
                    throw new Exception(
                        $"{component.GetType().Name} cannot serialize interface {field.Name} directly, use InterfaceRef instead");
                }

                var fieldValue = field.GetValue(component);
                if (updateAtRuntime || !Application.isPlaying)
                {
                    fieldValue = UpdateRef(attribute, component, field, fieldValue);
                }

                if (isUninstantiatedPrefab) continue;

                ValidateRef(attribute, component, field, fieldValue);
            }
        }

        private static void Clean(
            Object c,
            IList<ReflectionUtil.AttributedField<SceneReferenceAttribute>> requiredFields
        )
        {
            foreach (var attributedField in requiredFields)
            {
                var attribute = attributedField.attribute;
                if (attribute.Location == ReferencesLocation.Anywhere)
                {
                    continue;
                }

                var field = attributedField.fieldInfo;
                field.SetValue(c, null);
                EditorUtility.SetDirty(c);
            }
        }

        private static object UpdateRef(
            SceneReferenceAttribute attr,
            Component component,
            FieldInfo field,
            object existingValue
        )
        {
            if (attr.HasFlags(Flag.Optional)) return existingValue;
            var fieldType = field.FieldType;
            var isCollection = IsCollectionType(fieldType, out var isList);
            var includeInactive = attr.HasFlags(Flag.IncludeInactive);
            ISerializableRef iSerializable = null;
            if (typeof(ISerializableRef).IsAssignableFrom(fieldType))
            {
                iSerializable = (ISerializableRef)(existingValue ?? Activator.CreateInstance(fieldType));
                fieldType = iSerializable.RefType;
                existingValue = iSerializable.SerializedObject;
            }

            if (attr.HasFlags(Flag.Editable))
            {
                var isFilledArray = isCollection && (existingValue as IEnumerable)!.Cast<object>().Any();
                if (isFilledArray || existingValue is Object)
                {
                    // If the field is editable and the value has already been set, keep it.
                    return existingValue;
                }
            }

            var elementType = fieldType;
            if (isCollection)
            {
                elementType = GetElementType(fieldType);
                if (typeof(ISerializableRef).IsAssignableFrom(elementType))
                {
                    var interfaceType = elementType?
                        .GetInterfaces()
                        .FirstOrDefault(type =>
                            type.IsGenericType &&
                            type.GetGenericTypeDefinition() == typeof(ISerializableRef<>));

                    if (interfaceType != null)
                    {
                        elementType = interfaceType.GetGenericArguments()[0];
                    }
                }
            }

            object value = null;

            switch (attr.Location)
            {
                case ReferencesLocation.Anywhere:
                    if (isCollection
                            ? typeof(ISerializableRef).IsAssignableFrom(fieldType.GetElementType())
                            : iSerializable != null)
                    {
                        value = isCollection
                            ? (existingValue as ISerializableRef[])?.Select(existingRef =>
                                GetComponentIfWrongType(existingRef.SerializedObject, elementType)).ToArray()
                            : GetComponentIfWrongType(existingValue, elementType);
                    }

                    break;

                case ReferencesLocation.Self:
                    value = isCollection
                        ? component.GetComponents(elementType)
                        : component.GetComponent(elementType);
                    break;

                case ReferencesLocation.Parent:
                    value = isCollection
                        ? component.GetComponentsInParentOnly(elementType, includeInactive)
                        : component.GetComponentInParentOnly(elementType, includeInactive);
                    break;

                case ReferencesLocation.Child:
                    value = isCollection
                        ? component.GetComponentsInChildOnly(elementType, includeInactive)
                        : component.GetComponentInChildOnly(elementType, includeInactive);
                    break;

                case ReferencesLocation.Scene:
                    value = isCollection
                        ? Object.FindObjectsOfType(elementType)
                        : Object.FindAnyObjectByType(elementType);
                    break;

                case ReferencesLocation.SelfOrChild:
                    value = isCollection
                        ? component.GetComponentsInChildren(elementType, includeInactive)
                        : component.GetComponentInChildren(elementType, includeInactive);
                    break;

                case ReferencesLocation.SelfOrParent:
                    value = isCollection
                        ? component.GetComponentsInParent(elementType, includeInactive)
                        : component.GetComponentInParent(elementType, includeInactive);
                    break;

                default:
                    throw new Exception($"Unhandled Loc={attr.Location}");
            }

            if (value == null)
            {
                return existingValue;
            }

            var filter = attr.Filter;

            if (isCollection)
            {
                var realElementType = GetElementType(fieldType);

                var componentArray = (Array)value;
                if (filter != null)
                {
                    // TODO: probably a better way to do this without allocating a list
                    IList<object> list = componentArray.Cast<object>().Where(o => filter.IncludeSceneRef(o)).ToList();
                    componentArray = list.ToArray();
                }

                var typedArray = Array.CreateInstance(
                    realElementType ?? throw new InvalidOperationException(),
                    componentArray.Length
                );

                if (elementType == realElementType)
                {
                    Array.Copy(componentArray, typedArray, typedArray.Length);
                    value = typedArray;
                }
                else if (typeof(ISerializableRef).IsAssignableFrom(realElementType))
                {
                    for (var i = 0; i < typedArray.Length; i++)
                    {
                        var elementValue = Activator.CreateInstance(realElementType) as ISerializableRef;
                        elementValue?.OnSerialize(componentArray.GetValue(i));
                        typedArray.SetValue(elementValue, i);
                    }

                    value = typedArray;
                }
            }
            else if (filter?.IncludeSceneRef(value) == false)
            {
                iSerializable?.Clear();
                if (existingValue != null)
                {
                    EditorUtility.SetDirty(component);
                }

                return null;
            }

            if (iSerializable == null)
            {
                var valueIsEqual = existingValue != null &&
                                   isCollection
                    ? ((IEnumerable<object>)value).SequenceEqual((IEnumerable<object>)existingValue)
                    : value.Equals(existingValue);
                if (valueIsEqual)
                {
                    return existingValue;
                }

                if (isList)
                {
                    var listType = typeof(List<>);
                    Type[] typeArgs = { fieldType.GenericTypeArguments[0] };
                    var constructedType = listType.MakeGenericType(typeArgs);

                    var newList = Activator.CreateInstance(constructedType);

                    var addMethod = newList.GetType().GetMethod(nameof(List<object>.Add));

                    foreach (var s in (IEnumerable)value)
                    {
                        if (addMethod != null) addMethod.Invoke(newList, new[] { s });
                    }

                    field.SetValue(component, newList);
                }
                else
                {
                    field.SetValue(component, value);
                }
            }
            else
            {
                if (!iSerializable.OnSerialize(value))
                {
                    return existingValue;
                }
            }

            EditorUtility.SetDirty(component);
            return value;
        }

        private static Type GetElementType(Type fieldType)
        {
            return fieldType.IsArray ? fieldType.GetElementType() : fieldType.GenericTypeArguments[0];
        }

        private static object GetComponentIfWrongType(object existingValue, Type elementType)
        {
            if (existingValue is Component existingComponent && existingComponent &&
                !elementType.IsInstanceOfType(existingValue))
            {
                return existingComponent.GetComponent(elementType);
            }

            return existingValue;
        }

        private static object GetComponentInChildOnly(this Component component, Type elementType, bool includeInactive)
        {
            var value = component.GetComponentsInChildren(elementType, includeInactive);
            value = value.Where(v => v.gameObject != component.gameObject).ToArray();
            return value.Length > 0 ? value[0] : null;
        }

        private static Component[] GetComponentsInChildOnly(this Component component, Type elementType,
            bool includeInactive)
        {
            var value = component.GetComponentsInChildren(elementType, includeInactive);
            return value.Where(v => v.gameObject != component.gameObject).ToArray();
        }

        private static object GetComponentInParentOnly(this Component component, Type elementType, bool includeInactive)
        {
            var value = component.GetComponentsInParent(elementType, includeInactive);
            value = value.Where(v => v.gameObject != component.gameObject).ToArray();
            return value.Length > 0 ? value[0] : null;
        }

        private static Component[] GetComponentsInParentOnly(this Component component, Type elementType,
            bool includeInactive)
        {
            var value = component.GetComponentsInParent(elementType, includeInactive);
            return value.Where(v => v.gameObject != component.gameObject).ToArray();
        }


        private static void ValidateRef(SceneReferenceAttribute attr, Component c, FieldInfo field, object value)
        {
            var fieldType = field.FieldType;
            var isCollection = IsCollectionType(fieldType, out _);

            if (value is ISerializableRef ser)
            {
                value = ser.SerializedObject;
            }

            if (IsEmptyOrNull(value, isCollection))
            {
                if (attr.HasFlags(Flag.Optional)) return;
                var elementType = isCollection ? fieldType.GetElementType() : fieldType;
                elementType = typeof(ISerializableRef).IsAssignableFrom(elementType)
                    ? elementType?.GetGenericArguments()[0]
                    : elementType;
                Debug.LogError(
                    $"{c.GetType().Name} missing required {elementType?.Name + (isCollection ? "[]" : "")} ref '{field.Name}'",
                    c.gameObject);
                return;
            }

            if (isCollection)
            {
                var a = (IEnumerable)value;
                var enumerator = a.GetEnumerator();
                using var unknown = enumerator as IDisposable;
                while (enumerator.MoveNext())
                {
                    var o = enumerator.Current;
                    if (o is ISerializableRef serObj)
                    {
                        o = serObj.SerializedObject;
                    }

                    if (o != null)
                    {
                        ValidateRefLocation(attr.Location, c, field, o);
                    }
                    else
                    {
                        Debug.LogError($"{c.GetType().Name} missing required element ref in array '{field.Name}'",
                            c.gameObject);
                    }
                }
            }
            else
            {
                ValidateRefLocation(attr.Location, c, field, value);
            }
        }

        private static void ValidateRefLocation(ReferencesLocation location, Component component, FieldInfo field,
            object refObj)
        {
            switch (refObj)
            {
                case Component valueC:
                    ValidateRefLocation(location, component, field, valueC);
                    break;

                case ScriptableObject obj:
                    ValidateRefLocationAnywhere(location, component, field, obj);
                    break;

                case GameObject obj:
                    ValidateRefLocationAnywhere(location, component, field, obj);
                    break;

                default:
                    throw new Exception(
                        $"{component.GetType().Name} has unexpected reference type {refObj?.GetType().Name}");
            }
        }

        private static void ValidateRefLocation(ReferencesLocation location, Component component, FieldInfo field,
            Component refObj)
        {
            switch (location)
            {
                case ReferencesLocation.Anywhere:
                    break;

                case ReferencesLocation.Self:
                    if (refObj.gameObject != component.gameObject)
                        Debug.LogError(
                            $"{component.GetType().Name} requires {field.FieldType.Name} ref '{field.Name}' to be on Self",
                            component.gameObject);
                    break;

                case ReferencesLocation.Parent:
                    if (!component.transform.IsChildOf(refObj.transform) && refObj.gameObject != component.gameObject)
                        Debug.LogError(
                            $"{component.GetType().Name} requires {field.FieldType.Name} ref '{field.Name}' to be a Parent",
                            component.gameObject);
                    break;

                case ReferencesLocation.Child:
                    if (!refObj.transform.IsChildOf(component.transform) && refObj.gameObject != component.gameObject)
                        Debug.LogError(
                            $"{component.GetType().Name} requires {field.FieldType.Name} ref '{field.Name}' to be a Child",
                            component.gameObject);
                    break;

                case ReferencesLocation.Scene:
                    if (component == null)
                        Debug.LogError(
                            $"{component.GetType().Name} requires {field.FieldType.Name} ref '{field.Name}' to be in the scene",
                            component.gameObject);
                    break;

                case ReferencesLocation.SelfOrChild:
                    if (!refObj.transform.IsChildOf(component.transform))
                        Debug.LogError(
                            $"{component.GetType().Name} requires {field.FieldType.Name} ref '{field.Name}' to be a Child",
                            component.gameObject);
                    break;

                case ReferencesLocation.SelfOrParent:
                    if (!component.transform.IsChildOf(refObj.transform) && refObj.gameObject != component.gameObject)
                        Debug.LogError(
                            $"{component.GetType().Name} requires {field.FieldType.Name} ref '{field.Name}' to be a Parent",
                            component.gameObject);
                    break;
                default:
                    throw new Exception($"Unhandled Loc={location}");
            }
        }

        private static void ValidateRefLocationAnywhere(ReferencesLocation location, Component component,
            FieldInfo field, Object refObj)
        {
            switch (location)
            {
                case ReferencesLocation.Anywhere:
                    break;

                case ReferencesLocation.Self:
                case ReferencesLocation.Parent:
                case ReferencesLocation.Child:
                case ReferencesLocation.SelfOrChild:
                case ReferencesLocation.Scene:
                    break;

                default:
                    throw new Exception($"Unhandled Loc={location}");
            }
        }

        private static bool IsEmptyOrNull(object obj, bool isCollection)
        {
            if (obj is ISerializableRef ser)
            {
                return !ser.HasSerializedObject;
            }

            return obj == null || obj.Equals(null) || (isCollection && !((IEnumerable)obj).Cast<object>().Any());
        }

        private static bool IsCollectionType(Type t, out bool isList)
        {
            isList = t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>);
            return isList || t.IsArray;
        }
    }
}