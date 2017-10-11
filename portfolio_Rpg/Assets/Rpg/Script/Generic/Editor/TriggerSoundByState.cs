using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TriggerSoundByState : StateMachineBehaviour
{
    public GameObject audioSource;
    public List<AudioClip> sounds;
    private FisherYatesRandom _random;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_random == null)
            _random = new FisherYatesRandom();
        GameObject audioObject = null;
        if (audioSource != null)
            audioObject = Instantiate(audioSource.gameObject, animator.transform.position, Quaternion.identity) as GameObject;
        else
        {
            audioObject = new GameObject("audioObject");
            audioObject.transform.position = animator.transform.position;
        }
        if (audioObject != null)
        {
            var source = audioObject.gameObject.GetComponent<AudioSource>();
            var clip = sounds[_random.Next(sounds.Count)];
            source.PlayOneShot(clip);
        }
    }
}
