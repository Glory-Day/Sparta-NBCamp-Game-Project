using System.Collections;
using System.Collections.Generic;
using Backend.Object.UI.View;
using Backend.Util.Management;
using Backend.Util.Presentation;
using UnityEngine;

namespace Backend.Object.UI.Presenter
{
    public class SettingPresenter : Presenter<SettingView, IModel>
    {
        public SettingPresenter(SettingView view, IModel model) : base(view, model)
        {
            View.PauseAction += PauseAction;
            View.CloseButton.onClick.AddListener(() => OnCloseButtonClicked());
        }

        public override void Clear()
        {
            View.PauseAction -= PauseAction;
            View.CloseButton.onClick.RemoveListener(() => OnCloseButtonClicked());
            base.Clear();
        }

        private void OnCloseButtonClicked()
        {
            ApplicationManager.QuitApplication();
        }

        private void PauseAction()
        {
            ApplicationManager.PauseApplication();
        }
    }
}
