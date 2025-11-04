using System.Collections;
using System.Collections.Generic;
using Backend.Object.Character.Player;
using Backend.Object.UI.View;
using Backend.Util.Data;
using Backend.Util.Debug;
using Backend.Util.Presentation;
using Backend.Util.Presentation.Message;
using UnityEngine;

namespace Backend.Object.UI.Presenter
{
    public class ButtonPresenter : Presenter<ButtonView, PlayerStatus>, ISubscriber
    {
        private Dispatcher _dispatcher;
        private InventoryType _type;
        private int _index;
        private ItemData _item;
        public ButtonPresenter(ButtonView view, PlayerStatus model, Dispatcher dispatcher) : base(view, model)
        {
            _dispatcher = dispatcher;
            _dispatcher.Subscribe(this);

            View.ConfirmButton.onClick.AddListener(() => OnAccept());
            View.CancelButton.onClick.AddListener(() => OnDeny());
        }

        public override void Clear()
        {
            _dispatcher.Unsubscribe(this);
        }

        private void OnAccept()
        {
            Debugger.LogSuccess("Button OnAccept Good");
            //모델에다가 해당 아이템이 장착중이 아니면 데이터를 적용한다.
            if(_item is EquipableItemData equipableItem && equipableItem.IsEquipped == false)
            {
                ApplyStats(_item);
            }

            switch (_type)
            {
                case InventoryType.WeaponInventory:
                    if (_item is WeaponItemData weapon)
                    {
                        if (weapon.IsEquipped == false)
                        {
                            weapon.IsEquipped = true;
                            _dispatcher.DispatchTo<EquipmentInventoryPresenter, ItemDataMessage>(0, new ItemDataMessage(_index, _type, _item));
                        }
                        else
                        {
                            UnApplyStats(_item);
                            weapon.IsEquipped = false;
                            _dispatcher.DispatchTo<EquipmentInventoryPresenter, ItemDataMessage>(0, new ItemDataMessage(-1, _type, _item));
                        }
                    }
                    break;
                case InventoryType.ArmorInventory:
                    if (_item is ArmorItemData armor)
                    {
                        if (armor.IsEquipped == false)
                        {
                            armor.IsEquipped = true;
                            _dispatcher.DispatchTo<EquipmentInventoryPresenter, ItemDataMessage>(1, new ItemDataMessage(_index, _type, _item));
                        }
                        else
                        {
                            UnApplyStats(_item);
                            armor.IsEquipped = false;
                            _dispatcher.DispatchTo<EquipmentInventoryPresenter, ItemDataMessage>(1, new ItemDataMessage(-1, _type, _item));
                        }
                    }
                    break;
                case InventoryType.ConsumableInventory when _item is ConsumableItemData consume:
                    _dispatcher.DispatchTo<EquipmentInventoryPresenter, ItemDataMessage>(2, new ItemDataMessage(_index, _type, _item));
                    break;
            }

            _dispatcher.DispatchTo<LayoutGroupPresenter, LayoutActiveMessage>(4, new LayoutActiveMessage(false, LayoutType.Popup));
        }

        private void OnDeny()
        {
            //UIPopup 끄기
            _dispatcher.DispatchTo<LayoutGroupPresenter, LayoutActiveMessage>(4, new LayoutActiveMessage(false, LayoutType.Popup));
        }

        public void Receive<T>(T message)
        {
            //특정 아이템 데이터 가져와야할듯?, 스텟 적용 메세지
            if (message is ItemDataMessage itemDataMessage)
            {
                _index = itemDataMessage.ItemIndex;
                _type = itemDataMessage.InvType;
                _item = itemDataMessage.Item;
            }

            if (message is ItemStatDataMessage itemStatDataMessage)
            {
                if (itemStatDataMessage.EquipState == EquipState.UnEquip)
                {
                    UnApplyStats(itemStatDataMessage.Item);
                }
            }
        }

        private void ApplyStats(ItemData item)
        {
            if (item == null)
            {
                return;
            }

            if (item is WeaponItemData weapon)
            {
                Model.data.PhysicalDamage += weapon.PhysicalDamagePoint;
                Model.data.MagicalDamage += weapon.MagicalDamagePoint;
            }
            else if (item is ArmorItemData armor)
            {
                Model.data.PhysicalDefense += armor.PhysicalDefensePoint;
                Model.data.MagicalDefense += armor.MagicalDefensePoint;
            }
        }

        private void UnApplyStats(ItemData item)
        {
            if (item == null)
            {
                return;
            }

            if (item is WeaponItemData weapon)
            {
                Model.data.PhysicalDamage -= weapon.PhysicalDamagePoint;
                Model.data.MagicalDamage -= weapon.MagicalDamagePoint;
            }
            else if (item is ArmorItemData armor)
            {
                Model.data.PhysicalDefense -= armor.PhysicalDefensePoint;
                Model.data.MagicalDefense -= armor.MagicalDefensePoint;
            }
        }
    }
}
