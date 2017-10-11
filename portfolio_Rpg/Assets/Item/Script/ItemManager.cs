using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using Rpg.Character;
#if UNITY_EDITOR
using UnityEditor.Events;
#endif

namespace Rpg.Item
{
    public class ItemManager : MonoBehaviour
    {
        public string collectableTag = "Collectable";
        public bool dropItemsWhenDead;
        public GenericInput actionInput = new GenericInput("E", "A", "A");
        public GenericInput actionInput1 = new GenericInput("Fire1");
        public Inventory inventoryPrefab;

        [HideInInspector]
        public Inventory inventory;
        public ItemListData itemListData;
        [Header("---Items Filter---")]
        public List<ItemType> itemsFilter = new List<ItemType>() { 0 };

        #region SerializedProperties in Custom Editor
        [SerializeField]
        public List<ItemReference> startItems = new List<ItemReference>();

        public List<Item> items;
        public OnHandleItemEvent onUseItem, onAddItem;
        public OnChangeItemAmount onLeaveItem, onDropItem;
        public OnOpenCloseInventory onOpenCloseInventory;
        public OnChangeEquipmentEvent onEquipItem, onUnequipItem;
        [SerializeField]
        public List<EquipPoint> equipPoints;
        [SerializeField]
        public List<ApplyAttributeEvent> applyAttributeEvents;
        #endregion
        [HideInInspector]
        public bool inEquip;
        private float equipTimer;
        private Animator animator;
        private ThirdPersonInput tpInput;
        private static ItemManager instance;

        private void OnEnable()
        {
            Test(this, this.GetComponent<MeleeManager>());
        }

        IEnumerator Start()
        {


            if (instance == null)
            {
                inventory = FindObjectOfType<Inventory>();
                instance = this;

                if (!inventory && inventoryPrefab)
                    inventory = Instantiate(inventoryPrefab);

                if (!inventory) Debug.LogError("No vInventory assigned!");

                if (inventory)
                {
                    inventory.GetItemsHandler = GetItems;
                    inventory.onEquipItem.AddListener(EquipItem);
                    inventory.onUnequipItem.AddListener(UnequipItem);
                    inventory.onDropItem.AddListener(DropItem);
                    inventory.onLeaveItem.AddListener(LeaveItem);
                    inventory.onUseItem.AddListener(UseItem);
                    inventory.onOpenCloseInventory.AddListener(OnOpenCloseInventory);
                }
                animator = GetComponent<Animator>();
                tpInput = GetComponent<ThirdPersonInput>();

                if (dropItemsWhenDead)
                {
                    var character = GetComponent<Rpg.Character.Character>();
                    if (character)
                        character.onDead.AddListener(DropAllItens);
                }

                if (tpInput)
                    tpInput.onUpdateInput.AddListener(UpdateInput);

                var genericAction = GetComponent<GenericWarriorAction>();
                if (genericAction != null)
                    genericAction.OnDoAction.AddListener(CollectItem);

                yield return new WaitForEndOfFrame();
                items = new List<Item>();
                if (itemListData)
                {
                    for (int i = 0; i < startItems.Count; i++)
                    {
                        AddItem(startItems[i], true);
                    }
                }
            }
        }

        public List<Item> GetItems()
        {
            return items;
        }

        /// <summary>
        /// Add new Instance of Item to itemList
        /// </summary>
        /// <param name="item"></param>
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
                        onAddItem.Invoke(_item);

                        if (itemReference.autoEquip)
                        {
                            itemReference.autoEquip = false;
                            AutoEquipItem(_item, itemReference.indexArea, immediate);
                        }

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

        public void UseItem(Item item)
        {
            if (item)
            {
                onUseItem.Invoke(item);
                if (item.attributes != null && item.attributes.Count > 0 && applyAttributeEvents.Count > 0)
                {
                    foreach (ApplyAttributeEvent attributeEvent in applyAttributeEvents)
                    {
                        var attributes = item.attributes.FindAll(a => a.name.Equals(attributeEvent.attribute));
                        foreach (ItemAttribute attribute in attributes)
                            attributeEvent.onApplyAttribute.Invoke(attribute.value);
                    }
                }
                if (item.amount <= 0 && items.Contains(item)) items.Remove(item);
            }
        }

