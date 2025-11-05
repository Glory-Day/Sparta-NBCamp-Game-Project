using Backend.Object.UI.View;
using Backend.Util.Data;
using Backend.Util.Debug;
using Backend.Util.Presentation;
using Backend.Util.Presentation.Message;
using UnityEditor.PackageManager.Requests;

namespace Backend.Object.UI.Presenter
{
    public class PlayerInventoryPresenter : BaseInventoryPresenter
    {
        private int _index;
        private InventoryType _type;
        public PlayerInventoryPresenter(EquipmentView view, Inventory model, Dispatcher dispatcher) : base(view, model, dispatcher)
        {
            view.EquipAction += OnEquip;
        }

        public override void Receive<T>(T message)
        {
            switch (message)
            {
                case ItemDataMessage msg:
                    _index = msg.ItemIndex;
                    _type = msg.InvType;
                    break;
            }
        }

        private void OnEquip(int slotIndex)
        {
            //다시 눌렀을때
            var sourceItem = _repository.GetItemData(slotIndex);
            if (sourceItem == null)
            {
                Debugger.LogMessage($"빈칸 클릭으로 돌아가기");
                _dispatcher.DispatchTo<LayoutGroupPresenter, LayoutActiveMessage>(0, new LayoutActiveMessage(true, LayoutType.EquipInventory));
                _dispatcher.DispatchTo<LayoutGroupPresenter, LayoutActiveMessage>(1, new LayoutActiveMessage(false, LayoutType.PlayerInventory));
                return;
            }

            //장착 가능한지 확인
            bool isActionValid = false;
            switch (_type) // _type 목적지 슬롯 타입
            {
                case InventoryType.WeaponInventory:
                    if (sourceItem is WeaponItemData)
                    {
                        isActionValid = true;
                    }
                    break;
                case InventoryType.ArmorInventory:
                    if (sourceItem is ArmorItemData)
                    {
                        isActionValid = true;
                    }
                    break;
                case InventoryType.ConsumableInventory:
                    if (sourceItem is ConsumableItemData)
                    {
                        isActionValid = true;
                    }
                    break;
            }

            bool isEquipment = sourceItem is WeaponItemData || sourceItem is ArmorItemData;

            _dispatcher.DispatchTo<LayoutGroupPresenter, LayoutActiveMessage>(2, new LayoutActiveMessage(!isEquipment, LayoutType.ConsumeInfo));
            _dispatcher.DispatchTo<LayoutGroupPresenter, LayoutActiveMessage>(3, new LayoutActiveMessage(isEquipment, LayoutType.EquipInfo));

            if (!isActionValid)
            {
                Debugger.LogMessage($"아이템 '{sourceItem.Name}'은(는) 해당 슬롯에 장착할 수 없습니다.");
                return;
            }

            _dispatcher.DispatchTo<LayoutGroupPresenter, LayoutActiveMessage>(4, new LayoutActiveMessage(true, LayoutType.Popup));
            _dispatcher.DispatchTo<ButtonPresenter, ItemDataMessage>(0, new ItemDataMessage(_index, _type, sourceItem));
        }
    }
}
