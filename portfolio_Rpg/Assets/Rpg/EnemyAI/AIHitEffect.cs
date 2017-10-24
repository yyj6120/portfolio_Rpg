using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIHitEffect : MonoBehaviour
{
    public GameObject audioSource;
    public AudioClip[] hitSounds;

    [Header("attack:")]
    public AudioClip attackHitSounds;

    void Start()
    {
        var aiAttackObject = GetComponent<AIAttackObject>();
        if (aiAttackObject)
        {
            aiAttackObject.onDamageHit.AddListener(PlayHitEffects);
        }
    }

    public void PlayHitEffects(AIHitInfo hitInfo)
    {
        if (hitInfo.targetCollider != null && hitInfo.targetCollider.tag.Equals("Player"))
        {
            var clip = attackHitSounds;
            var audioObj = Instantiate(audioSource, transform.position, transform.rotation) as GameObject;
            audioObj.GetComponent<AudioSource>().PlayOneShot(clip);
        }

        if (audioSource != null && hitSounds.Length > 0)
        {
            var clip = hitSounds[UnityEngine.Random.Range(0, hitSounds.Length)];
            var audioObj = Instantiate(audioSource, transform.position, transform.rotation) as GameObject;
            audioObj.GetComponent<AudioSource>().PlayOneShot(clip);
        }
    }

}
