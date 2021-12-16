using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using OwnTool.Utils;

namespace DialogueTool
{
    /// <summary>
    /// All properties and methods for the Player.
    /// 
    /// Add this component to your player and give him a name.
    /// </summary>
    public class PlayerConversant : MonoBehaviour
    {
        #region Fields / Variables
        [SerializeField] private string playerName;

        private Dialogue currentDialogue;
        private DialogueNode currentNode = null;
        private AIConversant currentConversant = null;
        private bool isChoosing = false;

        public PlayerState playerState;

        public event Action onConversationUpdated;
        #endregion

        #region Properties

        /// <summary>
        /// A bool property to check if Player is in a dialogue.
        /// </summary>
        
        public bool IsActive()
        {
            return currentDialogue != null;
        }

        /// <summary>
        /// A bool property to check if the player can choose an option.
        /// </summary>

        public bool IsChoosing()
        {
            return isChoosing;
        }

        /// <summary>
        /// A string property to return the text of the current dialogue.
        /// </summary>
        /// <returns> string message </returns>
        public string GetText()
        {
            if (currentNode == null)
            {
                return "";
            }

            return currentNode.GetText();
        }

        /// <summary>
        /// Returns the current state of the player.
        /// </summary>
        /// <see cref="PlayerState"/>
        public PlayerState GetPlayerState()
        {
            return playerState;
        }


        /// <summary>
        /// Returns the current speaker name.
        /// </summary>

        public string GetCurrentConversantName()
        {
            if (isChoosing)
            {
                return playerName;
            }
            else
            {
                return currentConversant.GetName();
            }
        }

       public IEnumerable<DialogueNode> GetChoices()
       {
           return FilterOnCondition(currentDialogue.GetPlayerChildren(currentNode));
       }


        public bool HasNext()
        {
            return FilterOnCondition(currentDialogue.GetAllChildren(currentNode)).Count() > 0;
        }

        #endregion

        #region Conversant Methods
        /// <summary>
        /// Starts a dialogue.
        /// </summary>
        /// <param name="newConversant"></param>
        /// <param name="newDialogue"></param>
        public void StartDialogue(AIConversant newConversant, Dialogue newDialogue)
        {
            playerState = PlayerState.Dialogue;
            currentConversant = newConversant;
            currentDialogue = newDialogue;
            currentNode = currentDialogue.GetRootNode();
            TriggerEnterAction();
            onConversationUpdated();
        }

        /// <summary>
        /// Ends a dialogue.
        /// </summary>
        public void Quit()
        {
            playerState = PlayerState.None;
            currentDialogue = null;
            TriggerExitAction();
            currentNode = null;
            isChoosing = false;
            currentConversant = null;
            onConversationUpdated();
        }

        /// <summary>
        /// Executes the choice, the player made in the dialogue.
        /// </summary>
        /// <param name="chosenNode"></param>
        public void SelectChoice(DialogueNode chosenNode)
     {
         currentNode = chosenNode;
         TriggerEnterAction();
         isChoosing = false;
         Next();
     }

        // Continues the dialogue, depending on the choices and order of the dialogue.
       public void Next()
       {
           int numPlayerResponses = FilterOnCondition(currentDialogue.GetPlayerChildren(currentNode)).Count();
           if (numPlayerResponses > 0)
           {
               isChoosing = true;
               TriggerExitAction();
               onConversationUpdated();
               return;
           }
     
           DialogueNode[] children = FilterOnCondition(currentDialogue.GetAIChildren(currentNode)).ToArray();
           int randomIndex = UnityEngine.Random.Range(0, children.Count());
           TriggerExitAction();

            // Maybe temporary, if the last node is a PlayerConversant and has no children, end conversation

           if(randomIndex >= 0 && randomIndex < children.Length)
           currentNode = children[randomIndex];
            else
            {
                Debug.LogWarning("[Dialogue Tool Warning] A playernode should not be the last node. \nAborting Conversation.");
                Quit();
            }
           TriggerEnterAction();
           onConversationUpdated();
       }
        #endregion

        #region IEnumerables

        private IEnumerable<DialogueNode> FilterOnCondition(IEnumerable<DialogueNode> inputNode)
       {
           foreach (var node in inputNode)
           {
               if (node.CheckCondition(GetEvaluators()))
               {
                   yield return node;
               }
           }
       }

      private IEnumerable<IPredicateEvaluator> GetEvaluators()
      {
          return GetComponents<IPredicateEvaluator>();
      }
        #endregion

        #region Trigger Calls
        private void TriggerEnterAction()
        {
            if (currentNode != null)
            {
                TriggerAction(currentNode.GetOnEnterAction());
            }
        }

        private void TriggerExitAction()
        {
            if (currentNode != null)
            {
                TriggerAction(currentNode.GetOnExitAction());
            }
        }

        private void TriggerAction(string action)
        {
            if (action == "") return;

            Debug.Log(action);

            foreach (DialogueTrigger trigger in currentConversant.GetComponents<DialogueTrigger>())
            {
                trigger.Trigger(action);
            }
        }
        #endregion
    }
}