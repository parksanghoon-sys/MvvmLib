using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace CoreMvvmLib.Design.Geometies
{
    public class GeometryContainer
    {
        internal static GeometryRoot _data;
        internal static Dictionary<string, GeometryItem> _items;

        static GeometryContainer()
        {
            Build();
        }

        private static void Build()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var resourceName = "EmployManager.Component.Properties.Resources.geometries.yaml";

            using Stream stream = assembly.GetManifestResourceStream(resourceName);
            using StreamReader reader = new StreamReader(stream);

            var deserializer = new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .Build();

            GeometryRoot _data = deserializer.Deserialize<GeometryRoot>(reader);

            _items = new Dictionary<string, GeometryItem>();
            foreach (var item in _data.Items)
            {
                if (!string.IsNullOrEmpty(item.Name))
                    _items[item.Name] = item;
            }
        }
}
