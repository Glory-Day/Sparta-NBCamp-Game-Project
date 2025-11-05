using System;
using System.Dynamic;
using Backend.Util.Data;
using Backend.Util.Debug;
using Backend.Util.Presentation;
using Backend.Util.Presentation.Message;
using static UnityEditor.Progress;

namespace Backend.Object.UI.Presenter
{
    public abstract class BaseInventoryPresenter : Presenter<BaseInventoryView, Inventory>, ISubscriber
    {
        protected InventoryRepository _repository;
        protected Dispatcher _dispatcher;
        public BaseInventoryPresenter(BaseInventoryView view, Inventory model, Dispatcher dispatcher) : base(view, model)
        {
            _repository = new(view, model);

            _dispatcher = dispatcher;
            _dispatcher.Subscribe(this);

            View.RemoveAction += _repository.Remove;
            View.InfoAction += OnAccept;
            View.UpdateAction += OnUpdate;
        }

        public override void Clear()
        {
            _dispatcher.Unsubscribe(this);

            View.RemoveAction += _repository.Remove;
            View.InfoAction += OnAccept;
            View.UpdateAction += OnUpdate;
        }

        public abstract void Receive<T>(T message);

        public virtual void OnUpdate()
        {
            Reset(0);
            _repository.UpdateAllSlot();
            UpdateStatusText();
        }

        protected virtual void OnAccept(int slotIndex)
        {
            var index = slotIndex;
            var item = Model.items[index];

            Reset(0);

            //공통
            var commonMessage = new InventoryPointMessage(index, item.ID);
            _dispatcher.DispatchTo<ItemNamePresenter, InventoryPointMessage>(0, commonMessage);
            _dispatcher.DispatchTo<ImagePresenter, InventoryPointMessage>(0, commonMessage);

            switch (item)
            {
                case ConsumableItemData consumable:
                    _dispatcher.DispatchTo<ItemDescriptPresenter, InventoryPointMessage>(0, new InventoryPointMessage(index, item.ID));
                    _dispatcher.DispatchTo<ItemCountDifferenceTextPresenter, InventoryPointMessage>(0, new InventoryPointMessage(index, item.Count));
                    break;
                case WeaponItemData weapon when weapon.IsEquipped == false:
                    _dispatcher.DispatchTo<PhysicalPointDifferenceTextPresenter, InventoryPointMessage>(0, new InventoryPointMessage(index, (int)weapon.PhysicalDamagePoint));
                    _dispatcher.DispatchTo<MagicalPointDifferenceTextPresenter, InventoryPointMessage>(0, new InventoryPointMessage(index, (int)weapon.MagicalDamagePoint));
                    break;
                case ArmorItemData armor when armor.IsEquipped == false:
                    _dispatcher.DispatchTo<PhysicalDefenseDifferenceTextPresenter, InventoryPointMessage>(0, new InventoryPointMessage(index, (int)armor.PhysicalDefensePoint));
                    _dispatcher.DispatchTo<MagicalDefenseDifferenceTextPresenter, InventoryPointMessage>(0, new InventoryPointMessage(index, (int)armor.MagicalDefensePoint));
                    break;
            }
        }

        private void UpdateStatusText()
        {
            _dispatcher.DispatchTo<StatusTextPresenter, StatusPointMessage>(0, new StatusPointMessage(StatusType.Level));
            _dispatcher.DispatchTo<StatusTextPresenter, StatusPointMessage>(1, new StatusPointMessage(StatusType.Life));
            _dispatcher.DispatchTo<StatusTextPresenter, StatusPointMessage>(2, new StatusPointMessage(StatusType.Concentration));
            _dispatcher.DispatchTo<StatusTextPresenter, StatusPointMessage>(3, new StatusPointMessage(StatusType.Endurance));
            _dispatcher.DispatchTo<StatusTextPresenter, StatusPointMessage>(4, new StatusPointMessage(StatusType.Health));
            _dispatcher.DispatchTo<StatusTextPresenter, StatusPointMessage>(5, new StatusPointMessage(StatusType.Strength));
            _dispatcher.DispatchTo<StatusTextPresenter, StatusPointMessage>(6, new StatusPointMessage(StatusType.Intelligence));
            _dispatcher.DispatchTo<StatusTextPresenter, StatusPointMessage>(7, new StatusPointMessage(StatusType.Luck));
            _dispatcher.DispatchTo<StatusTextPresenter, StatusPointMessage>(8, new StatusPointMessage(StatusType.MaximumHealth));
            _dispatcher.DispatchTo<StatusTextPresenter, StatusPointMessage>(9, new StatusPointMessage(StatusType.MaximumStamina));
        }

        private void Reset(int index)
        {
            //사용하면 모든 _dispatcher Presenter에게 해당 메세지를 보냄
            //_dispatcher.DispatchToAll<InventoryPointMessage>(new InventoryPointMessage(index, 0));

            _dispatcher.DispatchTo<ItemDescriptPresenter, InventoryPointMessage>(0, new InventoryPointMessage(index, 0));
            _dispatcher.DispatchTo<ItemCountDifferenceTextPresenter, InventoryPointMessage>(0, new InventoryPointMessage(index, 0));

            _dispatcher.DispatchTo<PhysicalPointDifferenceTextPresenter, InventoryPointMessage>(0, new InventoryPointMessage(index, 0));
            _dispatcher.DispatchTo<MagicalPointDifferenceTextPresenter, InventoryPointMessage>(0, new InventoryPointMessage(index, 0));

            _dispatcher.DispatchTo<PhysicalDefenseDifferenceTextPresenter, InventoryPointMessage>(0, new InventoryPointMessage(index, 0));
            _dispatcher.DispatchTo<MagicalDefenseDifferenceTextPresenter, InventoryPointMessage>(0, new InventoryPointMessage(index, 0));
        }
    }
}
