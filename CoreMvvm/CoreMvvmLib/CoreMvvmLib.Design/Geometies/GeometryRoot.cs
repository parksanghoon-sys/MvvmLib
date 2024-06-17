using Newtonsoft.Json;
using System.Collections.Generic;

namespace CoreMvvmLib.Design.Geometies
{
    internal class GeometryRoot
    {
        [JsonProperty("geometries")]
        public List<GeometryItem> Items { get; set; }
    }
}
