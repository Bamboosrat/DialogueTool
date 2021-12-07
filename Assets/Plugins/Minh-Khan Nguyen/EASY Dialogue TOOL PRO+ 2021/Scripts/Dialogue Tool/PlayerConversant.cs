using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using OwnTool.Utils;

namespace DialogueTool
{
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

        public bool IsActive()
        {
            return currentDialogue != null;
        }

        public bool IsChoosing()
        {
            return isChoosing;
        }

        public string GetText()
        {
            if (currentNode == null)
            {
                return "";
            }

            return currentNode.GetText();
        }

        public PlayerState GetPlayerState()
        {
            return playerState;
        }

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
        public void StartDialogue(AIConversant newConversant, Dialogue newDialogue)
        {
            playerState = PlayerState.Dialogue;
            currentConversant = newConversant;
            currentDialogue = newDialogue;
            currentNode = currentDialogue.GetRootNode();
            TriggerEnterAction();
            onConversationUpdated();
        }

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


        public void SelectChoice(DialogueNode chosenNode)
     {
         currentNode = chosenNode;
         TriggerEnterAction();
         isChoosing = false;
         Next();
     }

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

            foreach (DialogueTrigger trigger in currentConversant.GetComponents<DialogueTrigger>())
            {
                trigger.Trigger(action);
            }
        }
        #endregion
    }
}