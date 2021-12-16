using UnityEngine;
using UnityEngine.Events;

namespace DialogueTool
{
    public class DialogueTrigger : MonoBehaviour
    {
        #region Fields / Variables
        [SerializeField] private string action;
        [SerializeField] private UnityEvent onTrigger;
        #endregion

        // In developement.
        #region Trigger Method
        public void Trigger(string actionToTrigger)
        {
            if (actionToTrigger == action)
            {
                onTrigger.Invoke();
            }
        }
        #endregion
    }
}