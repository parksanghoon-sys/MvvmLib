using System.Collections.Generic;
using System.Text.Json.Serialization;
using YamlDotNet.Serialization;

namespace CoreMvvmLib.Design.Images
{
    internal class ImageRoot
    {
        [YamlMember(Alias = "images")]
        public List<ImageItem> Items { get; set; }
    }
}
