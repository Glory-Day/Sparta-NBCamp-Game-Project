using System.Collections;
using System.Collections.Generic;
using Backend.Object.Character.Player;
using Backend.Object.UI.View;
using Backend.Util.Data;
using Backend.Util.Presentation;
using Backend.Util.Presentation.Message;
using Script.Object.UI;
using UnityEngine;

namespace Backend.Object.UI.Presenter
{
    public class StatusTextPresenter : TextPresenter
    {
        private StatusType _statusType;
        public StatusTextPresenter(StatusTextView view, PlayerStatus model, Dispatcher dispatcher) : base(view, model, dispatcher)
        {
            view.StatusAction += UpdateStatus;
            _statusType = view.StatusType;
        }

        public override void Receive<T>(T message)
        {
            if (Model is PlayerStatus playerStatus)
            {
                var status = (PlayerStatusData)playerStatus.data;

                switch (message)
                {
                    case StatusPointMessage msg when msg.StatusType == StatusType.Level:
                        View.Change(status.Level.ToString());
                        break;
                    case StatusPointMessage msg when msg.StatusType == StatusType.Life:
                        View.Change(status.LifePoint.ToString());
                        break;
                    case StatusPointMessage msg when msg.StatusType == StatusType.Concentration:
                        View.Change(status.ConcentrationPoint.ToString());
                        break;
                    case StatusPointMessage msg when msg.StatusType == StatusType.Endurance:
                        View.Change(status.EndurancePoint.ToString());
                        break;
                    case StatusPointMessage msg when msg.StatusType == StatusType.Health:
                        View.Change(status.HealthPoint.ToString());
                        break;
                    case StatusPointMessage msg when msg.StatusType == StatusType.Strength:
                        View.Change(status.StrengthPoint.ToString());
                        break;
                    case StatusPointMessage msg when msg.StatusType == StatusType.Intelligence:
                        View.Change(status.IntelligencePoint.ToString());
                        break;
                    case StatusPointMessage msg when msg.StatusType == StatusType.Luck:
                        View.Change(status.LuckPoint.ToString());
                        break;
                    case StatusPointMessage msg when msg.StatusType == StatusType.MaximumHealth:
                        View.Change(playerStatus.maximumHealthPoint.ToString());
                        break;
                    case StatusPointMessage msg when msg.StatusType == StatusType.MaximumStamina:
                        View.Change(playerStatus.maximumStaminaPoint.ToString());
                        break;
                }
            }
        }

        private void UpdateStatus()
        {
            if (Model is PlayerStatus playerStatus)
            {
                var status = (PlayerStatusData)playerStatus.data;

                switch (_statusType)
                {
                    case StatusType.Level:
                        View.Change(status.Level.ToString());
                        break;
                    case StatusType.Life:
                        View.Change(status.LifePoint.ToString());
                        break;
                    case StatusType.Concentration:
                        View.Change(status.ConcentrationPoint.ToString());
                        break;
                    case StatusType.Endurance:
                        View.Change(status.EndurancePoint.ToString());
                        break;
                    case StatusType.Health:
                        View.Change(status.HealthPoint.ToString());
                        break;
                    case StatusType.Strength:
                        View.Change(status.StrengthPoint.ToString());
                        break;
                    case StatusType.Intelligence:
                        View.Change(status.IntelligencePoint.ToString());
                        break;
                    case StatusType.Luck:
                        View.Change(status.LuckPoint.ToString());
                        break;
                    case StatusType.MaximumHealth:
                        View.Change(playerStatus.maximumHealthPoint.ToString());
                        break;
                    case StatusType.MaximumStamina:
                        View.Change(playerStatus.maximumStaminaPoint.ToString());
                        break;
                }
            }
        }
    }
}
