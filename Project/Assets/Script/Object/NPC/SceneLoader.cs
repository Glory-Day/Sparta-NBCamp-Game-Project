using System;
using Backend.Object.Management;
using Backend.Util.Management;
using UnityEngine;

namespace Backend.Object.NPC
{
    public class SceneLoader : MonoBehaviour
    {
        [Header("Scene Settings")]
        [SerializeField] private Mode mode;
        [SerializeField] public int index;
        [SerializeField] public int id;

        public void Load()
        {
            switch (mode)
            {
                case Mode.Index:
                    SceneManager.LoadSceneByIndex(index, id);
                    break;
                case Mode.Json:
                    DataManager.LoadUserData();
                    SceneManager.LoadSceneByIndex(DataManager.UserData.SceneIndex, DataManager.UserData.SpawnerIndex);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #region NESTED ENUMERATION API

        private enum Mode
        {
            Index,
            Json
        }

        #endregion
    }
}