        public void LeaveItem(Item item, int amount)
        {
            onLeaveItem.Invoke(item, amount);
            item.amount -= amount;
            if (item.amount <= 0 && items.Contains(item))
            {
                if (item.type != ItemType.Consumable)
                {
                    var equipPoint = equipPoints.Find(ep => ep.equipmentReference.item == item || ep.area != null && ep.area.ValidSlots.Find(slot => slot.item == item));
                    if (equipPoint != null)
                        if (equipPoint.area) equipPoint.area.RemoveItem(item);
                }
                items.Remove(item);
                Destroy(item);
            }
        }

        public void DropItem(Item item, int amount)
        {

            item.amount -= amount;
            if (item.dropObject != null)
            {
                var dropObject = Instantiate(item.dropObject, transform.position + (Vector3.up * 0.05f), item.dropObject.transform.rotation) as GameObject;
                ItemCollection collection = dropObject.GetComponent<ItemCollection>();
                if (collection != null)
                {
                    collection.items.Clear();
                    var itemReference = new ItemReference(item.id);
                    itemReference.amount = amount;
                    itemReference.attributes = new List<ItemAttribute>(item.attributes);
                    collection.items.Add(itemReference);
                }
            }
            onDropItem.Invoke(item, amount);
            if (item.amount <= 0 && items.Contains(item))
            {
                if (item.type != ItemType.Consumable)
                {
                    var equipPoint = equipPoints.Find(ep => ep.equipmentReference.item == item || ep.area != null && ep.area.ValidSlots.Find(slot => slot.item == item));
                    if (equipPoint != null)
                        if (equipPoint.area) equipPoint.area.RemoveItem(item);
                }
                items.Remove(item);
                Destroy(item);
            }
        }

        public void DropAllItens(GameObject target = null)
        {
            if (target != null && target != gameObject) return;
            List<ItemReference> itemReferences = new List<ItemReference>();
            for (int i = 0; i < items.Count; i++)
            {
                if (itemReferences.Find(_item => _item.id == items[i].id) == null)
                {
                    var sameItens = items.FindAll(_item => _item.id == items[i].id);
                    ItemReference itemReference = new ItemReference(items[i].id);
                    for (int a = 0; a < sameItens.Count; a++)
                    {
                        if (sameItens[a].type != ItemType.Consumable)
                        {
                            var equipPoint = equipPoints.Find(ep => ep.equipmentReference.item == sameItens[a]);
                            if (equipPoint != null && equipPoint.equipmentReference.equipedObject != null)
                                UnequipItem(equipPoint.area, equipPoint.equipmentReference.item);
                        }

                        itemReference.amount += sameItens[a].amount;
                        Destroy(sameItens[a]);
                    }
                    itemReferences.Add(itemReference);
                    if (equipPoints != null)
                    {
                        var equipPoint = equipPoints.Find(e => e.equipmentReference != null && e.equipmentReference.item != null && e.equipmentReference.item.id == itemReference.id && e.equipmentReference.equipedObject != null);
                        if (equipPoint != null)
                        {
                            Destroy(equipPoint.equipmentReference.equipedObject);
                            equipPoint.equipmentReference = null;
                        }
                    }
                    if (items[i].dropObject)
                    {
                        var dropObject = Instantiate(items[i].dropObject, transform.position, transform.rotation) as GameObject;
                        ItemCollection collection = dropObject.GetComponent<ItemCollection>();
                        if (collection != null)
                        {
                            collection.items.Clear();
                            collection.items.Add(itemReference);
                        }
                    }
                }
            }
            items.Clear();
        }

        #region Check Item in List
        /// <summary>
        /// Check if Item List contains a  Item
        /// </summary>
        /// <param name="id">Item id</param>
        /// <returns></returns>
        public bool ContainItem(int id)
        {
            return items.Exists(i => i.id == id);
        }

        /// <summary>
        /// Check if the list contains a item with certain amount, or more
        /// </summary>
        /// <param name="id">Item id</param>
        /// <param name="amount">Item amount</param>
        /// <returns></returns>
        public bool ContainItem(int id, int amount)
        {
            var item = items.Find(i => i.id == id && i.amount >= amount);
            return item != null;
        }

