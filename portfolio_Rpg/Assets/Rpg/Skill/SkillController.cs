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
        protected ActionSlot[] actionSlot;
        protected ThirdPersonController character;
        public virtual void Init()
        {
            actionSlot = HUDController.instance.GetComponentsInChildren<ActionSlot>();
            character = GetComponent<ThirdPersonController>();
        }
    }
}

