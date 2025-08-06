using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace CoreMvvmLib.Design.Geometies
{
    internal class GeometryItem
    {
        [YamlMember(Alias = "name")]
        public string Name { get; set; } = string.Empty;
        [YamlMember(Alias = "data")]
        public string Data { get; set; } = string.Empty;
    }
}
