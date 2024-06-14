using System.Runtime.CompilerServices;

namespace CoreMvvmLib.Design.Images
{
    public static class ImageConverter
    {
        public static string GetData([CallerMemberName] string name = null)
        {
            return ImageContainer._items[name].Data;
        }
    }
}