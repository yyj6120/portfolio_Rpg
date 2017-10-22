using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine.Events;

namespace Rpg.Character
{
    public class AudioSurface : ScriptableObject
    {
        #region pool value
        [System.Serializable]
        public class PlayRandomClipPool : UnityEvent<GameObject, FootStepObject, AudioMixerGroup, List<AudioClip>> { }
        public PlayRandomClipPool soundPool;
        #endregion

        public AudioSource audioSource;
        public AudioMixerGroup audioMixerGroup;
        public List<string> TextureOrMaterialNames;
        public List<AudioClip> audioClips;
        private FisherYatesRandom randomSource = new FisherYatesRandom();

        public AudioSurface()
        {
            audioClips = new List<AudioClip>();
            TextureOrMaterialNames = new List<string>();
        }

        public void PlayRandomClip(FootStepObject footStepObject)
        {
            if (audioClips == null || audioClips.Count == 0)
                return;

            if (randomSource == null)
                randomSource = new FisherYatesRandom();

            soundPool.Invoke(audioSource.gameObject, footStepObject, audioMixerGroup, audioClips);
        }
    }
}
