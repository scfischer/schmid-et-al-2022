using UnityEngine;

namespace Globals.Utilities
{
    /// <summary>
    /// This class contains static utility Methods for convenience or debugging purposes, which are not readily provided
    /// by Unity or C#.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Translate an array of float values provided as parameter into a human-readable string.
        /// </summary>
        /// <param name="data"> The array of floating point values to get a string representation of</param>
        /// <returns>A string of of the form [float1; float2; ...; floatLast]</returns>
        public static string FloatArrayToString(float[] data)
        {
            string output = "[";
            if (data.Length > 0)
            {
                output += data[0];
            }
            for (int i = 1; i < data.Length; i++)
            {
                output += "; " + data[i];
            }

            output += "]";

            return output;
        }

        /// <summary>
        /// Translate an array of integer values provided as parameter into a human-readable string.
        /// </summary>
        /// <param name="data"> The array of integer values to get a string representation of</param>
        /// <returns>A string of of the form [int1; int2; ...; intLast]</returns>
        public static string VectorArrayToString(Vector3[] data)
        {
            string output = "[";
            if (data.Length > 0)
            {
                output += data[0];
            }
            for (int i = 1; i < data.Length; i++)
            {
                output += "; " + data[i];
            }

            output += "]";

            return output;
        }
    }
}
