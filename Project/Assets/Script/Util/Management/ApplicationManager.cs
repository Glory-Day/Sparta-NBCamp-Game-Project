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

        // 게임 종료
        private void QuitApplication_Internal()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void PauseApplication()
        {
            Instance.PauseApplication_Internal();
        }

        public void ResumeApplication()
        {
            Instance.ResumeApplication_Internal();
        }

        public void QuitApplication()
        {
            Instance.QuitApplication_Internal();
        }

        #region STATIC PROPERTIES API

        public static bool IsApplicationPaused => Instance._isApplicationPaused;

        #endregion

    }
}
