using YamlDotNet.Serialization;

namespace CoreMvvmLib.Design.Images
{
    internal class ImageItem
    {
        [YamlMember(Alias = "name")]        
        public string Name { get; set; }
        [YamlMember(Alias = "data")]        
        public string Data { get; set; }
    }
}
