using CoreMvvmLib.Design.Geometies;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using YamlDotNet.Serialization;

namespace CoreMvvmLib.Design.Images
{
    public class ImageContainer
    {
        internal static ImageRoot _data;
        internal static Dictionary<string, ImageItem> _items;

        static ImageContainer()
        {
            Build();
        }

        private static void Build()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var resourceName = "CoreMvvmLib.Design.Properties.Resources.images.yaml";

            using Stream stream = assembly.GetManifestResourceStream(resourceName);
            using StreamReader reader = new StreamReader(stream);

            var deserializer = new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .Build();

            ImageRoot _data = deserializer.Deserialize<ImageRoot>(reader);

            _items = new Dictionary<string, ImageItem>();
            foreach (var item in _data.Items)
            {
                if (!string.IsNullOrEmpty(item.Name))
                    _items[item.Name] = item;
            }
        }
    }
}
