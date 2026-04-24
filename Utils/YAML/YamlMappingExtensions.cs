using System.Collections.Generic;
using SharpYaml.Model;
using System.Linq;

namespace ImpostersOrdeal
{
    public static class YamlMappingExtensions
    {
        /// <summary>
        /// Returns the given field as a mapping of yaml elements.
        /// </summary>
        public static IDictionary<string, YamlElement> GetMapping(this YamlMapping self, string field)
        {
            return (self[field] as YamlMapping).ToDictionary(kvp => kvp.Key.ToString(), kvp => kvp.Value);
        }

        /// <summary>
        /// Returns the given field as a sequence of yaml mappings.
        /// </summary>
        public static IEnumerable<YamlMapping> GetSequenceOfMappings(this YamlMapping self, string field)
        {
            return (self[field] as YamlSequence).Cast<YamlMapping>();
        }

        /// <summary>
        /// Returns the given field as a sequence of yaml elements.
        /// </summary>
        public static IEnumerable<YamlElement> GetSequenceOfElements(this YamlMapping self, string field)
        {
            return self[field] as YamlSequence;
        }
    }
}
