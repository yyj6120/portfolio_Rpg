using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class OnAIHitEnter : UnityEvent<AIHitInfo> { }

public class AIAttackObject : MonoBehaviour
{
    [HideInInspector]
    public AIManager aiManager;
    public int movesetID;
    public float distanceToAttack;
    public int attackID;
    public Damage damage;
    public List<AIHitBox> hitBoxes;
    public int damageModifier;
    [HideInInspector]
    public bool canApplyDamage;

    [HideInInspector]
    public OnAIHitEnter onDamageHit;
    [HideInInspector]
    public OnAIHitEnter onRecoilHit;
    [HideInInspector]
    public UnityEvent onEnableDamage, onDisableDamage;
    private Dictionary<AIHitBox, List<GameObject>> targetColliders;

    private void Start()
    {
        targetColliders = new Dictionary<AIHitBox, List<GameObject>>();// init list of targetColliders
        if (hitBoxes.Count > 0)
        {
            /// inicialize the hitBox properties
            foreach (AIHitBox hitBox in hitBoxes)
            {
                hitBox.attackObject = this;
                targetColliders.Add(hitBox, new List<GameObject>());
            }
        }
        else
        {
            this.enabled = false;
        }
    }
    /// <summary>
    /// Set Active all hitBoxes of the MeleeAttackObject
    /// </summary>
    /// <param name="value"> active value</param>  
    public virtual void SetActiveDamage(bool value)
    {
        canApplyDamage = value;
        for (int i = 0; i < hitBoxes.Count; i++)
        {
            var hitCollider = hitBoxes[i];
            hitCollider.trigger.enabled = value;
            if (value == false && targetColliders != null)
                targetColliders[hitCollider].Clear();
        }
        if (value)
            onEnableDamage.Invoke();
        else
        {
            onDisableDamage.Invoke();
        }
    }

    public virtual void OnHit(AIHitBox hitBox, Collider other)
    {
        //Check  first contition for hit 
        if (canApplyDamage && !targetColliders[hitBox].Contains(other.gameObject) && (aiManager != null && other.gameObject != aiManager.gameObject))
        {
            var inDamage = false;

            if (aiManager == null)
                aiManager = GetComponentInParent<AIManager>();
            //check if meleeManager exist and apply  his hitProperties  to this
            HitProperties _hitProperties = aiManager.hitProperties;

            /// Damage Conditions
            if ((_hitProperties.hitDamageTags.Contains(other.tag)))
                inDamage = true;

            if (inDamage)
            {
                ///add target collider in list to control frequency of hit him
                targetColliders[hitBox].Add(other.gameObject);
                AIHitInfo hitInfo = new AIHitInfo(this, hitBox, other, hitBox.transform.position);
                if (inDamage == true)
                {
                    /// If meleeManager 
                    /// call onDamageHit to control damage values
                    /// and  meleemanager will call the ApplyDamage after to filter the damage
                    /// if meleeManager is null
                    /// The damage will be  directly applied
                    /// Finally the OnDamageHit event is called
	                if (aiManager)
                        aiManager.OnDamageHit(hitInfo);
                    else
                    {
                        damage.sender = transform;
                        ApplyDamage(hitBox, other, damage);
                    }
                    onDamageHit.Invoke(hitInfo);
                }
            }
        }
    }

    /// <summary>
    /// Apply damage to target collider (SendMessage(TakeDamage,damage))
    /// </summary>
    /// <param name="hitBox">vHitBox object</param>
    /// <param name="other">collider target</param>
    /// <param name="damage"> damage</param>
    public void ApplyDamage(AIHitBox hitBox, Collider other, Damage damage)
    {
        Damage _damage = new Damage(damage);
        _damage.damageValue = (int)Mathf.RoundToInt(((float)(damage.damageValue + damageModifier) * (((float)hitBox.damagePercentage) * 0.01f)));
        //_damage.sender = transform;
        _damage.hitPosition = hitBox.transform.position;
        if (other.gameObject.IsAMeleeFighter())
        {
            other.gameObject.GetMeleeFighter().OnReceiveAttack(_damage, aiManager.fighter);
        }
        else if (other.gameObject.CanReceiveDamage())
            other.gameObject.ApplyDamage(_damage);
    }
}

public class AIHitInfo
{
    public AIAttackObject aiattackObject;
    public AIHitBox aiHitBox;
    public Vector3 hitPoint;
    public Collider targetCollider;

    public AIHitInfo(AIAttackObject attackObject, AIHitBox hitBox, Collider targetCollider, Vector3 hitPoint)
    {
        this.aiattackObject = attackObject;
        this.aiHitBox = hitBox;
        this.targetCollider = targetCollider;
        this.hitPoint = hitPoint;
    }
}

