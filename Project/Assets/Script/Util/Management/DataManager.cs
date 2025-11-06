using System.IO;
using Backend.Util.Data;
using Backend.Util.Json.Data;
using UnityEngine;
using File = System.IO.File;
using JsonSerializer = Backend.Util.Json.JsonSerializer;

namespace Backend.Util.Management
{
    public class DataManager : Singleton<DataManager>
    {
        #region PRIVATE CONSTANTS FIELD API

        private const string DefaultDirectoryPath = "/Data";

        private const string UserDataPath = "/Data/user.json";
        private const string SettingDataPath = "/Data/setting.json";
        private const string StatusDataPath = "/Data/status.json";

        #endregion

        private UserData _userData;
        private SettingData _settingData;
        private PlayerStatusData _statusData;

        private DataManager()
        {
            var path = Application.persistentDataPath + DefaultDirectoryPath;

            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }
        }

        private void LoadUserData_Internal()
        {
            var data = JsonSerializer.Deserialize<UserData>(Application.persistentDataPath + UserDataPath);

            _userData = data;
        }

        private void LoadSettingData_Internal()
        {
            var data = JsonSerializer.Deserialize<SettingData>(Application.persistentDataPath + SettingDataPath);

            _settingData = data;
        }

        private void LoadStatusData_Internal()
        {
            var data = JsonSerializer.Deserialize<PlayerStatusData>(Application.persistentDataPath + StatusDataPath);

            _statusData = data;
        }

        private void ResetUserData_Internal()
        {
            _userData = new UserData();

            SaveUserData_Internal();
        }

        private void ResetSettingData_Internal()
        {
            _settingData = new SettingData();

            SaveSettingData_Internal();
        }

        private void ResetStatusData_Internal()
        {
            _statusData = ScriptableObject.CreateInstance<PlayerStatusData>();

            SaveStatusData_Internal();
        }

        private void SaveUserData_Internal()
        {
            JsonSerializer.Serialize(Application.persistentDataPath + UserDataPath, _userData);
        }

        private void SaveSettingData_Internal()
        {
            JsonSerializer.Serialize(Application.persistentDataPath + SettingDataPath, _settingData);
        }

        private void SaveStatusData_Internal()
        {
            JsonSerializer.Serialize(Application.persistentDataPath + StatusDataPath, _statusData);
        }

        private void LoadAllData_Internal()
        {
            LoadUserData_Internal();
            LoadSettingData_Internal();
            LoadStatusData_Internal();
        }

        private void ResetAllData_Internal()
        {
            ResetUserData_Internal();
            ResetSettingData_Internal();
            ResetStatusData_Internal();
        }

        private void SaveAllData_Internal()
        {
            SaveUserData_Internal();
            SaveSettingData_Internal();
            SaveStatusData_Internal();
        }

        private bool IsUserDataExisted_Internal()
        {
            return File.Exists(Application.persistentDataPath + UserDataPath);
        }

        private bool IsSettingDataExisted_Internal()
        {
            return File.Exists(Application.persistentDataPath + SettingDataPath);
        }

        private bool IsStatusDataExisted_Internal()
        {
            return File.Exists(Application.persistentDataPath + StatusDataPath);
        }

        #region STATIC METHODS API

        public static void LoadUserData()
        {
            Instance.LoadUserData_Internal();
        }

        public static void LoadSettingData()
        {
            Instance.LoadSettingData_Internal();
        }

        public static void LoadStatusData()
        {
            Instance.LoadStatusData_Internal();
        }

        public static void SaveUserData()
        {
            Instance.SaveUserData_Internal();
        }

        public static void ResetUserData()
        {
            Instance.ResetUserData_Internal();
        }

        public static void ResetSettingData()
        {
            Instance.ResetSettingData_Internal();
        }

        public static void ResetStatusData()
        {
            Instance.ResetStatusData_Internal();
        }

        public static void SaveSettingData()
        {
            Instance.SaveSettingData_Internal();
        }

        public static void SaveStatusData()
        {
            Instance.SaveStatusData_Internal();
        }

        public static void LoadAllData()
        {
            Instance.LoadAllData_Internal();
        }

        public static void ResetAllData()
        {
            Instance.ResetAllData_Internal();
        }

        public static void SaveAllData()
        {
            Instance.SaveAllData_Internal();
        }

        #endregion

        #region STATIC PROPERTIES API

        public static UserData UserData => Instance._userData;

        public static SettingData SettingData => Instance._settingData;

        public static StatusData StatusData => Instance._statusData;

        public static bool IsUserDataExisted => Instance.IsUserDataExisted_Internal();

        public static bool IsSettingDataExisted => Instance.IsSettingDataExisted_Internal();

        public static bool IsStatusDataExisted => Instance.IsStatusDataExisted_Internal();

        #endregion
    }
}
