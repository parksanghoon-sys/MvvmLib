using CoreMvvmLib.Core.Services.RegionManager;

namespace CoreMvvmLib.WPF.Services;

internal class RegionStorage
{
    private static Dictionary<string, IRegion> _regions = new Dictionary<string, IRegion>();

    public static IRegion SetRegisterRegion(string regionKey, Type viewType)
    {
        foreach(var region in  _regions)
        {
            if(region.Key == regionKey)
            {
                throw new ArgumentException("Region 키가 동일합니다");
            }                
        }
        var _region = new Region(viewType.Name);
        _regions.Add(regionKey, _region);
        return _region;
    }
    public static void SetRegisterRegion(string regionKey)
    {
        var region = new Region(regionKey);
        _regions.Add(regionKey, region);
    }
    public static IRegion GetRegion(string regionKey)
    {
        if (_regions.ContainsKey(regionKey))
            return _regions[regionKey];
        else
            return null;
    }
    public static bool IsCheckRegionExist(string  regionKey)
    {
        if(_regions.ContainsKey(regionKey))
            return true;
        else
            return false;
    }


}
