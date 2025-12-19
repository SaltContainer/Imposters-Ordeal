using System;

namespace ImpostersOrdeal
{
    public static class FloatHelper
    {
        /// <summary>
        ///  Interprets bytes of an int32 as a float.
        /// </summary>
        public static float ConvertToFloat(int n)
        {
            return BitConverter.ToSingle(BitConverter.GetBytes(n));
        }

        /// <summary>
        ///  Interprets bytes of a float as an int32.
        /// </summary>
        public static int ConvertToInt(float n)
        {
            return BitConverter.ToInt32(BitConverter.GetBytes(n));
        }
    }
}
