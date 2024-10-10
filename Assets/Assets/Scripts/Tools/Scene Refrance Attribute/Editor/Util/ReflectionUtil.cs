using System;
using System.Collections.Generic;
using System.Reflection;

namespace KabreetGames.SceneReferences.Editor
{
    public static class ReflectionUtil
    {
        public struct AttributedField<T>
            where T : Attribute
        {
            public T attribute;
            public FieldInfo fieldInfo;
        }

        public static void GetFieldsWithAttributeFromType<T>(
            Type classToInspect,
            IList<AttributedField<T>> output,
            BindingFlags reflectionFlags = BindingFlags.Default
        )
            where T : Attribute
        {
            var type = typeof(T);
            do
            {
                var allFields = classToInspect.GetFields(reflectionFlags);
                foreach (var fieldInfo in allFields)
                {
                    ExtractAttributes(output, fieldInfo, type);
                }

                classToInspect = classToInspect.BaseType;
            } while (classToInspect != null);
        }

        private static void ExtractAttributes<T>(IList<AttributedField<T>> output, FieldInfo fieldInfo, Type type)
            where T : Attribute
        {
            var attributes = Attribute.GetCustomAttributes(fieldInfo);
            foreach (var attribute in attributes)
            {
                if (!type.IsInstanceOfType(attribute))
                    continue;

                output.Add(new AttributedField<T>
                {
                    attribute = attribute as T,
                    fieldInfo = fieldInfo
                });
                break;
            }
        }
    }
}