using Backend.Util.Data;
using Backend.Util.Debug;
using Backend.Util.Presentation;
using Backend.Util.Presentation.Message;

namespace Backend.Object.UI.Presenter
{
    public class InventoryPresenter : Presenter<InventoryView, Inventory>, ISubscriber
    {
        private InventoryRepository _repository;
        private Dispatcher _dispatcher;
        public InventoryPresenter(InventoryView view, Inventory model, Dispatcher dispatcher) : base(view, model)
        {
            _repository = new(view, model);

            _dispatcher = dispatcher;
            _dispatcher.Subscribe(this);

            View.RemoveAction += _repository.Remove;
            View.SwapAction += _repository.Swap;
            View.InfoAction += OnAccept;
            View.UpdateAction += OnUpdate;

            Reset(0);
        }

        public void Receive<T>(T message)
        {

        }

        public void OnUpdate()
        {
            _repository.UpdateAllSlot();
        }

        public void OnAccept(int slotIndex)
        {
            var index = slotIndex;
            var item = Model.items[index];

            Reset(index);

            bool isEquipment = item is WeaponItemData || item is ArmorItemData;
            _dispatcher.DispatchTo<EquipmentLayoutGroupPresenter, LayoutActiveMessage>(new LayoutActiveMessage(isEquipment));
            _dispatcher.DispatchTo<ConsumeLayoutGroupPresenter, LayoutActiveMessage>(new LayoutActiveMessage(!isEquipment));

            switch (item)
            {
                case ConsumableItemData consumable:
                    Debugger.LogMessage("사용 아이템");
                    _dispatcher.DispatchTo<ItemDescriptPresenter, InventoryPointMessage>(new InventoryPointMessage(index, item.ID));
                    _dispatcher.DispatchTo<ItemCountDifferenceTextPresenter, InventoryPointMessage>(new InventoryPointMessage(index, item.Count));
                    //_dispatcher.DispatchTo<LifePointDifferenceTextPresenter, InventoryPointMessage>(new InventoryPointMessage(index, (int)consumable.LifePoint));
                    //_dispatcher.DispatchTo<EndurancePointDifferenceTextPresenter, InventoryPointMessage>(new InventoryPointMessage(index, (int)consumable.EndurancePoint));
                    break;
                case WeaponItemData weapon:
                    _dispatcher.DispatchTo<PhysicalPointDifferenceTextPresenter, InventoryPointMessage>(new InventoryPointMessage(index, (int)weapon.PhysicalDamagePoint));
                    _dispatcher.DispatchTo<MagicalPointDifferenceTextPresenter, InventoryPointMessage>(new InventoryPointMessage(index, (int)weapon.MagicalDamagePoint));
                    break;
                case ArmorItemData armor:
                    _dispatcher.DispatchTo<PhysicalDefenceDifferenceTextPresenter, InventoryPointMessage>(new InventoryPointMessage(index, (int)armor.PhysicalDefensePoint));
                    _dispatcher.DispatchTo<MagicalDefenceDifferenceTextPresenter, InventoryPointMessage>(new InventoryPointMessage(index, (int)armor.MagicalDefensePoint));
                    break;
            }

            //공통
            var commonMessage = new InventoryPointMessage(index, item.ID);
            _dispatcher.DispatchTo<ItemNamePresenter, InventoryPointMessage>(commonMessage);
            _dispatcher.DispatchTo<ImagePresenter, InventoryPointMessage>(commonMessage);
        }

        private void Reset(int index)
        {
            _dispatcher.DispatchToAll<InventoryPointMessage>(new InventoryPointMessage(index, 0), false);
        }
    }
}
