using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System;

namespace Rpg.Character
{
    public class MeleeCombatInput : ThirdPersonInput , IMeleeFighter
    {
        [System.Serializable]
        public new class OnUpdateEvent : UnityEngine.Events.UnityEvent<MeleeCombatInput> { }

        #region Variables
        protected MeleeManager meleeManager;
        protected bool isAttacking;
        protected bool isBlocking;
        protected bool isLockingOn;

        [HideInInspector]
        public Vector3 AttackPoint;

        public virtual bool lockInventory
        {
            get
            {
                return isAttacking || character.isDead;
            }
        }
        [HideInInspector]
        public bool lockInputByItemManager;

        [Header("MeleeCombat Inputs")]
        public GenericInput weakAttackInput = new GenericInput("Fire2", "RB", "RB");
        public GenericInput blockInput = new GenericInput("Mouse1", "LB", "LB");
        public bool strafeWhileLockOn = true;
        public GenericInput lockOnInput = new GenericInput("Tab", "RightStickClick", "RightStickClick");

        //[HideInInspector]
        //public OnUpdateEvent onUpdateInput = new OnUpdateEvent();
        //[HideInInspector]
        //public bool lockInputByItemManager;

        #endregion

        protected override void Start()
        {
            base.Start();
        }

        //public virtual bool lockInventory
        //{
        //    get
        //    {
        //        return isAttacking || character.isDead;
        //    }
        //}

        protected override void LateUpdate()
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;
            base.LateUpdate();
            onUpdateInput.Invoke(this);
        }

        protected override void InputHandle()
        {
            if (character == null) return;

            if (MeleeAttackConditions && !lockInputByItemManager)
            {
                MeleeWeakAttackInput();
                BlockingInput();
            }
            else
            {
                isBlocking = false;
            }

            if (!isAttacking)
            {
                base.InputHandle();
                UpdateMeleeAnimations();
            }

            // LockOnInput();
        }

        #region MeleeCombat Input Methods

