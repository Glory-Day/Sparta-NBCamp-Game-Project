using Backend.Object.UI.View;
using Backend.Util.Data;
using Backend.Util.Presentation;
using Backend.Util.Presentation.Message;

namespace Backend.Object.UI.Presenter
{
    public class EquipmentInventoryPresenter : BaseInventoryPresenter
    {
        public EquipmentInventoryPresenter(EquipmentView view, Inventory model, Dispatcher dispatcher) : base(view, model, dispatcher)
        {
            view.EquipAction += OnEquip;
        }

        public override void Receive<T>(T message)
        {
            switch (message)
            {
                case ItemDataMessage msg when msg.InvType == View.inventoryType:
                    if(msg.ItemIndex != -1)
                    {
                        //아이템 장착
                        EquipSwap(msg.ItemIndex, msg.Item);
                        _repository.UpdateAllSlot();
                    }
                    else
                    {
                        //아이템 장착 해제
                        _repository.FindRemoveItem(msg.Item);
                    }
                    break;
            }
            _dispatcher.DispatchTo<LayoutGroupPresenter, LayoutActiveMessage>(0, new LayoutActiveMessage(true, LayoutType.EquipInventory));
            _dispatcher.DispatchTo<LayoutGroupPresenter, LayoutActiveMessage>(1, new LayoutActiveMessage(false, LayoutType.PlayerInventory));
        }

        private void OnEquip(int slotIndex)
        {
            var index = slotIndex;
            var item = Model.items[index];

            bool isEquipment = item is WeaponItemData || item is ArmorItemData;

            _dispatcher.DispatchTo<LayoutGroupPresenter, LayoutActiveMessage>(2, new LayoutActiveMessage(!isEquipment, LayoutType.ConsumeInfo));
            _dispatcher.DispatchTo<LayoutGroupPresenter, LayoutActiveMessage>(3, new LayoutActiveMessage(isEquipment, LayoutType.EquipInfo));

            _dispatcher.DispatchTo<PlayerInventoryPresenter, ItemDataMessage>(0, new ItemDataMessage(slotIndex, View.inventoryType));
            _dispatcher.DispatchTo<LayoutGroupPresenter, LayoutActiveMessage>(0, new LayoutActiveMessage(false, LayoutType.EquipInventory));
            _dispatcher.DispatchTo<LayoutGroupPresenter, LayoutActiveMessage>(1, new LayoutActiveMessage(true, LayoutType.PlayerInventory));
        }

        private void EquipSwap(int index, ItemData item)
        {
            //슬롯에 장착되었던 아이템
            var oldItem = _repository.GetItemData(index);

            if (oldItem != null)
            {
                if (oldItem is EquipableItemData oldEquipableItem)
                {
                    oldEquipableItem.IsEquipped = false;
                }
            }

            //버튼으로 알림을 보내면 어떨까
            _dispatcher.DispatchTo<ButtonPresenter, ItemStatDataMessage>(0, new ItemStatDataMessage(oldItem, EquipState.UnEquip));

            Model.items[index] = item;
        }
    }
}
