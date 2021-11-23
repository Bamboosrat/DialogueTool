using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;


// DialogueTool.Editor can use all scripts inside DialogueTool namespace but not vice versa
// Allowing for optimal nested usage
namespace DialogueTool.Editor
{

    public class DialogueEditor : EditorWindow
    {
        Dialogue selectedDialogue = null;


        // static keyword or methods belongs to no specific instances but all DialogueEditor scripts
        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow()
        {
            // Debug.Log("ShowEditorWindow");
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }


        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            // "as" keyword for casting, returns null if it can't find casted object
            Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
           
            if(dialogue != null)
            {
               // Debug.Log("Open Dialogue");

                ShowEditorWindow();
                return true;

            }

            return false;
        }

        private void OnEnable()
        {

            // This is an Event callback
            Selection.selectionChanged += OnSelectionChange;
            
        }

        private void OnSelectionChange()
        {
            Dialogue newDialogue = Selection.activeObject as Dialogue;
            if(newDialogue != null)
            {
                selectedDialogue = newDialogue;
                Repaint();
            }
        }


        private void OnGUI()
        {
            if(selectedDialogue == null)
                EditorGUILayout.LabelField("No Dialogue selected!");
            else
            {
         
                foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    string newText = EditorGUILayout.TextField(node.Text);
                    // save/ mark as dirty, updates/ changes the scriptable object
                    if (newText != node.Text)
                    {
                        node.Text = newText;
                        EditorUtility.SetDirty(selectedDialogue);
                    }
                    
                }

            }
        }
    }
}
