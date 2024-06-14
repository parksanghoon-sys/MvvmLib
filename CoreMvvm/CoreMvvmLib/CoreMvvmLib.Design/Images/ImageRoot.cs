﻿using Newtonsoft.Json;

namespace CoreMvvmLib.Design.Images
{
    internal class ImageRoot
    {
        [JsonProperty("images")]
        public List<ImageItem> Items { get; set; }
    }
}
