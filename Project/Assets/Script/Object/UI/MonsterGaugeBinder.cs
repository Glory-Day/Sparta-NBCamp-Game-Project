using System.Collections;
using System.Collections.Generic;
using Backend.Object.Character.Enemy;
using Backend.Object.UI.Presenter;
using Backend.Object.UI.View;
using Backend.Util.Presentation;
using UnityEngine;

namespace Backend.Object.UI
{
    public class MonsterGaugeBinder : MonoBehaviour
    {
        [SerializeField] private EnemyHealthPointBarView normalMonsterHpBarView;

        private HealthPointBarPresenter _healthPointBarPresenter;
        public void Bind(EnemyStatus status)
        {
            _healthPointBarPresenter = PresenterFactory.Create<HealthPointBarPresenter>(normalMonsterHpBarView, status);
        }

        public void OnDestroy()
        {
            _healthPointBarPresenter.Clear();
        }
    }
}
