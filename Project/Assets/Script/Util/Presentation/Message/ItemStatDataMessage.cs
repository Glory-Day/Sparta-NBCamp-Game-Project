using Backend.Util.Data;

namespace Backend.Util.Presentation.Message
{
    public enum EquipState
    {
        None,
        Equip,
        UnEquip,
    }
    public struct ItemStatDataMessage
    {
        public ItemData Item;
        public EquipState EquipState;
        public ItemStatDataMessage(ItemData item, EquipState equipState)
        {
            Item = item;
            EquipState = equipState;
        }
    }
}
