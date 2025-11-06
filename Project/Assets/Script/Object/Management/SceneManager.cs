using System;
using System.Collections;
using System.Collections.Generic;
using Backend.Object.Process;
using Backend.Object.UI;
using Backend.Object.UI.View;
using Backend.Util.Data;
using Backend.Util.Debug;
using Backend.Util.Management;
using Script.Util.Extension;
using UnityEngine;
using UnityEngine.SceneManagement;
using Path = System.IO.Path;

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

        private IEnumerator LoadingScene_Internal(int index, int id)
        {
            IsSceneLoaded = false;

            var previousSceneName = _sceneNames[_currentSceneIndex];
            var nextSceneName = _sceneNames[index];

            Debugger.LogProgress();
            Debugger.LogMessage($"{nextSceneName} is loading...");

            UIManager.CloseAllDefaultWindows();
            UIManager.Clear();

            Debugger.LogMessage($"All user interfaces are closed.");

            var value = UIManager.GetDefaultWindow(AddressData.Assets_Prefab_UI_Loading_Window_Prefab);
            var window = value.GetComponent<Window>();
            window.Open();

            var view = window.GetComponentInChildren<PointBarView>();
            view.Change(0f);

            Debugger.LogMessage($"Loading screen is opening.");

            ResourceManager.LoadAssetsByLabelAsync(previousSceneName, nextSceneName);

            var percentage = 0f;
            while (ResourceManager.IsLoadedDone == false)
            {
                if (percentage < 0.3f)
                {
                    percentage = Mathf.Lerp(view.Percentage, ResourceManager.GetProgress() * 0.3f, 0.1f);
                    view.Change(percentage);

                    yield return null;
                }
            }

            Debugger.LogMessage($"All assets are loaded.");

            _asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(nextSceneName);
            if (_asyncOperation == null)
            {
                Debugger.LogError($"{CurrentSceneName_Internal} loading failed.");

                yield break;
            }

            percentage = 0.3f;
            while (_asyncOperation.isDone == false)
            {
                if (percentage < 0.9f)
                {
                    percentage = Mathf.Lerp(view.Percentage, 0.3f + (_asyncOperation.progress * 0.6f), 0.1f);
                    view.Change(percentage);

                    yield return null;
                }
            }

            _currentSceneIndex = index;

            Debugger.LogMessage($"{CurrentSceneName_Internal} are loaded.");

            var bootstrap = FindObjectOfType<Bootstrap>();
            if (bootstrap == null)
            {
                Debugger.LogError("Bootstrap not found in the scene.");
            }
            else
            {
                Debugger.LogMessage("Bootstrap found in the scene.");
                Debugger.LogMessage("Bootstrap is running.");

                bootstrap.Run(id);

                while (bootstrap.IsDone == false)
                {
                    yield return null;
                }
            }

            view.Change(1f);

            ApplicationManager.SwitchCursorMode(index == 0 ? CursorLockMode.None : CursorLockMode.Locked);

            window.Close();

            IsSceneLoaded = true;

            Debugger.LogSuccess($"<b>{nextSceneName}</b> is loaded completely.");
        }

        private async void LoadSceneByIndex_Internal(int index, int id)
        {
            try
            {
                Debugger.LogProgress();

                await LoadingScene_Internal(index, id).AsTask(this);
            }
            catch (Exception exception)
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
        public static void LoadSceneByIndex(int index, int id)
        {
            Instance.LoadSceneByIndex_Internal(index, id);
        }

        #endregion

        #region STATIC PROPERTIES API

        public static bool IsSceneLoaded { get; private set; }

        public static int CurrentSceneIndex => Instance._currentSceneIndex;

        public static string CurrentSceneName => Instance.CurrentSceneName_Internal;

        #endregion
    }
}
