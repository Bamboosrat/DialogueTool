using Test.Control;
using UnityEngine;

namespace DialogueTool
{
    /// <summary>
    /// All properties and methods for the NPC / AI.
    /// 
    /// Add this component to your NPC, give the NPC a name and add a Dialogue.
    /// </summary>
    public class AIConversant : MonoBehaviour , IRaycastable
    {
        
        [SerializeField] Dialogue dialogue = null;
        [SerializeField] string conversantName;

        private Interactable interactable;

        private void Start()
        {
            interactable = gameObject.GetComponentInChildren<Interactable>();
        }

        
        // Not finished
        // Later implementation to change mouse icon when hovering over items, NPC, etc.

        public CursorType GetCursorType()
        {
            return CursorType.Dialogue;
        }

        // If player clicks on the NPC, execute the dialogue.
        public bool HandleRaycast(DialogueController callingController)
        {
            if (dialogue == null)
            {
                return false;
            }

            if (Input.GetMouseButtonDown(0) && interactable.GetClickable())  
            {
                callingController.GetComponent<PlayerConversant>().StartDialogue(this, dialogue);
                transform.LookAt(callingController.transform, Vector3.up);
            }
            return true;
        }

        /// <summary>
        /// Returns NPC name.
        /// </summary>
        
        public string GetName()
        {
            return conversantName;
        }
    }
}

