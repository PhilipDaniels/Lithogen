using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace Lithogen.Engine.Configuration
{
    /// <summary>
    /// Class is used to simplify Yaml deserialization of _config.lit.
    /// </summary>
    public class YamlMapping
    {
        public List<string> Extensions { get; set; }
        public List<string> Processors { get; set; }
        public bool? Publish { get; set; }
        public string Layout { get; set; }
        public string Model { get; set; }
        [YamlMember(Alias="extout")]
        public string ExtOut { get; set; }
    }

    /// <summary>
    /// Class is used to simplify Yaml deserialization of _config.lit.
    /// </summary>
    public class YamlMappings
    {
        public List<YamlMapping> Mappings { get; set; }
    }
}
