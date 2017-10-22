using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Button))]
public class ItemSlotKey : MonoBehaviour
{ 
    public GameObject Slot_Object;
    void Start()
    {
        Slot_Object = this.gameObject;
        GetComponent<Button>().onClick.AddListener(SendSelectKey);
    }
    public void SendSelectKey()
    {
      //  RPG_SingleTon.Instance.Overlay_root.ClickItemSlot(Slot_Object);
    }
}
