using UnityEngine;
using UnityEngine.Events;

namespace Rpg.Character
{
    class InstantiateStepMark : MonoBehaviour
    {
        public GameObject stepMark;
        public LayerMask stepLayer;
        public float activeTime = 3f;

        public CustomObjectPool objectPool = new CustomObjectPool();
        /// <summary>
        /// 발자국
        /// </summary>
        /// <param name="footStep"></param>
        void StepMark(FootStepObject footStep)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + new Vector3(0, 0.1f, 0), -footStep.sender.up, out hit, 1f, stepLayer))
            {
                var angle = Quaternion.FromToRotation(footStep.sender.up, hit.normal);
                if (stepMark != null)
                {
                    objectPool.Register(stepMark, stepMark.transform.parent);
                    var step = objectPool.GetInstance();
                    step.transform.position = hit.point;
                    step.transform.rotation = angle * footStep.sender.rotation;
                    StartCoroutine(objectPool.UnUseInsert(step, activeTime));
                }
            }
        }
    }
}
