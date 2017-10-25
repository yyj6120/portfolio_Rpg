using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace Rpg.Character
{
    public class SkillInput : MonoBehaviour
    {
        [System.Serializable]
        public class OnSkill : UnityEvent<Skill> { }
        [HideInInspector]
        public OnSkill onSkill;
        [Header("ActionBar Inputs : To use it, you must set the string value in the editor.")]
        [SerializeField]
        public List<GenericInput> actionBarInput;
        private ActionSlot[] actionSlot;
        private ThirdPersonController character;

        private void Start()
        {
            actionSlot = HUDController.instance.GetComponentsInChildren<ActionSlot>();
            character = GetComponent<ThirdPersonController>();
            onSkill.AddListener(Register);
        }

        private void LateUpdate()
        {
            InputActionBar();
        }

        protected virtual void InputActionBar()
        {
            for (int i = 0; i < actionSlot.Length; i++)
                if (actionBarInput[i].GetButtonDown() && actionSlot[i].socket != null)
                    onSkill.Invoke(actionSlot[i].socket);
        }

        public void Register(Skill socket)
        {
            character.animator.SetInteger("SkillID", socket.skillID);
            character.animator.SetTrigger("TriggerSkill");
        }
    }
}

