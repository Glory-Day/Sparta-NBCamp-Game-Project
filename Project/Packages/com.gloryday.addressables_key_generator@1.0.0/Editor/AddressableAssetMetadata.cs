#if UNITY_EDITOR

using System.Collections.Generic;

namespace GloryDay.Addressables
{
    [System.Serializable]
    public record AddressableAssetMetadata
    {
        public List<string> Addresses { get; set; } =  new ();

        public Dictionary<string, Dictionary<string, List<string>>> Groups { get; set; } = new ();
    }
}

#endif