        /// <summary>
        /// Check if the list contains a certain count of items, or more
        /// </summary>
        /// <param name="id">Item id</param>
        /// <param name="count">Item count</param>
        /// <returns></returns>
        public bool ContainItems(int id, int count)
        {
            var _items = items.FindAll(i => i.id == id);
            return _items != null && _items.Count >= count;
        }

        #endregion

        #region Get Item in List
        /// <summary>
        /// Get a single Item with same id
        /// </summary>
        /// <param name="id">Item id</param>
        /// <returns></returns>
        public Item GetItem(int id)
        {
            return items.Find(i => i.id == id);
        }

        /// <summary>
        /// Get All Items with same id
        /// </summary>
        /// <param name="id">Item id</param>
        /// <returns></returns>
        public List<Item> GetItems(int id)
        {
            var _items = items.FindAll(i => i.id == id);
            return _items;
        }

        /// <summary>
        /// Ask if the Item is currently equipped
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsItemEquipped(int id)
        {
            return equipPoints.Exists(ep => ep.equipmentReference != null && ep.equipmentReference.item != null && ep.equipmentReference.item.id.Equals(id));
        }

        /// <summary>
        /// Get a specific Item on a specific EquipmentPoint
        /// </summary>
        /// <param name="equipPointName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsItemEquippedOnSpecificEquipPoint(string equipPointName, int id)
        {
            return equipPoints.Exists(ep => ep.equipPointName.Equals(equipPointName) && ep.equipmentReference != null && ep.equipmentReference.item != null && ep.equipmentReference.item.id.Equals(id));
        }

        #endregion

        public void EquipItem(EquipArea equipArea, Item item)
        {
            onEquipItem.Invoke(equipArea, item);

            if (item != equipArea.currentEquipedItem)
                return;

            var equipPoint = equipPoints.Find(ep => ep.equipPointName == equipArea.equipPointName);
            if (equipPoint != null && item != null && equipPoint.equipmentReference.item != item)
            {
                equipTimer = item.equipDelayTime;

                var type = item.type;
                if (type != ItemType.Consumable)
                {
                    if (!inventory.isOpen)
                    {
                        animator.SetInteger("EquipItemID", equipArea.equipPointName.Contains("Right") ? item.EquipID : -item.EquipID);
                        animator.SetTrigger("EquipItem");
                    }
                    equipPoint.area = equipArea;
                    StartCoroutine(EquipItemRoutine(equipPoint, item));
                }
            }
        }

        public void UnequipItem(EquipArea equipArea, Item item)
        {
            onUnequipItem.Invoke(equipArea, item);
            //if (item != equipArea.lastEquipedItem) return;
            var equipPoint = equipPoints.Find(ep => ep.equipPointName == equipArea.equipPointName && ep.equipmentReference.item != null && ep.equipmentReference.item == item);
            if (equipPoint != null && item != null)
            {
                equipTimer = item.equipDelayTime;
                var type = item.type;
                if (type != ItemType.Consumable)
                {
                    if (!inventory.isOpen && !inEquip)
                    {
                        animator.SetInteger("EquipItemID", equipArea.equipPointName.Contains("Right") ? item.EquipID : -item.EquipID);
                        animator.SetTrigger("EquipItem");
                    }
                    StartCoroutine(UnequipItemRoutine(equipPoint, item));
                }
            }
        }

