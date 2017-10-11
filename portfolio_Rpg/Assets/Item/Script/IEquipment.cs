namespace Rpg.Item
{
    interface IEquipment
    {
        void OnEquip(Item item);
        void OnUnequip(Item item);
    }
}
