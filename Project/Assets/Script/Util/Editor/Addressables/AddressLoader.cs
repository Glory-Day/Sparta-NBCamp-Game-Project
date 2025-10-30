#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using UnityEditor.AddressableAssets;
using Backend.Util.Debug;

namespace Backend.Util.Editor.Addressables
{
    public class AddressLoader
    {
        public void Update()
        {
            Addresses.Clear();
            Groups.Clear();

            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings is null)
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

                if (IsDefaultGroup(group.Name) == false)
                {
                    Groups.Add(group.Name, new Dictionary<string, List<string>>());
                    foreach (var entry in group.entries)
                    {
                        if (IsDefaultAsset(entry.address) == false)
                        {
                            Addresses.Add(entry.address);
                        }

                        foreach (var label in entry.labels)
                        {
                            if (Groups[group.Name].TryGetValue(label, out var list) == false)
                            {
                                list = new List<string>();

                                Groups[group.Name][label] = list;
                            }
                            list.Add(entry.address);
                        }
                    }
                }
            }
        }

        private bool IsDefaultAsset(string name)
        {
            return name is  "EditorSceneList" or "Resources";
        }

        private bool IsDefaultGroup(string name)
        {
            return name is "Default Local Group" or "Built In Data";
        }

        public List<string> Addresses { get; } = new ();

        public Dictionary<string, Dictionary<string, List<string>>> Groups { get; } = new ();

        public Dictionary<string, List<string>> Labels { get; } = new ();
    }
}

#endif
