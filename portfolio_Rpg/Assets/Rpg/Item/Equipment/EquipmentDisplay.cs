﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Rpg.Item
{
    public class EquipmentDisplay : ItemSlot
    {
        public Text slotIdentifier;

        public override void AddItem(Item item)
        {
            if (this.item != item)
            {
                base.AddItem(item);
                if (item != null && item.amount > 1)
                    this.amountText.text = item.amount.ToString("00");
                else
                    this.amountText.text = "";
            }
        }

        public void ItemIdentifier(int identifier = 0, bool showIdentifier = false)
        {
            if (!slotIdentifier) return;

            if (showIdentifier)
            {
                slotIdentifier.text = identifier.ToString();
            }
            else
            {
                slotIdentifier.text = string.Empty;
            }
        }
    }
}

