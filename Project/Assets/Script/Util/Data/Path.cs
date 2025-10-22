using UnityEngine;

namespace Backend.Util.Data
{
    public static class Path
    {
        private const string StatusDataDirectory = "Data/Status/";

        public static string ToAbsolutePath(string fileName)
        {
            return Application.persistentDataPath + StatusDataDirectory + fileName;
        }
    }
}
