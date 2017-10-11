using UnityEngine;
using UnityEngine.Events;

namespace Rpg.Character
{
    public abstract class CharacterSpell : MonoBehaviour
    {
        protected abstract void PlayDown();
        protected abstract void PlayHeld();
        protected abstract void Playup();
    }

    [System.Serializable]
    public class OnDead : UnityEvent<GameObject> { }
    [System.Serializable]
    public class OnReceiveDamage : UnityEvent<Damage> { }
    [System.Serializable]
    public class OnActiveRagdoll : UnityEvent { }

    [System.Serializable]
    public class OnActionHandle : UnityEvent<Collider> { }

    public abstract class Character : MonoBehaviour , IDamageReceiver
    {
        [HideInInspector]
        public float percentage = 1f;
        public float maxHealth = 1000f;
        [HideInInspector]
        protected float health = 1000.0f;
        [HideInInspector]
        protected float mana = 1000.0f;
        [HideInInspector]
        public float currentHealthRecoveryDelay;
        public float healthRecovery = 0f;
        [HideInInspector]
        public float healthRecoveryDelay = 0f;

        public float currentHealth;

        [HideInInspector]
        protected bool playerIsDead = false;
        [HideInInspector]
        public Animator animator;

        [HideInInspector]
        public OnDead onDead = new OnDead();

        [HideInInspector]
        public bool isDead;

        [HideInInspector]
        public bool ragdolled
        {
            get; set;
        }

        public Transform GetTransform
        {
            get { return transform; }
        }

        [HideInInspector]
        public OnActiveRagdoll onActiveRagdoll = new OnActiveRagdoll();
        [HideInInspector]
        public OnReceiveDamage onReceiveDamage = new OnReceiveDamage();
        [HideInInspector]
        public OnActionHandle onActionEnter = new OnActionHandle();
        [HideInInspector]
        public OnActionHandle onActionStay = new OnActionHandle();
        [HideInInspector]
        public OnActionHandle onActionExit = new OnActionHandle();

        public enum DeathBy
        {
            Animation,
            AnimationWithRagdoll,
            Ragdoll
        }

        public DeathBy deathBy = DeathBy.Animation;

        public virtual void Init()
        {
            var actionListeners = GetComponents<ActionListener>();
            for (int i = 0; i < actionListeners.Length; i++)
            {
                if (actionListeners[i].actionEnter)
                    onActionEnter.AddListener(actionListeners[i].OnActionEnter);
                if (actionListeners[i].actionStay)
                    onActionStay.AddListener(actionListeners[i].OnActionStay);
                if (actionListeners[i].actionExit)
                    onActionExit.AddListener(actionListeners[i].OnActionExit);
            }
        }

        public virtual void TakeDamage(Damage damage, bool hitReaction = true)
        {
            if (damage != null)
            {
                currentHealth -= damage.damageValue;
                percentage  = (currentHealth / maxHealth);
                Debug.Log(percentage);
                if (damage.activeRagdoll)
                {
                    EnableRagdoll();
                }
            }
        }

        public virtual void ResetRagdoll()
        {

        }

        public virtual void EnableRagdoll()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            onActionEnter.Invoke(other);
        }

        private void OnTriggerStay(Collider other)
        {
            onActionStay.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            onActionExit.Invoke(other);
        }

    }
}   
