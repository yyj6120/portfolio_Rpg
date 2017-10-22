using UnityEngine;
using System.Collections.Generic;

namespace Rpg.Item
{
    public class ItemEnumsList : ScriptableObject
    {
        [SerializeField]
        public List<string> itemTypeEnumValues = new List<string>();
        [SerializeField]
        public List<string> itemAttributesEnumValues = new List<string>();
    }
}
