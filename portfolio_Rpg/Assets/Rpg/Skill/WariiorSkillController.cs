using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Rpg.Character
{
    public class WariiorSkillController : SkillController
    {
        [System.Serializable]
        public class OnSkill : UnityEvent<WarriorTypeSkill> { }
        public OnSkill onSkill;

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
            {
                if(actionBarInput[i].GetButtonDown())
                {
                    onSkill.Invoke(actionSlot[i].skill.warriorTypeSkill);
                }
            }
        }

        public void Register(WarriorTypeSkill warriorTypeSkill)
        {
            if(WarriorTypeSkill.test2 == warriorTypeSkill)
            {
                test2();
            }
        }

        public void test2()
        {
            Debug.Log(1);
        }
    }
}
