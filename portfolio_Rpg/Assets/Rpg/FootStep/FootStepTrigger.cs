using UnityEngine;

namespace Rpg.Character
{
    class FootStepTrigger : MonoBehaviour
    {
        protected Collider _trigger;
        protected FootStepFromTexture footStepFromTexture;

        void Start()
        {
            footStepFromTexture = GetComponentInParent<FootStepFromTexture>();
            if (footStepFromTexture == null)
                gameObject.SetActive(false);
        }

        public Collider trigger
        {
            get
            {
                if (_trigger == null)
                    _trigger = GetComponent<Collider>();

                return _trigger;
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (footStepFromTexture == null)
                return;

            if (other.GetComponent<Terrain>() != null)
                footStepFromTexture.StepOnTerrain(new FootStepObject(transform, other.transform));
            else
            {
                var renderer = other.GetComponent<Renderer>();
                if (renderer != null && renderer.material != null)
                {
                    var index = 0;
                    var _name = string.Empty;
                        _name = renderer.materials[index].name;
                    footStepFromTexture.StepOnMesh(new FootStepObject(transform, other.transform, _name));
                }
            }
        }
    }

    public class FootStepObject
    {
        public string name;
        public Transform sender;
        public Transform ground;

        public FootStepObject(Transform sender, Transform ground, string name = "")
        {
            this.name = name;
            this.sender = sender;
            this.ground = ground;
        }
    }
}
