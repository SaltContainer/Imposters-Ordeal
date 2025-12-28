using System.Globalization;

namespace ImpostersOrdeal.Utils
{
    public static class IntHelper
    {
        /// <summary>
        /// Converts an int32 to a string in a culture-invariant way.
        /// </summary>
        public static string ConvertToString(int n)
        {
            return n.ToString("{G}", CultureInfo.InvariantCulture);
        }
    }
}
