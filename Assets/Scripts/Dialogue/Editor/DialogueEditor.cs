using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;


// DialogueTool.Editor can use all scripts inside DialogueTool namespace but not vice versa
// Allowing for optimal nested usage
namespace DialogueTool.Editor
{

    public class DialogueEditor : EditorWindow
    {
        Dialogue selectedDialogue = null;
        GUIStyle nodeStyle;
        DialogueNode draggingNode = null;
        Vector2 draggingOffset;


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
                ShowEditorWindow();
                return true;
            }

            return false;
        }

        // This is an Event callback
        private void OnEnable()
        {

            Selection.selectionChanged += OnSelectionChange;

            // Change the style of the Node here

            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            nodeStyle.normal.textColor = Color.white;
            nodeStyle.padding = new RectOffset(20, 20, 20, 20);
            nodeStyle.border = new RectOffset(12, 12, 12, 12);
            
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
                ProcessEvents();
                foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    DrawConnections(node);
                }
                foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    DrawNode(node);
                }
            }
        }

        private void ProcessEvents()
        {
           if(Event.current.type == EventType.MouseDown && draggingNode == null)
            {
                draggingNode = GetNodeAtPoint(Event.current.mousePosition);
                if (draggingNode != null)
                    draggingOffset = draggingNode.rect.position - Event.current.mousePosition;
            }
            else if (Event.current.type == EventType.MouseDrag && draggingNode != null)
            {
                Undo.RecordObject(selectedDialogue, "Move Dialogue Position");
                draggingNode.rect.position = Event.current.mousePosition + draggingOffset;
                
                GUI.changed = true;
            }
            else if(Event.current.type == EventType.MouseUp && draggingNode != null)
            {
                draggingNode = null;

            } 

        }

        private DialogueNode GetNodeAtPoint(Vector2 point)
        {

            DialogueNode foundNode = null;

            foreach (DialogueNode node in selectedDialogue.GetAllNodes())
            {
                if (node.rect.Contains(point))
                {
                    foundNode = node;
                }
 
            }

            return foundNode;
        }

        private void DrawNode(DialogueNode node)
        {
            GUILayout.BeginArea(node.rect, nodeStyle);
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Node:", EditorStyles.whiteLabel);
            string newID = EditorGUILayout.TextField(node.uniqueID);
            string newText = EditorGUILayout.TextField(node.text);

            // save/ mark as dirty, updates/ changes the scriptable object

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(selectedDialogue, "Update Dialogue Text");
                node.uniqueID = newID;
                node.text = newText;
            }

            foreach (DialogueNode childNode in selectedDialogue.GetAllChildren(node))
            {

                EditorGUILayout.LabelField(childNode.text);
            }

            GUILayout.EndArea();
        }


        private void DrawConnections(DialogueNode node)
        {
                Vector3 startPosition = new Vector2(node.rect.xMax, node.rect.center.y);
            foreach (DialogueNode childNode in selectedDialogue.GetAllChildren(node))
            {
                Vector3 endPosition = new Vector2(childNode.rect.xMin, childNode.rect.center.y);
                Vector3 controlPointOffset = endPosition - startPosition;
                controlPointOffset.y = 0;
                controlPointOffset.x *= 0.8f;
                Handles.DrawBezier(
                    startPosition, endPosition, 
                    startPosition + controlPointOffset, 
                    endPosition - controlPointOffset, 
                    Color.white, null,  10f);

            }
        }


    }
}
