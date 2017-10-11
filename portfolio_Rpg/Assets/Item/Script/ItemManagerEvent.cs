﻿using UnityEngine;
using System.Collections;

namespace Rpg.Item
{
    [System.Serializable]
    public class OnChangeEquipmentEvent : UnityEngine.Events.UnityEvent<EquipArea, Item> { }
    [System.Serializable]
    public class OnInstantiateItemObjectEvent : UnityEngine.Events.UnityEvent<GameObject> { }
    [System.Serializable]
    public class OnHandleItemEvent : UnityEngine.Events.UnityEvent<Item> { }
    [System.Serializable]
    public class OnChangeItemAmount : UnityEngine.Events.UnityEvent<Item, int> { }
    [System.Serializable]
    public class OnCollectItems : UnityEngine.Events.UnityEvent<GameObject> { }
    [System.Serializable]
    public class OnApplyAttribute : UnityEngine.Events.UnityEvent<int> { }
    [System.Serializable]
    public class OnOpenCloseInventory : UnityEngine.Events.UnityEvent<bool> { }
}
