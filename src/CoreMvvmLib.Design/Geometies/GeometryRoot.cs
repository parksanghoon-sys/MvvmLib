using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace CoreMvvmLib.Design.Geometies
{
    internal class GeometryRoot
    {
        [YamlMember(Alias = "geometries")]
        public List<GeometryItem> Items { get; set; }
    }
}
