#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor.AddressableAssets;
using Backend.Util.Debug;

namespace Backend.Util.Editor.Addressable
{
    public class AddressableDataLoader
    {
        public AddressableDataLoader()
        {
            Addresses = new List<string>();

            Groups = new Dictionary<string, List<string>>();
        }

        public void Update()
        {
            Addresses.Clear();
            Groups.Clear();

            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debugger.LogError("Addressable settings is not existed.");

                return;
            }

            foreach (var group in settings.groups)
            {
                if (group == null)
                {
                    continue;
                }

                foreach (var entry in group.entries)
                {
                    if (entry == null)
                    {
                        continue;
                    }

                    Addresses.Add(entry.address);

                    foreach (var label in entry.labels)
                    {
                        if (Groups.TryGetValue(label, out var set))
                        {
                            set = new List<string>();

                            Groups[label] = set;
                        }

                        set.Add(entry.address);
                    }
                }
            }
        }

        public List<string> Addresses { get; private set; }

        public Dictionary<string, List<string>> Groups { get; private set; }
    }
}

#endif