        IEnumerator EquipItemRoutine(EquipPoint equipPoint, Item item)
        {
            if (!inEquip)
            {
                inventory.canEquip = false;
                inEquip = true;
                if (!inventory.isOpen)
                {
                    while (equipTimer > 0)
                    {
                        equipTimer -= Time.deltaTime;
                        if (item == null)
                            break;
                        yield return new WaitForEndOfFrame();
                    }
                }
                if (equipPoint != null)
                {
                    if (item.originalObject)
                    {
                        if (equipPoint.equipmentReference != null && equipPoint.equipmentReference.equipedObject != null)
                        {
                            var _equipment = equipPoint.equipmentReference.equipedObject.GetComponent<IEquipment>();
                            if (_equipment != null)
                                _equipment.OnUnequip(equipPoint.equipmentReference.item);
                            Destroy(equipPoint.equipmentReference.equipedObject);
                        }

                        var point = equipPoint.handler.customHandlers.Find(p => p.name == item.customEquipPoint);
                        var equipTransform = point != null ? point : equipPoint.handler.defaultHandler;
                        var equipedObject = Instantiate(item.originalObject, equipTransform.position, equipTransform.rotation) as GameObject;
                        equipedObject.transform.parent = equipTransform;

                        if (equipPoint.equipPointName.Contains("Left"))
                        {
                            var scale = equipedObject.transform.localScale;
                            scale.x *= -1;
                            equipedObject.transform.localScale = scale;
                        }

                        equipPoint.equipmentReference.item = item;
                        equipPoint.equipmentReference.equipedObject = equipedObject;
                        var equipment = equipedObject.GetComponent<IEquipment>();
                        if (equipment != null)
                            equipment.OnEquip(item);
                        equipPoint.onInstantiateEquiment.Invoke(equipedObject);
                    }
                    else if (equipPoint.equipmentReference != null && equipPoint.equipmentReference.equipedObject != null)
                    {
                        var _equipment = equipPoint.equipmentReference.equipedObject.GetComponent<IEquipment>();

                        if (_equipment != null)
                            _equipment.OnUnequip(equipPoint.equipmentReference.item);
                        Destroy(equipPoint.equipmentReference.equipedObject);
                        equipPoint.equipmentReference.item = null;
                    }
                }
                inEquip = false;
                inventory.canEquip = true;
                if (equipPoint != null)
                    CheckTwoHandItem(equipPoint, item);
            }
        }

        void CheckTwoHandItem(EquipPoint equipPoint, Item item)
        {
            if (item == null)
                return;

            var opposite = equipPoints.Find(ePoint => ePoint.area != null && ePoint.equipPointName.Equals("LeftArm") && ePoint.area.currentEquipedItem != null);
            if (equipPoint.equipPointName.Equals("LeftArm"))
                opposite = equipPoints.Find(ePoint => ePoint.area != null && ePoint.equipPointName.Equals("RightArm") && ePoint.area.currentEquipedItem != null);
            else if (!equipPoint.equipPointName.Equals("RightArm"))
            {
                return;
            }
            if (opposite != null && (item.twoHandWeapon || opposite.area.currentEquipedItem.twoHandWeapon))
            {
                opposite.area.RemoveCurrentItem();
            }
        }

        IEnumerator UnequipItemRoutine(EquipPoint equipPoint, Item item)
        {
            if (!inEquip)
            {
                inventory.canEquip = false;
                inEquip = true;
                if (equipPoint != null && equipPoint.equipmentReference != null && equipPoint.equipmentReference.equipedObject != null)
                {
                    var equipment = equipPoint.equipmentReference.equipedObject.GetComponent<IEquipment>();
                    if (equipment != null) equipment.OnUnequip(item);
                    if (!inventory.isOpen)
                    {
                        while (equipTimer > 0)
                        {
                            equipTimer -= Time.deltaTime;
                            yield return new WaitForEndOfFrame();
                        }
                    }
                    Destroy(equipPoint.equipmentReference.equipedObject);
                    equipPoint.equipmentReference.item = null;
                }
                inEquip = false;
                inventory.canEquip = true;
            }
        }

        void OnOpenCloseInventory(bool value)
        {
           // if (value)
              //  animator.SetTrigger("ResetState");
            onOpenCloseInventory.Invoke(value);
        }

        public void UpdateInput(MeleeCombatInput tpInput)
        {
            inventory.lockInput = tpInput.lockInventory;
            tpInput.lockInputByItemManager = inventory.isOpen || inEquip;
        }

        /// <summary>
        /// Equip item to specific area and specific slot
        /// </summary>
        /// <param name="indexOfArea">Index of Equip Area</param>
        /// <param name="indexOfSlot">Index of Slot in Equip Area</param>
        /// <param name="item">Item to Equip</param>
        /// <param name="immediate">Force immediate</param>
        public void EquipItemToEquipArea(int indexOfArea, int indexOfSlot, Item item, bool immediate = false)
        {
            if (!inventory) return;
            if (immediate)
                inventory.isOpen = immediate;
            if (inventory.equipAreas != null && indexOfArea < inventory.equipAreas.Length)
            {
                var area = inventory.equipAreas[indexOfArea];
                if (area != null)
                {
                    area.AddItemToEquipSlot(indexOfSlot, item);
                }
            }
            if (immediate)
                inventory.isOpen = false;
        }

