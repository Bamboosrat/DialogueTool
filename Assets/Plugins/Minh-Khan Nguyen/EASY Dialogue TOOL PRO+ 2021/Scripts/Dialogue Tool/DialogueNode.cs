using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using OwnTool.Utils;


namespace DialogueTool
{
   /// <summary>
   /// Properties and methods of a node.
   /// </summary>
    public class DialogueNode : ScriptableObject
    {

        #region Field
        // Change to enum if more Conversant or Characters speaking in a conversation
        [Tooltip("Is the player speaking?")]
        [SerializeField]
        private bool isPlayerSpeaking = false;

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

        /// <summary>
        /// 
        /// </summary>
        public Rect GetRect()
        {
            return rect;
        }

        /// <summary>
        /// 
        /// </summary>
        public int GetTextBoxSize()
        {
            return (int)rect.height - 60;
        }


        /// <summary>
        /// 
        /// </summary>
        public string GetText()
        {
            return text;
        }

        /// <summary>
        /// 
        /// </summary>
        public List<string> GetChildren()
        {
            return children;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsPlayerSpeaking()
        {
            return isPlayerSpeaking;
        }

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

