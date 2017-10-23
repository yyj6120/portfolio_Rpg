using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Rpg.Character
{
    public class WariiorSkillController : SkillController
    {
        [System.Serializable]
        public class OnSkill : UnityEvent<SkillofWariior> { }
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
            for(int i = 0; i < actionSlot.Length; i++)
                if(actionBarInput[i].GetButtonDown() && actionSlot[i].skill != null)
                    onSkill.Invoke(actionSlot[i].skill.skillofWariior);            
        }

        public void Register(SkillofWariior skillofWariior)
        {
            if (SkillofWariior.Rotarycut == skillofWariior) { Rotarycut(); }
        }

        public void Rotarycut()
        {
            character.animator.SetTrigger("Rotarycut");
        }
    }
}
