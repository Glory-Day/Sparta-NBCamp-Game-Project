using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Backend.Object.Process;
using Backend.Util.Debug;
using Backend.Util.Management;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Backend.Object.Management
{
    public class SceneManager : SingletonGameObject<SceneManager>
    {
        private readonly List<string> _sceneNames = new ();

        private int _currentSceneIndex;

        /// <summary>
        /// To check the status of asynchronous scene load operations.
        /// </summary>
        private AsyncOperation _asyncOperation;

        protected override void OnAwake()
        {
            Debugger.LogProgress();

            // Initialize the names of the built scenes.
            for (var i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; i++)
            {
                var path = SceneUtility.GetScenePathByBuildIndex(i);
                _sceneNames.Add(Path.GetFileNameWithoutExtension(path));
            }

            _currentSceneIndex = 0;
        }

        private IEnumerator LoadingScene_Internal(int index)
        {
            IsSceneLoaded = false;

            var previousSceneName = _sceneNames[_currentSceneIndex];
            var nextSceneName = _sceneNames[index];

            Debugger.LogProgress();
            Debugger.LogMessage($"{nextSceneName} is loading...");

            Util.Management.ResourceManager.LoadAssetsByLabelAsync(previousSceneName, nextSceneName);

            while (Util.Management.ResourceManager.IsLoadedDone == false)
            {
                yield return null;
            }

            _asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Single);

            while (_asyncOperation?.isDone == false)
            {
                yield return null;
            }

            _currentSceneIndex = index;

            var bootstrap = FindObjectOfType<Bootstrap>();
            if (bootstrap == null)
            {
                Debugger.LogError("Bootstrap not found in the scene.");
            }
            else
            {
                Debugger.LogSuccess("Bootstrap found in the scene.");

                bootstrap.Run();

                while (bootstrap.IsDone == false)
                {
                    yield return null;
                }
            }

            ApplicationManager.SwitchCursorMode(index == 0 ? CursorLockMode.None : CursorLockMode.Locked);

            IsSceneLoaded = true;

            Debugger.LogSuccess($"<b>{nextSceneName}</b> is loaded.");
        }

        private void LoadSceneByIndex_Internal(int index)
        {
            Debugger.LogProgress();

            try
            {
                StartCoroutine(LoadingScene_Internal(index));
            }
            catch (IndexOutOfRangeException exception)
            {
                Debugger.LogError(exception.Message);
            }
        }

        private string CurrentSceneName_Internal => _sceneNames[_currentSceneIndex];

        #region STATIC METHOD API

        /// <summary>
        /// Load the scene asynchronously by index.
        /// </summary>
        /// <param name="index"> Number of scene index. </param>
        public static void LoadSceneByIndex(int index)
        {
            Instance.LoadSceneByIndex_Internal(index);
        }

        #endregion

        #region STATIC PROPERTIES API

        public static bool IsSceneLoaded { get; private set; }

        public static int CurrentSceneIndex => Instance._currentSceneIndex;

        public static string CurrentSceneName => Instance.CurrentSceneName_Internal;

        #endregion
    }
}