        /// <summary>
        /// Unequip item of specific area and specific slot
        /// </summary>
        /// <param name="indexOfArea">Index of Equip Area</param>
        /// <param name="indexOfSlot">Index of Slot in Equip Area</param>
        /// <param name="immediate">For to unequip immediate</param>
        public void UnequipItemOfEquipArea(int indexOfArea, int indexOfSlot, bool immediate = false)
        {
            if (!inventory) return;
            if (immediate)
                inventory.isOpen = immediate;
            if (inventory.equipAreas != null && indexOfArea < inventory.equipAreas.Length)
            {
                var area = inventory.equipAreas[indexOfArea];
                if (area != null)
                {
                    area.RemoveItemOfEquipSlot(indexOfSlot);
                }
            }
            if (immediate)
                inventory.isOpen = false;
        }

        /// <summary>
        /// Equip or change Item to current equiped slot of area by equipPointName
        /// </summary>
        /// <param name="item">Item to equip</param>
        /// <param name="indexOfArea">Index of Equip area</param>
        /// <param name="immediate">Force equip Immediate</param>
        public void EquipCurrentItemToArea(Item item, int indexOfArea, bool immediate = false)
        {
            if (!inventory && items.Count == 0) return;

            if (immediate)
                inventory.isOpen = immediate;
            if (inventory.equipAreas != null && indexOfArea < inventory.equipAreas.Length)
            {
                inventory.equipAreas[indexOfArea].AddCurrentItem(item);
            }
            if (immediate)
                inventory.isOpen = false;
        }

        /// <summary>
        /// Unequip current equiped item of specific area 
        /// </summary>
        /// <param name="indexOfArea">Index of Equip area</param>
        /// <param name="immediate">Force unequip Immediate</param>
        public void UnequipCurrentEquipedItem(int indexOfArea, bool immediate = false)
        {
            if (!inventory && items.Count == 0) return;

            if (immediate)
                inventory.isOpen = immediate;
            if (inventory.equipAreas != null && indexOfArea < inventory.equipAreas.Length)
            {
                inventory.equipAreas[indexOfArea].RemoveCurrentItem();
            }
            if (immediate)
                inventory.isOpen = false;
        }

        /// <summary>
        /// Drop current equiped item of specific area
        /// </summary>
        /// <param name="indexOfArea">Index of Equip Area</param>
        /// <param name="immediate">Force to Drop immediate</param>
        public void DropCurrentEquipedItem(int indexOfArea, bool immediate = false)
        {
            if (!inventory && items.Count == 0) return;

            if (immediate)
                inventory.isOpen = immediate;
            if (inventory.equipAreas != null && indexOfArea < inventory.equipAreas.Length)
            {
                var item = inventory.equipAreas[indexOfArea].currentEquipedItem;
                if (item)
                    DropItem(item, item.amount);
            }

            if (immediate)
                inventory.isOpen = false;
        }

        /// <summary>
        /// Leave (Destroy) current equiped item of specific area
        /// </summary>
        /// <param name="indexOfArea">Index of Equip Area</param>
        /// <param name="immediate">Force to Leave immediate</param>
        public void LeaveCurrentEquipedItem(int indexOfArea, bool immediate = false)
        {
            if (!inventory && items.Count == 0) return;

            if (immediate)
                inventory.isOpen = immediate;
            if (inventory.equipAreas != null && indexOfArea < inventory.equipAreas.Length)
            {
                var item = inventory.equipAreas[indexOfArea].currentEquipedItem;
                if (item)
                    LeaveItem(item, item.amount);
            }

            if (immediate)
                inventory.isOpen = false;
        }

        /// <summary>
        /// Auto equip Item
        /// Ps: If item type doesn't match to any area or slot 
        /// </summary>
        /// <param name="item">Item to equip</param>
        /// <param name="immediate">Force equip immediate</param>
        public void AutoEquipItem(Item item, int indexArea, bool immediate = false)
        {
            if (!inventory) return;
            if (immediate)
                inventory.isOpen = immediate;
            if (inventory.equipAreas != null && inventory.equipAreas.Length > 0 && indexArea < inventory.equipAreas.Length)
            {
                var validSlot = inventory.equipAreas[indexArea].equipSlots.Find(slot => slot.isValid && slot.item == null && slot.itemType.Contains(item.type));
                if (validSlot)
                {
                    var indexOfSlot = inventory.equipAreas[indexArea].equipSlots.IndexOf(validSlot);
                    EquipItemToEquipArea(indexArea, indexOfSlot, item);
                }
            }
            else
            {
                Debug.LogWarning("Fail to auto equip " + item.name + " on equipArea " + indexArea);
            }

            if (immediate)
                inventory.isOpen = false;
        }

