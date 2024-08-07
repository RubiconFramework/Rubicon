using System.Linq;
using Godot;

namespace Konkon
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
        
        /// <summary>
        /// Converts a measure to milliseconds based on the number of beats in a measure.
        /// </summary>
        /// <param name="measure">The measure</param>
        /// <param name="bpm">The BPM</param>
        /// <param name="timeSignatureNumerator">The number of beats in a measure.</param>
        /// <returns>The measure, in milliseconds.</returns>
        public static double MeasureToMs(double measure, double bpm, double timeSignatureNumerator = 4f)
        {
            return MeasureToBeats(measure, timeSignatureNumerator) * 60000f / bpm;
        }

        /// <summary>
        /// Converts measures to beats.
        /// </summary>
        /// <param name="measure">The measure.</param>
        /// <param name="timeSignatureNumerator">The number of beats in a measure.</param>
        /// <returns>The measure, in beats.</returns>
        public static double MeasureToBeats(double measure, double timeSignatureNumerator = 4f)
        {
            return measure * timeSignatureNumerator;
        }

        /// <summary>
        /// Converts measures to steps.
        /// </summary>
        /// <param name="measure">The measure</param>
        /// <param name="timeSignatureNumerator">The number of beats in a measure.</param>
        /// <param name="timeSignatureDenominator">The type of note which equals one beat.</param>
        /// <returns>The measure, in steps.</returns>
        public static double MeasureToSteps(double measure, double timeSignatureNumerator = 4f, double timeSignatureDenominator = 16f)
        {
            return measure * timeSignatureNumerator * (timeSignatureNumerator * timeSignatureDenominator);
        }

        /// <summary>
        /// Converts beats to steps.
        /// </summary>
        /// <param name="beats">The beats</param>
        /// <param name="timeSignatureDenominator">The type of note which equals one beat.</param>
        /// <returns>The beats, in steps.</returns>
        public static double BeatsToSteps(double beats, double timeSignatureDenominator = 4f)
        {
            return beats * timeSignatureDenominator;
        }

        /// <summary>
        /// Converts steps to measures.
        /// </summary>
        /// <param name="steps">The steps.</param>
        /// <param name="timeSignatureNumerator">The number of beats in a measure.</param>
        /// <param name="timeSignatureDenominator">The type of note which equals one beat.</param>
        /// <returns>The steps, in measures.</returns>
        public static double StepsToMeasures(double steps, double timeSignatureNumerator = 4f, double timeSignatureDenominator = 16f)
        {
            return steps / (timeSignatureNumerator * timeSignatureDenominator);
        }
    }
}