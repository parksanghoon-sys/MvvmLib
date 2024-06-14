using Newtonsoft.Json;

namespace CoreMvvmLib.Design.Geometies
{
    internal class GeometryItem
    {
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
        [JsonProperty("data")]
        public string Data { get; set; } = string.Empty;
    }
}
