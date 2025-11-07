using System.Collections;
using System.Collections.Generic;
using Backend.Object.UI.Presenter;
using Backend.Object.UI.View;
using Backend.Util.Presentation;
using UnityEngine;

namespace Backend.Object.UI
{
    public class SettingInformationBinder : MonoBehaviour
    {
        [SerializeField] private SettingView settingView;

        private SettingPresenter _settingPresenter;

        public void Bind()
        {
            _settingPresenter = PresenterFactory.Create<SettingPresenter>(settingView, null);
        }

        private void OnDestroy()
        {
            _settingPresenter.Clear();
        }
    }
}
