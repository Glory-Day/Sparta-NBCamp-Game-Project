using System.Collections;
using System.Collections.Generic;
using Backend.Object.UI.View;
using Backend.Util.Data;
using Backend.Util.Presentation;
using Backend.Util.Presentation.Message;
using Script.Object.UI;
using Script.Object.UI.View;
using Unity.VisualScripting;
using UnityEngine;

namespace Backend.Object.UI.Presenter
{
    public class WeaponCalculatePresenter : Presenter<WeaponCalculateView, Inventory>
    {
        public WeaponCalculatePresenter(WeaponCalculateView view, Inventory model, Dispatcher dispatcher) : base(view, model)
        {
            View.CaculateAction += WeaponCalculate;
        }

        public override void Clear()
        {
            base.Clear();
            View.CaculateAction -= WeaponCalculate;
        }

        private void WeaponCalculate()
        {
            for (int i = 0; i < Model.items.Length; i++)
            {
                if (Model.items[i] is WeaponItemData weapon)
                {
                    View.Change(i, weapon.DamagePoint.ToString());
                }
                else
                {
                    View.Change(i, "0");
                }
            }
        }
    }
}
