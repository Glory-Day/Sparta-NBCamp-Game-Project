using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Backend.Util.Debug;
using Backend.Util.Management;
using Script.Test;
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

        private WaitUntil _delay;

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

            _delay = new WaitUntil(() => _asyncOperation.isDone);
        }

        private IEnumerator LoadingScene_Internal(string sceneName)
        {
            Debugger.LogProgress();
            Debugger.LogMessage($"{sceneName} is loading...");

            IsSceneLoaded = false;

            _asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

            yield return _delay;

            var bootstrap = FindObjectOfType<Bootstrap>();
            if (bootstrap == null)
            {
                Debugger.LogError("Bootstrap not found in the scene.");
            }
            else
            {
                Debugger.LogSuccess("Bootstrap found in the scene.");
            }

            IsSceneLoaded = true;

            Debugger.LogSuccess($"<b>{sceneName}</b> is loaded.");
        }

        private void LoadSceneByIndex_Internal(int index)
        {
            Debugger.LogProgress();

            try
            {
                var previousSceneName = _sceneNames[_currentSceneIndex];
                var nextSceneName = _sceneNames[index];

                Util.Management.ResourceManager.LoadAssetsByLabelAsync(previousSceneName, nextSceneName);

                StartCoroutine(LoadingScene_Internal(nextSceneName));

                ApplicationManager.SwitchCursorMode(index == 0 ? CursorLockMode.None : CursorLockMode.Locked);
            }
            catch (IndexOutOfRangeException exception)
            {
                Debugger.LogError(exception.Message);
            }
        }

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

        #endregion
    }
}
