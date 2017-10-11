using UnityEngine;
using System.Collections.Generic;

namespace Rpg.Item
{
    public class ItemListData : ScriptableObject
    {
        public List<Item> items = new List<Item>();
        public bool inEdition;
        public bool itemsHidden = true;
    }
}
