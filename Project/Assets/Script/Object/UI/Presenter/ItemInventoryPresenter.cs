using System.Collections;
using System.Collections.Generic;
using Backend.Object.UI.View;
using Backend.Util.Data;
using Backend.Util.Presentation;
using Backend.Util.Presentation.Message;
using UnityEngine;

namespace Backend.Object.UI.Presenter
{
    public class ItemInventoryPresenter : BaseInventoryPresenter
    {
        public ItemInventoryPresenter(ItemInventoryView view, Inventory model, Dispatcher dispatcher) : base(view, model, dispatcher)
        {
            view.SwapAction += _repository.Swap;
            view.EquipAction += OnEquip;
        }

        public override void Receive<T>(T message)
        {
        }

        protected override void OnAccept(int slotIndex)
        {
            base.OnAccept(slotIndex);
        }

        private void OnEquip(int slotIndex)
        {
            var index = slotIndex;
            var item = Model.items[index];

            bool isEquipment = item is WeaponItemData || item is ArmorItemData;

            _dispatcher.DispatchTo<LayoutGroupPresenter, LayoutActiveMessage>(0, new LayoutActiveMessage(!isEquipment, LayoutType.ConsumeInfo));
            _dispatcher.DispatchTo<LayoutGroupPresenter, LayoutActiveMessage>(1, new LayoutActiveMessage(isEquipment, LayoutType.EquipInfo));
        }
    }
}
