using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace Rpg.Character
{
    public abstract class SkillController : MonoBehaviour
    {
        [Header("ActionBar Inputs : To use it, you must set the string value in the editor.")]
        [SerializeField]
        public List<GenericInput> actionBarInput;
        public ActionSlot[] actionSlot;
        public virtual void Init()
        {
            actionSlot = HUDController.instance.GetComponentsInChildren<ActionSlot>();
        }
    }
}

