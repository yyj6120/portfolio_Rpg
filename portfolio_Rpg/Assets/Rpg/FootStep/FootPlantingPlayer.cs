using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Rpg.Character
{

    public abstract class FootPlantingPlayer : MonoBehaviour
    {
        public AudioSurface defaultSurface;
        public List<AudioSurface> customSurfaces;

        public void PlayFootFallSound(FootStepObject footStepObject)
        {
            for (int i = 0; i < customSurfaces.Count; i++)
                if (customSurfaces[i] != null && ContainsTexture(footStepObject.name, customSurfaces[i]))
                {
                    customSurfaces[i].PlayRandomClip(footStepObject);
                    return;
                }
            if (defaultSurface != null)
                defaultSurface.PlayRandomClip(footStepObject);
        }

        // check if AudioSurface Contains texture in TextureName List
        private bool ContainsTexture(string name, AudioSurface surface)
        {
            for (int i = 0; i < surface.TextureOrMaterialNames.Count; i++)
                if (name.Contains(surface.TextureOrMaterialNames[i]))
                    return true;

            return false;
        }
    }
}
