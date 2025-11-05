using System.Collections;
using System.Collections.Generic;
using Backend.Object.UI.View;
using Backend.Util.Data;
using Backend.Util.Presentation;
using UnityEngine;

namespace Backend.Object.UI.Presenter
{
    public class ArmorCalculatePresenter : Presenter<ArmorCalculateView, Inventory>
    {
        public ArmorCalculatePresenter(ArmorCalculateView view, Inventory model, Dispatcher dispatcher) : base(view, model)
        {
            View.CaculateAction += ArmorCalculate;
        }

        public override void Clear()
        {
            base.Clear();
            View.CaculateAction -= ArmorCalculate;
        }

        private void ArmorCalculate()
        {
            View.Change(0, "0");
            View.Change(1, "0");

            var physics = 0f;
            var magical = 0f;

            for (int i = 0; i < Model.items.Length; i++)
            {
                if (Model.items[i] is ArmorItemData armor)
                {
                    physics += armor.PhysicalDefensePoint;
                    magical += armor.MagicalDefensePoint;
                }
            }

            View.Change(0, physics.ToString());
            View.Change(1, magical.ToString());
        }
    }
}
