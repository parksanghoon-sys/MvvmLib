using Newtonsoft.Json;
using System.Collections.Generic;

namespace CoreMvvmLib.Design.Images
{
    internal class ImageRoot
    {
        [JsonProperty("images")]
        public List<ImageItem> Items { get; set; }
    }
}
