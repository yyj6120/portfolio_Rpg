using UnityEngine;
using UnityEngine.Events;
using Rpg.Character;
using UnityEngine.Audio;
using System.Collections.Generic;

public class ObjectPoolManager : MonoBehaviour
{
    private FisherYatesRandom randomSource = new FisherYatesRandom();
    private CustomObjectPool objectPool = new CustomObjectPool();

    /// <summary>
    /// 발걸음 소리 풀링
    /// </summary>
    /// <param name="audioSource"></param>
    /// <param name="footStepObject"></param>
    /// <param name="audioMixerGroup"></param>
    public void TriggerStepSound(GameObject audioSource, FootStepObject footStepObject, AudioMixerGroup audioMixerGroup, List<AudioClip> audioClips)
    {
        GameObject audioObject = null;
        if (audioSource != null)
        {
            objectPool.Register(audioSource, footStepObject.sender);
            audioObject = objectPool.GetInstance();
            audioObject.transform.position = footStepObject.sender.position;
            audioObject.transform.rotation = Quaternion.identity;
        }

        var source = audioObject.GetComponent<AudioSurfaceControl>();
        if (audioMixerGroup != null)
        {
            source.outputAudioMixerGroup = audioMixerGroup;
        }
        int index = randomSource.Next(audioClips.Count);
        source.PlayOneShot(audioClips[index], objectPool);
    }
    /// <summary>
    /// 발자국 on
    /// </summary>
    /// <param name="stepMark"></param>
    /// <param name="footStep"></param>
    /// <param name="stepLayer"></param>
    public void TriggerStepMarkPaticle(GameObject stepMark, FootStepObject footStep , LayerMask stepLayer)
    {
        RaycastHit hit;
        if (Physics.Raycast(footStep.sender.position + new Vector3(0, 0.1f, 0), -footStep.sender.up, out hit, 1f, stepLayer))
        {
            var angle = Quaternion.FromToRotation(footStep.sender.up, hit.normal);
            if (stepMark != null)
            {
                objectPool.Register(stepMark, stepMark.transform.parent);
                var step = objectPool.GetInstance();
                step.transform.position = hit.point;
                step.transform.rotation = angle * footStep.sender.rotation;
                StartCoroutine(objectPool.UnUseInsert(step , 3f));
            }
        }
    }
}

