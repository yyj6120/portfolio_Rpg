using UnityEngine;
using System;

[Serializable]
public class OnAIHitEvent : UnityEngine.Events.UnityEvent<AIHitInfo> { }

public class AIManager : MonoBehaviour
{
    #region SeralizedProperties in CustomEditor
    public Damage defaultDamage = new Damage(10);
    public HitProperties hitProperties;
    public AIAttackObject aiAttackObject;
    public OnAIHitEvent onDamageHit, onRecoilHit;
    #endregion

    [Tooltip("NPC ONLY- Ideal distance for the attack")]
    public float defaultAttackDistance = 1f;

    [HideInInspector]
    public IMeleeFighter fighter;
    private int damageMultiplier;
    private string attackName;

    protected virtual void Start()
    {
        Init();
    }

    /// <summary>
    /// Init properties
    /// </summary>
    protected virtual void Init()
    {
        fighter = gameObject.GetMeleeFighter();
        var attackObject = this.aiAttackObject.GetComponent<AIAttackObject>();

        attackObject.aiManager = this;
        attackObject.SetActiveDamage(false);
    }

    /// <summary>
    /// Set active Multiple Parts to attack
    /// </summary>
    /// <param name="bodyParts"></param>
    /// <param name="type"></param>
    /// <param name="active"></param>
    /// <param name="damageMultiplier"></param>
    public virtual void SetActiveAttack(vAttackType type, bool active = true, int damageMultiplier = 0, string attackName = "")
    {
        this.damageMultiplier = damageMultiplier;
        this.attackName = attackName;

        if (type == vAttackType.Unarmed)
        {
            aiAttackObject.SetActiveDamage(active);
        }
    }
    /// <summary>
    /// Listener of Damage Event
    /// </summary>
    /// <param name="hitInfo"></param>
    public virtual void OnDamageHit(AIHitInfo hitInfo)
    {
        Damage damage = new Damage(hitInfo.aiattackObject.damage);
        damage.sender = transform;
        damage.receiver = hitInfo.targetCollider.transform;
        if (this.attackName != string.Empty)
            damage.attackName = this.attackName;
        /// Calc damage with multiplier 
        /// and Call ApplyDamage of attackObject 
        damage.damageValue *= damageMultiplier > 1 ? damageMultiplier : 1;
        hitInfo.aiattackObject.ApplyDamage(hitInfo.aiHitBox, hitInfo.targetCollider, damage);
        onDamageHit.Invoke(hitInfo);
    }
    /// <summary>
    /// Get Current Attack ID
    /// </summary>
    /// <returns></returns>
    public virtual int GetAttackID()
    {
        if (this.aiAttackObject != null && this.aiAttackObject.gameObject.activeSelf)
            return aiAttackObject.attackID;

        return 0;
    }
    /// <summary>
    /// Get ideal distance for the attack
    /// </summary>
    /// <returns></returns>
    public virtual float GetAttackDistance()
    {
        if (aiAttackObject != null && aiAttackObject.gameObject.activeSelf)
            return aiAttackObject.distanceToAttack;

        return defaultAttackDistance;
    }
    /// <summary>
    /// Get Current MoveSet ID
    /// </summary>
    /// <returns></returns>
    public virtual int GetMoveSetID()
    {
        if (aiAttackObject != null && aiAttackObject.gameObject.activeSelf)
            return aiAttackObject.movesetID;

        return 0;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="weaponObject"></param>
    public void SetAIattackObject(AIAttackObject aiAttackObject)
    {
        if (aiAttackObject)
        {
            this.aiAttackObject = aiAttackObject;
            aiAttackObject.aiManager = this;
            aiAttackObject.SetActiveDamage(false);
        }
    }
}