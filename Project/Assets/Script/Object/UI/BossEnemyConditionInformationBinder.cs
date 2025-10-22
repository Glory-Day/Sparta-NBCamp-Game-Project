using System;
using Backend.Object.Character;
using Backend.Object.Character.Enemy;
using Backend.Object.UI;
using Backend.Object.UI.View;
using Backend.Util.Presentation;
using UnityEngine;

namespace Script.Object.UI
{
    public class BossEnemyConditionInformationBinder : MonoBehaviour
    {
        [Header("View References")]
        [SerializeField] private TextView textView;
        [SerializeField] private DelayedPointBarView healthPointBarView;

        private BossEnemyNameTextPresenter _bossEnemyNameTextPresenter;
        private HealthPointBarPresenter _healthPointBarPresenter;

        public void Bind(EnemyStatus model)
        {
            _bossEnemyNameTextPresenter = PresenterFactory.Create<BossEnemyNameTextPresenter>(textView, model);
            _healthPointBarPresenter = PresenterFactory.Create<HealthPointBarPresenter>(healthPointBarView, model);
        }

        public void OnDestroy()
        {
            _healthPointBarPresenter.Clear();
        }
    }
}
