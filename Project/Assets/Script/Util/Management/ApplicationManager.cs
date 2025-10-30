using System;
using Backend.Util.Debug;
using UnityEngine;

namespace Backend.Util.Management
{
    public class ApplicationManager : Singleton<ApplicationManager>
    {
        // 일시정지 상태
        private bool _isApplicationPaused;
        private ApplicationManager()
        {
            _isApplicationPaused = false;
        }

        // 일시정지
        private void PauseApplication_Internal()
        {
            if (_isApplicationPaused)
            {
                Debugger.LogError("Application is already paused.");
                return;
            }
            _isApplicationPaused = true;
            Time.timeScale = 0f;
        }

        // 일시정지 해제
        private void ResumeApplication_Internal()
        {
            if (!_isApplicationPaused)
            {
                Debugger.LogError("Application is not paused.");
                return;
            }
            _isApplicationPaused = false;
            Time.timeScale = 1f;
        }

        private void SwitchCursorMode_Internal(CursorLockMode mode)
        {
            Debugger.LogProgress();

            switch (mode)
            {
                case CursorLockMode.None:
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    break;
                case CursorLockMode.Locked:
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    break;
                case CursorLockMode.Confined:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // 게임 종료
        private void QuitApplication_Internal()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public static void PauseApplication()
        {
            Instance.PauseApplication_Internal();
        }

        public static void ResumeApplication()
        {
            Instance.ResumeApplication_Internal();
        }

        public static void QuitApplication()
        {
            Instance.QuitApplication_Internal();
        }

        public static void SwitchCursorMode(CursorLockMode mode)
        {
            Debugger.LogProgress();

            Instance.SwitchCursorMode_Internal(mode);
        }

        #region STATIC PROPERTIES API

        public static bool IsApplicationPaused => Instance._isApplicationPaused;

        #endregion

    }
}
