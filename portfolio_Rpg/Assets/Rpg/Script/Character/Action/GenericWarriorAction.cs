using UnityEngine;
using System.Collections;

namespace Rpg.Character
{
    class GenericWarriorAction : ActionListener
    {
        [Tooltip("Tag of the object you want to access")]
        public string actionTag = "Action";
        [Header("--- Debug Only ---")]
        public TriggerGenericAction triggerAction;
        public bool isPlayingAnimation;
        public bool canTriggerAction;
        public bool triggerActionOnce;
        [Tooltip("Input to make the action")]
        public GenericInput actionInput = new GenericInput("E", "A", "A");

        protected ThirdPersonInput thirdPersonInput;

        public virtual bool actionConditions
        {
            get
            {
                return !(thirdPersonInput.character.isJumping || thirdPersonInput.character.actions || !canTriggerAction || isPlayingAnimation) && !thirdPersonInput.character.animator.IsInTransition(0);
            }
        }

        private void Awake()
        {
            actionStay = true;
            actionExit = true;
        }

        protected virtual void Start()
        {
            thirdPersonInput = GetComponent<ThirdPersonInput>();
        }

        protected virtual void LateUpdate()
        {
            AnimationBehaviour();
            TriggerActionInput();
        }

        void OnAnimatorMove()
        {
            if (!playingAnimation)
                return;
            // enable movement using full root motion
            transform.rotation = thirdPersonInput.character.animator.rootRotation;
            transform.position = thirdPersonInput.character.animator.rootPosition;
        }

        protected virtual void TriggerActionInput()
        {
            if (triggerAction == null)
                return;

            if (canTriggerAction)
            {
                if ((triggerAction.autoAction && actionConditions) || (actionInput.GetButtonDown() && actionConditions))
                {
                    if (!triggerActionOnce)
                    {
                        OnDoAction.Invoke(triggerAction);
                        TriggerAnimation();
                    }
                }
            }
        }

        protected virtual void AnimationBehaviour()
        {
            if (playingAnimation)
            {
                if (triggerAction.matchTarget != null)
                    thirdPersonInput.character.MatchTarget(triggerAction.matchTarget.transform.position, triggerAction.matchTarget.transform.rotation, triggerAction.avatarTarget, new MatchTargetWeightMask(triggerAction.matchTargetMask, 0), triggerAction.startMatchTarget, triggerAction.endMatchTarget);

                    if (triggerAction.useTriggerRotation)
                        transform.rotation = Quaternion.Lerp(transform.rotation, triggerAction.transform.rotation, thirdPersonInput.character.animator.GetCurrentAnimatorStateInfo(0).normalizedTime);


                if (triggerAction.resetPlayerSettings && thirdPersonInput.character.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= triggerAction.endExitTimeAnimation)
                {
                    ResetPlayerSettings();
                }
            }
        }

        protected virtual void TriggerAnimation()
        {
            if (!string.IsNullOrEmpty(triggerAction.playAnimation))
            {
                isPlayingAnimation = true;
                thirdPersonInput.character.animator.CrossFadeInFixedTime(triggerAction.playAnimation, 0.1f);
            }
            StartCoroutine(triggerAction.OnDoActionDelay(gameObject));

            // bool to limit the autoAction run just once
            if (triggerAction.autoAction)
                triggerActionOnce = true;

            // destroy the triggerAction if checked with destroyAfter
            if (triggerAction.destroyAfter)
                StartCoroutine(DestroyDelay());
        }

        public virtual IEnumerator DestroyDelay()
        {
            yield return new WaitForSeconds(triggerAction.destroyDelay);
            ResetPlayerSettings();
            Destroy(triggerAction.gameObject);
        }

        public override void OnActionEnter(Collider other)
        {
            if (other.gameObject.CompareTag(actionTag))
            {
                if (triggerAction != null)
                {
                    triggerAction.OnPlayerEnter.Invoke();
                }
            }
        }

        public override void OnActionStay(Collider other)
        {
            if (other.gameObject.CompareTag(actionTag) && !isPlayingAnimation)
            {
                CheckForTriggerAction(other);
            }
        }

        public override void OnActionExit(Collider other)
        {
            if (other.gameObject.CompareTag(actionTag))
            {
                if (triggerAction != null)
                {
                    triggerAction.OnPlayerExit.Invoke();
                }
                ResetPlayerSettings();
            }
        }

        protected virtual void CheckForTriggerAction(Collider other)
        {
            var _triggerAction = other.GetComponent<TriggerGenericAction>();
            if (!_triggerAction) return;

            var dist = Vector3.Distance(transform.forward, _triggerAction.transform.forward);

            if (!_triggerAction.activeFromForward || dist <= 0.8f)
            {
                triggerAction = _triggerAction;
                canTriggerAction = true;
                triggerAction.OnPlayerEnter.Invoke();
            }
            else
            {
                if (triggerAction != null)
                    triggerAction.OnPlayerExit.Invoke();

                canTriggerAction = false;
            }
        }

        protected virtual bool playingAnimation
        {
            get
            {
                if (triggerAction == null)
                {
                    isPlayingAnimation = false;
                    return false;
                }

                if (!isPlayingAnimation && !string.IsNullOrEmpty(triggerAction.playAnimation) && thirdPersonInput.character.baseLayerInfo.IsName(triggerAction.playAnimation))
                {
                    isPlayingAnimation = true;
                    if (triggerAction != null)
                        triggerAction.OnPlayerExit.Invoke();

                    ApplyPlayerSettings();
                }
                else if (isPlayingAnimation && !string.IsNullOrEmpty(triggerAction.playAnimation) && !thirdPersonInput.character.baseLayerInfo.IsName(triggerAction.playAnimation))
                    isPlayingAnimation = false;

                return isPlayingAnimation;
            }
        }

        protected virtual void ApplyPlayerSettings()
        {
            if (triggerAction.disableGravity)
            {
                thirdPersonInput.character.rigidbody.useGravity = false;               // disable gravity of the player
                thirdPersonInput.character.rigidbody.velocity = Vector3.zero;
                thirdPersonInput.character.isGrounded = true;                           // ground the character so that we can run the root motion without any issues
                thirdPersonInput.character.animator.SetBool("IsGrounded", true);        // also ground the character on the animator so that he won't float after finishes the climb animation
                thirdPersonInput.character.animator.SetInteger("ActionState", 1);       // set actionState 1 to avoid falling transitions     
            }
            if (triggerAction.disableCollision)
                thirdPersonInput.character.capsuleCollider.isTrigger = true;           // disable the collision of the player if necessary 
        }

        protected virtual void ResetPlayerSettings()
        {
            if (!playingAnimation || thirdPersonInput.character.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= triggerAction.endExitTimeAnimation)
            {
                thirdPersonInput.character.EnableGravityAndCollision(0f);             // enable again the gravity and collision
                thirdPersonInput.character.animator.SetInteger("ActionState", 0);     // set actionState 1 to avoid falling transitions
            }
            canTriggerAction = false;
            triggerActionOnce = false;
        }
    }
}
