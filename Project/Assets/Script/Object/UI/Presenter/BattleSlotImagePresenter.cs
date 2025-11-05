using System.Collections;
using System.Collections.Generic;
using Backend.Object.UI.View;
using Backend.Util.Data;
using Backend.Util.Presentation;
using Backend.Util.Presentation.Message;
using UnityEngine;

namespace Backend.Object.UI.Presenter
{
    public class BattleSlotImagePresenter : Presenter<BattleSlotView, Inventory>
    {
        private Sprite _lastSprite;
        private BattleSlotType _slotType;
        public BattleSlotImagePresenter(BattleSlotView view, Inventory model) : base(view, model)
        {
            view.OnSlotAction += ChangeImage;
            _slotType = View.SlotType;

            InitImage();
        }

        private void InitImage()
        {
            if (Model == null)
            {
                _lastSprite = null;
                return;
            }

            var sprite = Model.items[0]?.IconImage;
            _lastSprite = sprite;
            View.Change(sprite);
        }

        public void ChangeImage()
        {
            if (Model == null)
            {
                if (_lastSprite != null)
                {
                    _lastSprite = null;
                    View.Change(null);
                }
                return;
            }

            Sprite targetSprite = null;
            switch (_slotType)
            {
                case BattleSlotType.RightWeaponSlot:
                    targetSprite = Model.items[0]?.IconImage;
                    break;
                case BattleSlotType.LeftWeaponSlot:
                    targetSprite = Model.items[3]?.IconImage;
                    break;
                case BattleSlotType.ConsumeItemSlot:
                    targetSprite = Model.items[0]?.IconImage;
                    break;
            }

            if (ReferenceEquals(_lastSprite, targetSprite))
            {
                return;
            }

            _lastSprite = targetSprite;
            View.Change(targetSprite);
        }

        public override void Clear()
        {
            View.OnSlotAction -= ChangeImage;
            base.Clear();
        }
    }
}
