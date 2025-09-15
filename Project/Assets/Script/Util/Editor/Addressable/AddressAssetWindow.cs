#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Backend.Util.Debug;
using Backend.Util.Json;
using Backend.Util.Json.Data.Addressables;

namespace Backend.Util.Editor.Addressable
{
    public class AddressAssetWindow : EditorWindow
    {
        #region CONSTANT FIELD API

        private const string CacheFilePath = "/Addressables";
        private const string CacheFileName = "/cache.json";

        #endregion

        private Vector2 _scroll;

        private int _selectedTabIndex;
        private string[] _tabNames = new string[] { "Current", "Update", "Settings" };

        private MultiColumnHeader _multiColumnHeader;
        private MultiColumnHeaderState _multiColumnHeaderState;
        private MultiColumnHeaderState.Column[] _columns;

        public string _path;
        public AddressAssetCacheData Data;
        private readonly AddressableDataLoader _loader = new();

        public AddressAssetSettingWindow PopUpWindow;

        private readonly StringBuilder _builder = new();

        private Rect _settingsButtonPosition;
        private bool _isOpened;

        private void Awake()
        {
            Debugger.LogProgress();

            InitializeMultiColumnHeader();
        }

        private void OnEnable()
        {
            _path = Application.persistentDataPath + CacheFilePath;
            Debugger.LogMessage(_path);

            LoadAddressableAssetData();
        }

        [MenuItem("Window/Addressable/Address Viewer")]
        public static void ShowWindow()
        {
            Debugger.LogProgress();

            GetWindow<AddressAssetWindow>("Address Viewer");
        }

        private void InitializeMultiColumnHeader()
        {
            _columns = new MultiColumnHeaderState.Column[]
            {
                new MultiColumnHeaderState.Column()
                {
                    allowToggleVisibility = true,
                    autoResize = false,
                    canSort = false,
                    sortingArrowAlignment = TextAlignment.Right,
                    headerContent = new GUIContent(EditorGUIUtility.IconContent("d_UnityEditor.ConsoleWindow")),
                    headerTextAlignment = TextAlignment.Center,
                    width = 24f
                },
                new MultiColumnHeaderState.Column()
                {
                    allowToggleVisibility = true,
                    autoResize = true,
                    canSort = true,
                    sortingArrowAlignment = TextAlignment.Right,
                    headerContent = new GUIContent("Address"),
                    headerTextAlignment = TextAlignment.Left,
                    minWidth = 128f
                },
                new MultiColumnHeaderState.Column()
                {
                    allowToggleVisibility = true,
                    autoResize = false,
                    canSort = false,
                    sortingArrowAlignment = TextAlignment.Right,
                    headerContent = new GUIContent("State"),
                    headerTextAlignment = TextAlignment.Left,
                }
            };

            _multiColumnHeaderState = new MultiColumnHeaderState(_columns);
            _multiColumnHeader = new MultiColumnHeader(_multiColumnHeaderState);

            _multiColumnHeader.visibleColumnsChanged += (multiColumnHeader) => multiColumnHeader.ResizeToFit();

            // Initial resizing of the content.
            _multiColumnHeader.ResizeToFit();
        }

        public void OnGUI()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            GUILayout.Button("Fetch", EditorStyles.toolbarButton, GUILayout.Width(45f));
            if (GUILayout.Button("Update", EditorStyles.toolbarButton, GUILayout.Width(54f)))
            {
                UpdateAddressableAssetData();
            }

            if (GUILayout.Button("Settings", EditorStyles.toolbarButton, GUILayout.Width(60f)))
            {
                _isOpened = _isOpened == false;

                var rect = GUILayoutUtility.GetLastRect();
                var screenPosition = GUIUtility.GUIToScreenPoint(new Vector2(rect.x, rect.y + rect.height));

                var height = 20f + EditorGUIUtility.singleLineHeight * 5f + 2f * 4f;
                // window.position = new Rect(45f + 54f + 0.6f, EditorGUIUtility.singleLineHeight + 3f, 280f, height);

                if (PopUpWindow == null)
                {
                    PopUpWindow = CreateInstance<AddressAssetSettingWindow>();
                    PopUpWindow.position = new Rect(screenPosition.x + 45f + 54f + 0.6f, screenPosition.y + EditorGUIUtility.singleLineHeight + 3f, 280f, height);
                    PopUpWindow.SetParentWindow(this);
                    PopUpWindow.ShowPopup();
                }
                else
                {
                    PopUpWindow.Close();
                    PopUpWindow = null;
                }
            }

            GUILayout.Label(" ");

            EditorGUILayout.EndHorizontal();

            if (_multiColumnHeader is null)
            {
                InitializeMultiColumnHeader();
            }

            GUILayout.FlexibleSpace();

            // Get automatically aligned rect for our multi-column header component.
            var windowRect = GUILayoutUtility.GetLastRect();
            windowRect.width = position.width;
            windowRect.height = position.height;

            var columnHeight = EditorGUIUtility.singleLineHeight;
            var columnRectPrototype = new Rect(windowRect)
            {
                height = columnHeight * 1.6f
            };

            _multiColumnHeader.OnGUI(columnRectPrototype, 0.0f);
        }

        public void LoadAddressableAssetData()
        {
            if (Directory.Exists(_path) == false)
            {
                Directory.CreateDirectory(_path);
            }

            var fullFilePath = _path + CacheFileName;
            if (File.Exists(fullFilePath) == false)
            {
                Data = new AddressAssetCacheData();

                JsonSerializer.Serialize(fullFilePath, Data);
            }

            Data = JsonSerializer.Deserialize<AddressAssetCacheData>(fullFilePath);
        }

        public void SaveAddressableAssetData()
        {
            JsonSerializer.Serialize(_path + CacheFileName, Data);
        }

        public void UpdateAddressableAssetData()
        {

        }

        private void CreateClassFile()
        {
            if (Directory.Exists(Data.Path) == false)
            {
                Directory.CreateDirectory(Data.Path);
            }

            var fileName = $"{Application.persistentDataPath}/{Data.Path}/{Data.ClassName}.cs";
            if (File.Exists(fileName) == false)
            {
                File.Create(fileName);
            }

            _loader.Update();

            _builder.Append("using System.Collections.Generic\n");
            _builder.Append("\n");
            _builder.Append($"namespace {Data.Namespace}\n");
            _builder.Append("{\n");
            _builder.Append($"\tpublic static class {Data.ClassName}\n");
            _builder.Append("\t{\n");
            _builder.Append("\t\n");
            _builder.Append("\t}\n");
            _builder.Append("}");


        }
    }
}


#endif
