using System.Linq;

namespace KendaAi.BlackboardSystem.Util
{
    public static  class StringExtensions
    {
        /// <summary>
        /// Computes the FNV-1a hash for the input string. 
        /// The FNV-1a hash is a non-cryptographic hash function known for its speed and good distribution properties.
        /// Useful for creating Dictionary keys instead of using strings.
        /// https://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function
        /// </summary>
        /// <param name="str">The input string to hash.</param>
        /// <returns>An integer representing the FNV-1a hash of the input string.</returns>
        public static int ComputeFnv1AHash(this string str) {
            var hash = str.Aggregate(2166136261, (current, c) => (current ^ c) * 16777619);
            return unchecked((int)hash);
        }
    }
}