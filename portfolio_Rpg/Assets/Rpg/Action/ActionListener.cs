using UnityEngine;
using UnityEngine.Events;

namespace Rpg.Character
{
    [System.Serializable]
    public class OnTriggerActionHandle : UnityEvent<TriggerGenericAction> { }

    public abstract class ActionListener : MonoBehaviour
    {
        public bool actionEnter;
        public bool actionStay;
        public bool actionExit;
        [HideInInspector]
        public OnTriggerActionHandle OnDoAction;

        public virtual void OnActionEnter(Collider other)
        {

        }

        public virtual void OnActionStay(Collider other)
        {

        }

        public virtual void OnActionExit(Collider other)
        {

        }
    }
}
