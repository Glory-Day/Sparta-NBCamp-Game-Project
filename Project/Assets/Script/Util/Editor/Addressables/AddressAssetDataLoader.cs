#if UNITY_EDITOR

using System.IO;
using System.Linq;
using System.Text;
using Backend.Util.Debug;
using Backend.Util.Json;
using Backend.Util.Json.Data.Addressables;
using UnityEngine;

namespace Backend.Util.Editor.Addressables
{
    public class AddressAssetDataLoader
    {
        #region CONSTANT FIELD API

        private const string CacheFilePath = "/Addressables";
        private const string CacheFileName = "/cache.json";

        #endregion

        private readonly AddressLoader _loader = new();

        private readonly StringBuilder _builder = new();

        private readonly string _folderPath;
        private readonly string _filePath;

        public AddressAssetDataLoader()
        {
            _folderPath = Application.persistentDataPath + CacheFilePath;
            _filePath = _folderPath + CacheFileName;
        }

        public void LoadAddressableAssetCacheData(AddressAssetTreeView treeView)
        {
            Debugger.LogProgress();

            if (Directory.Exists(_folderPath) == false)
            {
                Directory.CreateDirectory(_folderPath);
            }

            if (File.Exists(_filePath) == false)
            {
                Cache = new AddressAssetCacheData();

                JsonSerializer.Serialize(_filePath, Cache);
            }

            Cache = JsonSerializer.Deserialize<AddressAssetCacheData>(_filePath);

            treeView.Clear();

            for (var i = 0; i < Cache.AddressAssetData.Addresses.Count; i++)
            {
                var item = new AddressAssetTreeViewItem
                           {
                               Address = Cache.AddressAssetData.Addresses[i],
                               Name = Cache.AddressAssetData.ToConstantFieldName(i)
                           };

                treeView.AddItem(item);
            }
        }

        public void SaveAddressableAssetCacheData()
        {
            Debugger.LogProgress();

            JsonSerializer.Serialize(_filePath, Cache);
        }

        public void UpdateAddressableAssetData(AddressAssetTreeView treeView)
        {
            Debugger.LogProgress();

            _loader.Update();

            Cache.AddressAssetData.Addresses = _loader.Addresses;

            SaveAddressableAssetCacheData();
            LoadAddressableAssetCacheData(treeView);

            CreateClassFile();
        }

        private void CreateClassFile()
        {
            if (Directory.Exists(Cache.Path) == false)
            {
                Directory.CreateDirectory(Cache.Path);
            }

            var labels = _loader.Groups.Values.SelectMany(value => value.Keys).ToHashSet();

            _builder.Append("using System.Collections.Generic;\n");
            _builder.Append("\n");
            _builder.Append($"namespace {Cache.Namespace}\n");
            _builder.Append("{\n");
            _builder.Append($"\tpublic static class {Cache.ClassName}\n");
            _builder.Append("\t{\n");

            var data = Cache.AddressAssetData;
            for (var i = 0; i < data.Addresses.Count; i++)
            {
                _builder.Append($"\t\tpublic const string {data.ToConstantFieldName(i)} = \"{data.Addresses[i]}\";\n");
            }
            _builder.Append("\t\t\n");

            _builder.Append("\t\tpublic static Dictionary<string, Dictionary<string, HashSet<string>>> Groups = new ()\n");
            _builder.Append("\t\t{\n");

            foreach (var group in _loader.Groups)
            {
                _builder.Append("\t\t\t{\n");
                _builder.Append($"\t\t\t\t\"{group.Key}\",\n");
                _builder.Append("\t\t\t\tnew Dictionary<string, HashSet<string>>()\n");
                _builder.Append("\t\t\t\t{\n");

                foreach (var label in labels)
                {
                    if (_loader.Groups[group.Key].ContainsKey(label) == false)
                    {
                        _builder.Append($"\t\t\t\t\t{{ \"{label}\", new HashSet<string> {{ }} }},\n");
                    }
                }

                foreach (var item in _loader.Groups[group.Key])
                {
                    var value = string.Join(",", item.Value.Select(i => $"\"{i}\""));
                    _builder.Append($"\t\t\t\t\t{{ \"{item.Key}\", new HashSet<string> {{ {value} }} }},\n");
                }

                _builder.Append("\t\t\t\t}\n");
                _builder.Append("\t\t\t},\n");
            }

            _builder.Append("\t\t};\n");
            _builder.Append("\n");

            _builder.Append("\t\t#region NESTED STRUCTURE API\n");
            _builder.Append("\n");

            _builder.Append("\t\tpublic struct Group\n");
            _builder.Append("\t\t{\n");

            foreach (var key in _loader.Groups.Keys)
            {
                _builder.Append($"\t\t\tpublic const string {key.Replace(" ", "_")} = \"{key}\";\n");
            }

            _builder.Append("\t\t}\n");
            _builder.Append("\n");

            _builder.Append("\t\tpublic struct Label\n");
            _builder.Append("\t\t{\n");

            foreach (var key in labels)
            {
                _builder.Append($"\t\t\tpublic const string {key} = \"{key}\";\n");
            }

            _builder.Append("\t\t}\n");
            _builder.Append("\n");

            _builder.Append("\t\t#endregion\n");

            _builder.Append("\t}\n");
            _builder.Append("}");

            var text = _builder.ToString();

            var fileName = $"{Application.dataPath}/{Cache.Path}/{Cache.ClassName}.cs";

            File.WriteAllText(fileName, text);
        }

        public AddressAssetCacheData Cache { get; private set; }
    }
}

#endif