        #region Item Collector    

        public virtual void CollectItem(List<ItemReference> collection, bool immediate = false)
        {
            foreach (ItemReference reference in collection)
            {
                AddItem(reference, immediate);
            }
        }

        public virtual void CollectItem(TriggerGenericAction action)
        {
            var collection = action.GetComponentInChildren<ItemCollection>();
            if (collection != null)
            {
                collection.CollectItems(this);
            }
        }

        #endregion

        public void Test(ItemManager itemManager, MeleeManager meleeManager)
        {
            animator = itemManager.GetComponent<Animator>();
            if (itemManager.equipPoints == null)
                itemManager.equipPoints = new List<EquipPoint>();

            #region LeftEquipPoint
            var equipPointL = itemManager.equipPoints.Find(p => p.equipPointName == "LeftArm");
            if (equipPointL == null)
            {
                EquipPoint pointL = new EquipPoint();
                pointL.equipPointName = "LeftArm";
                if (meleeManager)
                {
                    pointL.onInstantiateEquiment.AddListener(meleeManager.SetLeftWeapon);

//#if UNITY_EDITOR
//                    UnityEventTools.AddPersistentListener<GameObject>(pointL.onInstantiateEquiment, meleeManager.SetLeftWeapon);
//#else
//                  pointL.onInstantiateEquiment.AddListener(manager.SetLeftWeapon);
//#endif
                }

                if (animator)
                {
                    var defaultEquipPointL = new GameObject("defaultEquipPoint");
                    var parent = animator.GetBoneTransform(HumanBodyBones.LeftHand);
                    defaultEquipPointL.transform.SetParent(parent);
                    defaultEquipPointL.transform.localPosition = Vector3.zero;
                    defaultEquipPointL.transform.forward = itemManager.transform.forward;
                    defaultEquipPointL.gameObject.tag = "Ignore Ragdoll";
                    pointL.handler = new Handler();
                    pointL.handler.defaultHandler = defaultEquipPointL.transform;
                }
                itemManager.equipPoints.Add(pointL);
            }
            else
            {
                if (equipPointL.handler.defaultHandler == null)
                {
                    if (animator)
                    {
                        var parent = animator.GetBoneTransform(HumanBodyBones.LeftHand);
                        var defaultPoint = parent.Find("defaultEquipPoint");

                        if (defaultPoint)
                            equipPointL.handler.defaultHandler = defaultPoint;
                        else
                        {
                            var _defaultPoint = new GameObject("defaultEquipPoint");
                            _defaultPoint.transform.SetParent(parent);
                            _defaultPoint.transform.localPosition = Vector3.zero;
                            _defaultPoint.transform.forward = itemManager.transform.forward;
                            _defaultPoint.gameObject.tag = "Ignore Ragdoll";
                            equipPointL.handler.defaultHandler = _defaultPoint.transform;
                        }
                    }
                }

                bool containsListener = false;
                for (int i = 0; i < equipPointL.onInstantiateEquiment.GetPersistentEventCount(); i++)
                {
                    if (equipPointL.onInstantiateEquiment.GetPersistentTarget(i).GetType().Equals(typeof(MeleeManager)) && equipPointL.onInstantiateEquiment.GetPersistentMethodName(i).Equals("SetLeftWeapon"))
                    {
                        containsListener = true;
                        break;
                    }
                }

                if (!containsListener && meleeManager)
                {
                    equipPointL.onInstantiateEquiment.AddListener(meleeManager.SetLeftWeapon);
//#if UNITY_EDITOR
//                    UnityEventTools.AddPersistentListener<GameObject>(equipPointL.onInstantiateEquiment, meleeManager.SetLeftWeapon);
//#else
//                    equipPointL.onInstantiateEquiment.AddListener(manager.SetLeftWeapon);
//#endif
                }
            }
            #endregion

            #region RightEquipPoint
            var equipPointR = itemManager.equipPoints.Find(p => p.equipPointName == "RightArm");
            if (equipPointR == null)
            {
                EquipPoint pointR = new EquipPoint();
                pointR.equipPointName = "RightArm";
                if (meleeManager)
                {
                    pointR.onInstantiateEquiment.AddListener(meleeManager.SetRightWeapon);
                    //#if UNITY_EDITOR
                    //                    UnityEventTools.AddPersistentListener<GameObject>(pointR.onInstantiateEquiment, meleeManager.SetRightWeapon);
                    //#else
                    //                    pointR.onInstantiateEquiment.AddListener(manager.SetRightWeapon);
                    //#endif
                }

                if (animator)
                {
                    var defaultEquipPointR = new GameObject("defaultEquipPoint");
                    var parent = animator.GetBoneTransform(HumanBodyBones.RightHand);
                    defaultEquipPointR.transform.SetParent(parent);
                    defaultEquipPointR.transform.localPosition = Vector3.zero;
                    defaultEquipPointR.transform.forward = itemManager.transform.forward;
                    defaultEquipPointR.gameObject.tag = "Ignore Ragdoll";
                    pointR.handler = new Handler();
                    pointR.handler.defaultHandler = defaultEquipPointR.transform;
                }
                itemManager.equipPoints.Add(pointR);
            }
            else
            {
                if (equipPointR.handler.defaultHandler == null)
                {
                    if (animator)
                    {
                        var parent = animator.GetBoneTransform(HumanBodyBones.RightHand);
                        var defaultPoint = parent.Find("defaultEquipPoint");
                        if (defaultPoint) equipPointR.handler.defaultHandler = defaultPoint;
                        else
                        {
                            var _defaultPoint = new GameObject("defaultEquipPoint");
                            _defaultPoint.transform.SetParent(parent);
                            _defaultPoint.transform.localPosition = Vector3.zero;
                            _defaultPoint.transform.forward = itemManager.transform.forward;
                            _defaultPoint.gameObject.tag = "Ignore Ragdoll";
                            equipPointR.handler.defaultHandler = _defaultPoint.transform;
                        }
                    }
                }

                bool containsListener = false;
                for (int i = 0; i < equipPointR.onInstantiateEquiment.GetPersistentEventCount(); i++)
                {

                    if (equipPointR.onInstantiateEquiment.GetPersistentTarget(i).GetType().Equals(typeof(MeleeManager)) && equipPointR.onInstantiateEquiment.GetPersistentMethodName(i).Equals("SetRightWeapon"))
                    {
                        containsListener = true;
                        break;
                    }
                }

                if (!containsListener && meleeManager)
                {
                    equipPointR.onInstantiateEquiment.AddListener(meleeManager.SetRightWeapon);
//#if UNITY_EDITOR
//                    UnityEventTools.AddPersistentListener<GameObject>(equipPointR.onInstantiateEquiment, meleeManager.SetRightWeapon);
//#else
//                    equipPointR.onInstantiateEquiment.AddListener(manager.SetRightWeapon);
//#endif
                }
                #endregion
            }
        }
    }

    [System.Serializable]
    public class ItemReference
    {
        public int id;
        public int amount;
        public ItemReference(int id)
        {
            this.id = id;
            this.autoEquip = true;
        }
        public List<ItemAttribute> attributes;
        public bool changeAttributes;
        public bool autoEquip = true;
        public int indexArea;
    }

    [System.Serializable]
    public class EquipPoint
    {
        #region SeralizedProperties in CustomEditor

        [SerializeField]
        public string equipPointName;
        public EquipmentReference equipmentReference = new EquipmentReference();
        [HideInInspector]
        public EquipArea area;
        public Handler handler = new Handler();
        //public Transform defaultPoint;
        //public List<Transform> customPoints = new List<Transform>();
        public OnInstantiateItemObjectEvent onInstantiateEquiment = new OnInstantiateItemObjectEvent();

        #endregion
    }

    public class EquipmentReference
    {
        public GameObject equipedObject;
        public Item item;
    }

    [System.Serializable]
    public class ApplyAttributeEvent
    {
        [SerializeField]
        public ItemAttributes attribute;
        [SerializeField]
        public OnApplyAttribute onApplyAttribute;
    }

}

