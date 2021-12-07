using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using OwnTool.Utils;


namespace DialogueTool
{
   
    public class DialogueNode : ScriptableObject
    {

        #region Field
        // Change to enum if more Conversant or Characters speaking in a conversation
        [Tooltip("Is the player speaking?")]
        [SerializeField]
        private bool isPlayerSpeaking = false;
       // [SerializeField]
       // private bool isConversationEnding = false;

        [SerializeField]
        [TextArea(5,20)]
        private string text;
        [Tooltip("Change the size of the node.")]
        [SerializeField]
        private Rect rect = new Rect(0, 0, 200, 150);

        [Header("Conditions")]
        [SerializeField]
        private string onEnterAction;
        [SerializeField]
        private string onExitAction;
        [SerializeField]
        private Condition condition;

        [Header("Child nodes")]
        [SerializeField]
        private List<string> children = new List<string>();

        #endregion

        #region Properties

        // Get rekt, nerd. GG EZ lmao
        public Rect GetRect()
        {
            return rect;
        }

        public int GetTextBoxSize()
        {
            return (int)rect.height - 60;
        }

        public string GetText()
        {
            return text;
        }

        public List<string> GetChildren()
        {
            return children;
        }

        public bool IsPlayerSpeaking()
        {
            return isPlayerSpeaking;
        }

       // public bool IsConversationEnding()
       // {
       //     return isConversationEnding;
       // }

        public string GetOnEnterAction()
        {
            return onEnterAction;
        }

        public string GetOnExitAction()
        {
            return onExitAction;
        }

        public bool CheckCondition(IEnumerable<IPredicateEvaluator> evaluators)
        {
            return condition.Check(evaluators);
        }


        #if UNITY_EDITOR
        public void SetPosition(Vector2 newPosition)
        {
            Undo.RecordObject(this, "Move Dialogue Position");
            rect.position = newPosition;
            EditorUtility.SetDirty(this);
        }

        public void SetText(string newText)
        {
            if (newText != text)
            {
                Undo.RecordObject(this, "Update Dialogue Text");

                text = newText;
                EditorUtility.SetDirty(this);
            }
        }

        public void AddChild(string childID)
        {
            Undo.RecordObject(this, "Add Dialogue Link");
            children.Add(childID);
            EditorUtility.SetDirty(this);
        }

        public void RemoveChild(string childID)
        {
            Undo.RecordObject(this, "Remove Dialogue Link");
            children.Remove(childID);
            EditorUtility.SetDirty(this);
        }

        public void SetPlayerSpeaking(bool newIsPlayerSpeaking)
        {
            Undo.RecordObject(this, "Change Dialogue Speaker");
            isPlayerSpeaking = newIsPlayerSpeaking;
            EditorUtility.SetDirty(this);
        }
        #endif

        #endregion
    }
}

