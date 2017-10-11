using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class HitDamageParticle : MonoBehaviour
{
    [System.Serializable]
    public class TriggerHitParticlePool : UnityEvent<GameObject, HittEffectInfo , Transform> { }

    public GameObject defaultHitEffect;
    public List<HitEffect> customHitEffects = new List<HitEffect>();
    public TriggerHitParticlePool HitParticlePool;

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        var character = GetComponent<Rpg.Character.Character>();
        if (character != null)
        {
            character.onReceiveDamage.AddListener(OnReceiveDamage);
        }
    }

    public void OnReceiveDamage(Damage damage)
    {
        // instantiate the hitDamage particle - check if your character has a HitDamageParticle component
        var damageDirection = new Vector3(transform.position.x, damage.hitPosition.y, transform.position.z) - damage.hitPosition;
        var hitrotation = damageDirection != Vector3.zero ? Quaternion.LookRotation(damageDirection) : transform.rotation;

        if (damage.damageValue > 0)
            TriggerHitParticle(new HittEffectInfo(new Vector3(transform.position.x, damage.hitPosition.y, transform.position.z), hitrotation, damage.attackName, damage.receiver));
    }

    /// <summary>
    /// Raises the hit event.
    /// </summary>
    /// <param name="hitEffectInfo">Hit effect info.</param>
    void TriggerHitParticle(HittEffectInfo hitEffectInfo)
    {
        var hitEffect = customHitEffects.Find(effect => effect.hitName.Equals(hitEffectInfo.hitName));

        if (hitEffect != null)
        {
            if (hitEffect.hitPrefab != null)
            {
                var prefab = Instantiate(hitEffect.hitPrefab, hitEffectInfo.position, hitEffect.rotateToHitDirection ? hitEffectInfo.rotation : hitEffect.hitPrefab.transform.rotation) as GameObject;
                if (hitEffect.attachInReceiver && hitEffectInfo.receiver)
                    prefab.transform.SetParent(hitEffectInfo.receiver);
            }
        }
        else if (defaultHitEffect != null)
        {
            Instantiate(defaultHitEffect, hitEffectInfo.position, hitEffectInfo.rotation);
         //   HitParticlePool.Invoke(defaultHitEffect, hitEffectInfo , transform );
        }
    }
}

public class HittEffectInfo
{
    public Transform receiver;
    public Vector3 position;
    public Quaternion rotation;
    public string hitName;
    public HittEffectInfo(Vector3 position, Quaternion rotation, string hitName = "", Transform receiver = null)
    {
        this.receiver = receiver;
        this.position = position;
        this.rotation = rotation;
        this.hitName = hitName;
    }
}

[System.Serializable]
public class HitEffect
{
    public string hitName = "";
    public GameObject hitPrefab;
    public bool rotateToHitDirection = true;
    [Tooltip("Attach prefab in Damage Receiver transform")]
    public bool attachInReceiver = false;
}
