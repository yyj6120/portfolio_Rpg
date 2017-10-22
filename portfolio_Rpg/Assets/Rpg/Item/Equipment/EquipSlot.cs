using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Rpg.Item
{
    public class EquipSlot : ItemSlot
    {
        [Header("--- Item Type ---")]
        public List<ItemType> itemType;

        public override void AddItem(Item item)
        {
            if (item) item.isInEquipArea = true;
            base.AddItem(item);
            onAddItem.Invoke(item);
        }

        public override void RemoveItem()
        {
            onRemoveItem.Invoke(item);
            if (item != null) item.isInEquipArea = false;
            base.RemoveItem();
        }
    }
}