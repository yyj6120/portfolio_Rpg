using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Rpg.Character
{
    public class WariiorSkillController : SkillController
    {
        [System.Serializable]
        public class OnSkill : UnityEvent<Skill> { }
        [HideInInspector]
        public OnSkill onSkill;

        private void Start()
        {
            Init();
        }

        public override void Init()
        {
            base.Init();
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
