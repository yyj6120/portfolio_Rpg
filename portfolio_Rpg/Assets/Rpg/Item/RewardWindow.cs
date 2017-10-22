using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Text;

namespace Rpg.Item
{
    public class RewardWindow : MonoBehaviour
    { 
        public List<Item> items;
        public ItemListData itemListData;
        public ItemCollection itemCollection;
        [HideInInspector]
        public ItemSlot currentSelectedSlot;
        public List<ItemSlot> slots;
        public ItemSlot rewardPrefab;
        public RectTransform contentWindow;
        private OnSubmitSlot onSubmitSlot;
        private OnSelectSlot onSelectSlot;
        private ItemManager itemManager;
        public Button closeWindow;
        private void LateUpdate()
        {
            if(items.Count <= 0)
            {
                itemCollection.currentRewardWindow = null;
                itemCollection.OffAction();
                Destroy(gameObject);
            }
        }
        public void CreateRewardWindow()
        {
            if (items.Count == 0)
            {
                if (slots.Count > 0)
                {
                    for (int i = 0; i < slots.Count; i++)
                    {
                        Destroy(slots[i].gameObject);
                    }
                    slots.Clear();
                }
                return;
            }

            if (slots == null)
                slots = new List<ItemSlot>();

            var count = slots.Count;
            if (count < items.Count)
            {
                for (int i = count; i < items.Count; i++)
                {
                    var slot = Instantiate(rewardPrefab) as ItemSlot;
                    slots.Add(slot);
                }
            }
            else if (count > items.Count)
            {
                for (int i = count - 1; i > items.Count - 1; i--)
                {
                    Destroy(slots[slots.Count - 1].gameObject);
                    slots.RemoveAt(slots.Count - 1);

                }
            }

            count = slots.Count;
            for (int i = 0; i < items.Count; i++)
            {
                ItemSlot slot = null;
                if (i < items.Count)
                {
                    slot = slots[i];
                    slot.AddItem(items[i]);
                    slot.CheckItem(items[i].isInEquipArea);
                    slot.onSubmitSlotCallBack = OnSubmit;
                    var rectTranform = slot.GetComponent<RectTransform>();
                    rectTranform.SetParent(contentWindow);
                    rectTranform.localPosition = Vector3.zero;
                    rectTranform.localScale = Vector3.one;
                }
            }

        }

        public void CollectItem(List<ItemReference> collection, ItemManager itemManager , bool immediate = false)
        {
            this.itemManager = itemManager;
            foreach (ItemReference reference in collection)
            {
                AddItem(reference, immediate);
            }
        }

        public void AddItem(ItemReference itemReference, bool immediate = false)
        {
            if (itemReference != null && itemListData != null && itemListData.items.Count > 0)
            {
                var item = itemListData.items.Find(t => t.id.Equals(itemReference.id));
                if (item)
                {
                    var sameItems = items.FindAll(i => i.stackable && i.id == item.id && i.amount < i.maxStack);
                    if (sameItems.Count == 0)
                    {
                        var _item = Instantiate(item);
                        _item.name = _item.name.Replace("(Clone)", string.Empty);
                        if (itemReference.attributes != null && _item.attributes != null && item.attributes.Count == itemReference.attributes.Count)
                            _item.attributes = new List<ItemAttribute>(itemReference.attributes);
                        _item.amount = 0;
                        for (int i = 0; i < item.maxStack && _item.amount < _item.maxStack && itemReference.amount > 0; i++)
                        {
                            _item.amount++;
                            itemReference.amount--;
                        }
                        items.Add(_item);
                       // onAddItem.Invoke(_item);

                        if (itemReference.amount > 0)
                            AddItem(itemReference);
                    }
                    else
                    {
                        var indexOffItem = items.IndexOf(sameItems[0]);

                        for (int i = 0; i < items[indexOffItem].maxStack && items[indexOffItem].amount < items[indexOffItem].maxStack && itemReference.amount > 0; i++)
                        {
                            items[indexOffItem].amount++;
                            itemReference.amount--;
                        }
                        if (itemReference.amount > 0)
                            AddItem(itemReference);
                    }
                }
            }
        }

        public void OnSubmit(ItemSlot slot)
        {
            currentSelectedSlot = slot;
            if (slot.item)
            {
                var rect = slot.GetComponent<RectTransform>();
                currentSelectedSlot = slot;
            }    
            foreach(var collectionItem in itemCollection.items)
            {
                if(collectionItem.id == slot.item.id)
                {
                    itemCollection.items.Remove(collectionItem);
                    break;
                }
            }
            itemManager.items.Add(slot.item);
            items.Remove(slot.item);
            Destroy(slot.gameObject);
        }

        public void OnSelectSlot(ItemSlot slot)
        {
            currentSelectedSlot = slot;
        }

    }
}
