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
            var resourceName = "CoreMvvmLib.Component.Properties.Resources.geometries.yaml";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                StringReader r = new StringReader(reader.ReadToEnd());
                Deserializer deserializer = new Deserializer();
                object yamlObject = deserializer.Deserialize<object>(r);

                JsonSerializer js = new JsonSerializer();
                StringWriter w = new StringWriter();
                js.Serialize(w, yamlObject);
                string jsonText = w.ToString();
                _data = JsonConvert.DeserializeObject<GeometryRoot>(jsonText);
                _items = new Dictionary<string, GeometryItem>();

                foreach (var item in _data.Items)
                {
                    _items.Add(item.Name, item);
                }
            }
        }
    }
}
