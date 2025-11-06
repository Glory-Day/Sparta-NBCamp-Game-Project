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

        public void Load()
        {
            switch (mode)
            {
                case Mode.Index:
                    SceneManager.LoadSceneByIndex(index);
                    break;
                case Mode.Json:
                    SceneManager.LoadSceneByIndex(DataManager.UserData.SceneIndex);
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
