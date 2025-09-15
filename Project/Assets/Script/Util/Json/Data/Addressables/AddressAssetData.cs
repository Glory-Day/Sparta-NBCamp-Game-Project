using System.Collections.Generic;

namespace Backend.Util.Json.Data.Addressables
{
    [System.Serializable]
    public class AddressAssetData
    {
        public List<string> Names { get; set; } = new();

        public List<string> Addresses { get; set; } = new();

        public Dictionary<string, List<string>> Groups { get; set; } = new();
    }

    [System.Serializable]
    public class AddressAssetCacheData
    {
        public string Path { get; set; } = string.Empty;

        public string Namespace { get; set; } = string.Empty;

        public string ClassName { get; set; } = string.Empty;

        public AddressAssetData AddressAssetData { get; set; } = new();
    }
}
