using Newtonsoft.Json;

namespace CoreMvvmLib.Design.Images
{
    internal class ImageItem
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("data")]
        public string Data { get; set; }
    }
}
