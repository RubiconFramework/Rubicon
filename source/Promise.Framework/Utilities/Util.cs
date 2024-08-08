using System.Linq;
using Godot;

namespace Promise.Framework.Utilities
{
    /// <summary>
    /// General utility class for the engine.
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Gets the first child that is of type specified.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <param name="includeInternal"></param>
        /// <returns></returns>
        public static T GetChildOfType<T>(this Node node, bool includeInternal = false) where T : class
        {
            return node.GetChildren(includeInternal).FirstOrDefault(x => x is T) as T;
        }

        /// <summary>
        /// Gets all children that is of type specified.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <param name="includeInternal"></param>
        /// <returns></returns>
        public static T[] GetChildrenOfType<T>(this Node node, bool includeInternal = false) where T : class
        {
            Node[] nodesFound = node.GetChildren(includeInternal).Where(x => x is T).ToArray();
            T[] childrenFound = new T[nodesFound.Length];

            for (int i = 0; i < nodesFound.Length; i++)
                childrenFound[i] = nodesFound[i] as T;

            return childrenFound;
        }
    }
}