        /// <summary>
        /// WEAK ATK INPUT
        /// </summary>
        protected virtual void MeleeWeakAttackInput()
        {
            if (character.animator == null)
                return;

            if (weakAttackInput.GetButtonDown())
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, clickMoveLayer))
                {
                    AttackPoint = hit.point;
                }
                character.animator.SetInteger("AttackID", meleeManager.GetAttackID());
                character.animator.SetTrigger("WeakAttack");
            }
        }

        /// <summary>
        /// BLOCK INPUT
        /// </summary>
        protected virtual void BlockingInput()
        {
            if (character.animator == null) return;

            isBlocking = blockInput.GetButtonDown();
        }

        /// <summary>
        /// LOCK ON INPUT
        /// </summary>
        //protected void LockOnInput()
        //{
        //    // lock the camera into a target, if there is any around
        //    if (lockOnInput.GetButtonDown() && !character.actions)
        //    {
        //        isLockingOn = !isLockingOn;
        //        tpCamera.UpdateLockOn(isLockingOn);
        //    }
        //    // unlock the camera if the target is null
        //    else if (isLockingOn && tpCamera.lockTarget == null)
        //    {
        //        isLockingOn = false;
        //        tpCamera.UpdateLockOn(false);
        //    }

        //    // choose to use lock-on with strafe of free movement
        //    if (!cc.locomotionType.Equals(vThirdPersonMotor.LocomotionType.OnlyStrafe))
        //    {
        //        if (strafeWhileLockOn && isLockingOn && tpCamera.lockTarget != null)
        //            cc.isStrafing = true;
        //        else
        //            cc.isStrafing = false;
        //    }

        //    // switch targets using inputs
        //    SwitchTargetsInput();
        //}

        /// <summary>
        /// SWITCH TARGETS INPUT
        /// </summary>
        //void SwitchTargetsInput()
        //{
        //    if (tpCamera == null) return;

        //    if (tpCamera.lockTarget)
        //    {
        //        // switch between targets using Keyboard
        //        if (inputDevice == InputDevice.MouseKeyboard)
        //        {
        //            if (Input.GetKey(KeyCode.X))
        //                tpCamera.gameObject.SendMessage("ChangeTarget", 1, SendMessageOptions.DontRequireReceiver);
        //            else if (Input.GetKey(KeyCode.Z))
        //                tpCamera.gameObject.SendMessage("ChangeTarget", -1, SendMessageOptions.DontRequireReceiver);
        //        }
        //        // switch between targets using GamePad
        //        else if (inputDevice == InputDevice.Joystick)
        //        {
        //            var value = Input.GetAxisRaw("RightAnalogHorizontal");
        //            if (value == 1)
        //                tpCamera.gameObject.SendMessage("ChangeTarget", 1, SendMessageOptions.DontRequireReceiver);
        //            else if (value == -1f)
        //                tpCamera.gameObject.SendMessage("ChangeTarget", -1, SendMessageOptions.DontRequireReceiver);
        //        }
        //    }
        //}

        #endregion

        #region Conditions

        //protected virtual bool MeleeAttackStaminaConditions()
        //{
        //    var result = cc.currentStamina - meleeManager.GetAttackStaminaCost();
        //    return result >= 0;
        //}

        protected virtual bool MeleeAttackConditions
        {
            get
            {
                if (meleeManager == null)
                    meleeManager = GetComponent<MeleeManager>();

                return meleeManager != null && !character.customAction && !character.lockMovement && !character.isCrouching;
            }
        }

        #endregion

        #region Update Animations

        protected virtual void UpdateMeleeAnimations()
        {
            if (character.animator == null || meleeManager == null)
                return;
            character.animator.SetInteger("AttackID", meleeManager.GetAttackID());
        //    character.animator.SetInteger("DefenseID", meleeManager.GetDefenseID());
       //     character.animator.SetBool("IsBlocking", isBlocking);
            character.animator.SetFloat("MoveSet_ID", meleeManager.GetMoveSetID(), .2f, Time.deltaTime);
        }

        #endregion

        #region Melee Methods

        public void OnEnableAttack()
        {
            var dir = (AttackPoint - transform.position).normalized;
            character.attackDirection = new Vector3(dir.x, 0 , dir.z);

            //character.currentStaminaRecoveryDelay = meleeManager.GetAttackStaminaRecoveryDelay();
            //character.currentStamina -= meleeManager.GetAttackStaminaCost();
            isAttacking = true;
        }

        public void OnDisableAttack()
        {
            isAttacking = false;
        }

        public void ResetAttackTriggers()
        {
            character.animator.ResetTrigger("WeakAttack");
          //  character.animator.ResetTrigger("StrongAttack");
        }

        public void BreakAttack(int breakAtkID)
        {
            ResetAttackTriggers();
            OnRecoil(breakAtkID);
        }

        public void OnRecoil(int recoilID)
        {
            character.animator.SetInteger("RecoilID", recoilID);
            character.animator.SetTrigger("TriggerRecoil");
          //  character.animator.SetTrigger("ResetState");
            character.animator.ResetTrigger("WeakAttack");
       //     character.animator.ResetTrigger("StrongAttack");
        }

        public void OnReceiveAttack(Damage damage, IMeleeFighter attacker)
        {
            // character is blocking
            if (!damage.ignoreDefense && isBlocking && meleeManager != null && meleeManager.CanBlockAttack(attacker.Character().transform.position))
            {
                var damageReduction = meleeManager.GetDefenseRate();
                if (damageReduction > 0)
                    damage.ReduceDamage(damageReduction);
                if (attacker != null && meleeManager != null && meleeManager.CanBreakAttack())
                    attacker.OnRecoil(meleeManager.GetDefenseRecoilID());
                meleeManager.OnDefense();
                //cc.currentStaminaRecoveryDelay = damage.staminaRecoveryDelay;
                //cc.currentStamina -= damage.staminaBlockCost;
            }
            // apply damage
            character.TakeDamage(damage, !isBlocking);
        }

        public Character Character()
        {
            return character;
        }

        #endregion
    }
}
