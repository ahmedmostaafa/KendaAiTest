using System;
using System.Collections.Generic;

namespace KabreetGames.EventBus.Interfaces
{
    public static class PreDefinedAssemblyUtil
    {
        /// <summary>
        /// Method looks through a given assembly and adds types that fulfill a certain interface to the provided collection.
        /// </summary>
        /// <param name="assemblyTypes">Array of Type objects representing all the types in the assembly.</param>
        /// <param name="interfaceType">Type representing the interface to be checked against.</param>
        /// <param name="results">Collection of types where result should be added.</param>
        private static void AddTypesFromAssembly(Type[] assemblyTypes, Type interfaceType, ICollection<Type> results)
        {
            if (assemblyTypes == null) return;
            foreach (var type in assemblyTypes)
            {
                if (type != interfaceType && interfaceType.IsAssignableFrom(type))
                {
                    results.Add(type);
                }
            }
        }
        
        /// <summary>
        /// Gets all Types from all assemblies in the current AppDomain that implement the provided interface type.
        /// </summary>
        /// <param name="interfaceType">Interface type to get all the Types for.</param>
        /// <returns>List of Types implementing the provided interface type.</returns>    
        public static List<Type> GetTypes(Type interfaceType)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        
            var assemblyTypes = new Dictionary<string, Type[]>();
            var types = new List<Type>();
            foreach (var t in assemblies)
            {
                var assemblyType = t.GetName().Name;
                if (!assemblyType.Contains("KabreetGames")) continue;
                assemblyTypes.Add(assemblyType, t.GetTypes());
            }
        
            foreach (var keyValuePair in assemblyTypes)
            {
                AddTypesFromAssembly(keyValuePair.Value, interfaceType, types);
            }
            return types;
        }
    }
}