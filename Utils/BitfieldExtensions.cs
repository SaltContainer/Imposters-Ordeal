using System.Linq;

namespace ImpostersOrdeal.Utils
{
    public static class BitfieldExtensions
    {
        /// <summary>
        /// Gets an array of bools by interpreting this uint as a bitfield.
        /// </summary>
        public static bool[] GetBitArray(this uint self)
        {
            return Enumerable.Range(0, sizeof(uint) * 8)
                .Select(i => (self & (1 << i)) != 0)
                .ToArray();
        }

        /// <summary>
        /// Gets an array of bools by interpreting this int as a bitfield.
        /// </summary>
        public static bool[] GetBitArray(this int self)
        {
            return Enumerable.Range(0, sizeof(int) * 8)
                .Select(i => (self & (1 << i)) != 0)
                .ToArray();
        }

        /// <summary>
        /// Converts an array of bools to a uint bitfield.
        /// </summary>
        public static uint ConvertBitArrayToUint(this bool[] b)
        {
            uint ret = 0;
            for (int i = 0; i < sizeof(uint) * 8 && i < b.Length; i++)
                ret += (uint)(b[i] ? (1 << i) : 0);

            return ret;
        }

        /// <summary>
        /// Converts an array of bools to an int bitfield.
        /// </summary>
        public static int ConvertBitArrayToInt(this bool[] b)
        {
            int ret = 0;
            for (int i = 0; i < sizeof(int) * 8 && i < b.Length; i++)
                ret += b[i] ? (1 << i) : 0;

            return ret;
        }
    }
}
