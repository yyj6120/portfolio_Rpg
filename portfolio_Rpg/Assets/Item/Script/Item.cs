using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Rpg.Item
{
    [System.Serializable]
    public class Item : ScriptableObject
    {
        [HideInInspector]
        public int id;
        [HideInInspector]
        public string description = "Item Description";
        [HideInInspector]
        public ItemType type;
        [HideInInspector]
        public Sprite icon;
        [HideInInspector]
        public bool stackable = true;
        [HideInInspector]
        public int maxStack;
        [HideInInspector]
        public int amount;
        [HideInInspector]
        public GameObject originalObject;
        [HideInInspector]
        public GameObject dropObject;
        [HideInInspector]
        public List<ItemAttribute> attributes = new List<ItemAttribute>();
        [HideInInspector]
        public bool isInEquipArea;

        public bool twoHandWeapon;

        [Header("Equipable Settings")]
        public int EquipID;
        public string customEquipPoint = "defaultPoint";
        public float equipDelayTime = 0.5f;

        public Texture2D iconTexture
        {
            get
            {
                if (!icon) return null;
                try
                {
                    if (icon.rect.width != icon.texture.width || icon.rect.height != icon.texture.height)
                    {
                        Texture2D newText = new Texture2D((int)icon.textureRect.width, (int)icon.textureRect.height);
                        newText.name = icon.name;
                        Color[] newColors = icon.texture.GetPixels((int)icon.textureRect.x, (int)icon.textureRect.y, (int)icon.textureRect.width, (int)icon.textureRect.height);
                        newText.SetPixels(newColors);
                        newText.Apply();

                        return newText;
                    }
                    else
                        return icon.texture;
                }
                catch
                {
                    return icon.texture;
                }
            }
        }

        public ItemAttribute GetItemAttribute(string name)
        {
            if (attributes != null)
                return attributes.Find(attribute => attribute.name.Equals(name));
            return null;
        }
    }
}
