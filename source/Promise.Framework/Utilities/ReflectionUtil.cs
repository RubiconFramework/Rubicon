using System;
using System.Collections.Generic;
using System.Reflection;

namespace Promise.Framework.Utilities
{
    /// <summary>
    /// A utility that helps with Reflection things. http://answers.unity.com/answers/983130/view.html
    /// </summary>
    public static class ReflectionUtil
    {
        /// <summary>
        /// Gets all types that derive from the specified type.
        /// </summary>
        /// <param name="aAppDomain">The app domain</param>
        /// <param name="aType">The type to look for</param>
        /// <returns>All types that derive from the specified type.</returns>
        public static Type[] GetAllDerivedTypes(this AppDomain aAppDomain, Type aType)
        {
            List<Type> result = new List<Type>();
            Assembly[] assemblies = aAppDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                Type[] types = assemblies[i].GetTypes();
                for (int j = 0; j < types.Length; j++)
                {
                    if (types[j].IsSubclassOf(aType))
                        result.Add(types[j]);
                }
            }
            return result.ToArray();
        }

        /// <summary>
        /// Gets all types that derive from type T.
        /// </summary>
        /// <param name="aAppDomain">The app domain</param>
        /// <typeparam name="T">The type to look for</typeparam>
        /// <returns>All types that derive from type T.</returns>
        public static Type[] GetAllDerivedTypes<T>(this AppDomain aAppDomain)
        {
            return GetAllDerivedTypes(aAppDomain, typeof(T));
        }

        /// <summary>
        /// Gets all types that implements the interface provided.
        /// </summary>
        /// <param name="aAppDomain">The app domain</param>
        /// <param name="aInterfaceType">The interface to match with</param>
        /// <returns>All types that implement the interface type specified.</returns>
        public static Type[] GetTypesWithInterface(this AppDomain aAppDomain, Type aInterfaceType)
        {
            List<Type> result = new List<Type>();
            Assembly[] assemblies = aAppDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                Type[] types = assemblies[i].GetTypes();
                for (int j = 0; j < types.Length; j++)
                {
                    if (aInterfaceType.IsAssignableFrom(types[j]) && aInterfaceType != types[j])
                        result.Add(types[j]);
                }
            }
            return result.ToArray();
        }

        /// <summary>
        /// Gets all types that implements interface T.
        /// </summary>
        /// <param name="aAppDomain">The app domain</param>
        /// <typeparam name="T">The interface to match with</typeparam>
        /// <returns>All types that implement interface type T.</returns>
        public static Type[] GetTypesWithInterface<T>(this AppDomain aAppDomain)
        {
            return GetTypesWithInterface(aAppDomain, typeof(T));
        }
    }
}