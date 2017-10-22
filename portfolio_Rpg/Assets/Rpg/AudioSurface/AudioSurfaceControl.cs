using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

namespace Rpg.Character
{
    [RequireComponent(typeof(AudioSource))]
    class AudioSurfaceControl : MonoBehaviour
    {
        private AudioSource source;
        private bool isWorking;
        private CustomObjectPool unUsePool;

        public void PlayOneShot(AudioClip clip, CustomObjectPool unUsePool)
        {
            if (!source)
                source = GetComponent<AudioSource>();

            source.PlayOneShot(clip);
            isWorking = true;
            this.unUsePool = unUsePool;
        }
        void Update()
        {
            if (isWorking && !source.isPlaying)
            {
                unUsePool.UnUseInsert(gameObject);
            }
        }

        public AudioMixerGroup outputAudioMixerGroup
        {
            set
            {
                if (!source)
                    source = GetComponent<AudioSource>();

                source.outputAudioMixerGroup = value;
            }
        }
    }
}
