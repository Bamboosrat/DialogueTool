#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;
using System.Collections.Generic;

// DialogueTool.Editor can use all scripts inside DialogueTool namespace but not vice versa
// Allowing for optimal nested usage
namespace DialogueTool.Editor
{

    public class DialogueEditor : EditorWindow
    {

        #region Properties / Field
        private Dialogue selectedDialogue = null;
        [NonSerialized]
        private DialogueNode draggingNode = null;
        [NonSerialized]
        private DialogueNode selectedNode = null;
        [NonSerialized]
        private DialogueNode creatingNode = null;
        [NonSerialized]
        private DialogueNode deletingNode = null;
        [NonSerialized]
        private DialogueNode linkingParentNode = null;

        [NonSerialized]
        private GUIStyle nodeStyle = new GUIStyle();
        [NonSerialized]
        private GUIStyle startNodeStyle = new GUIStyle();
        [NonSerialized]
        private GUIStyle endNodeStyle = new GUIStyle();
        [NonSerialized]
        private GUIStyle playerNodeStyle = new GUIStyle();
        [NonSerialized]
        private Vector2 nodeOffset;

        [NonSerialized]
        private Vector2 draggingOffset;
        [NonSerialized]
        private bool draggingCanvas = false;
        [NonSerialized]
        private Vector2 draggingCanvasOffset;

        private const float canvasSize = 4000f;
        private const float backgroundSize = 50f;

        private Vector2 scrollPosition;

        private static List<string> dialogueList = new List<string>();
        private int popUpIndex = 0;

        private enum NodeColor
        {
            // "node0" 
            Gray,
            // "node1"
            Blue,
            // "node2"
            Lime,
            // "node3" RESERVED for Start Node
            Green,
            // "node4"
            Yellow,
            // "node5"
            Orange,
            // "node6" RESERVED for End Node
            Red
            
        }

        #endregion

        #region On Editor Callbacks

        // static keyword or methods belongs to no specific instances but all DialogueEditor scripts
        [MenuItem("Dialogue Tool/Dialogue Editor")]
        public static void ShowEditorWindow()
        {
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

            ChangeNodeStyle(nodeStyle, NodeColor.Gray, Color.white);
            ChangeNodeStyle(playerNodeStyle, NodeColor.Blue, Color.white);
            ChangeNodeStyle(startNodeStyle, NodeColor.Green, Color.white);
            ChangeNodeStyle(endNodeStyle, NodeColor.Red, Color.white);

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

        #endregion

        #region OnGUI

        private void OnGUI()
        {
            if (selectedDialogue == null)
            {
                // This section is highly inefficient but it works :)
                dialogueList.Clear();
                foreach (Dialogue dialogue in FindAssetsByType<Dialogue>())
                {
                        dialogueList.Add(dialogue.ToString());
                }

                EditorGUILayout.LabelField("Load a Dialogue.");
                popUpIndex = EditorGUILayout.Popup("Select a Dialogue: ", popUpIndex, dialogueList.ToArray());

                if(FindAssetsByType<Dialogue>().Count >= popUpIndex  && GUILayout.Button("Load"))
                selectedDialogue = FindAssetsByType<Dialogue>().ToArray()[popUpIndex];
            }
            else
            {
                ProcessEvents();
                EditorGUILayout.LabelField(selectedDialogue.name);
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                #region Background of Dialogue Window

                Rect canvas = GUILayoutUtility.GetRect(canvasSize, canvasSize);
                Texture2D backgroundTexture = Resources.Load("background") as Texture2D;
                Rect texCoords = new Rect(0, 0, canvasSize / backgroundSize, canvasSize / backgroundSize);
                GUI.DrawTextureWithTexCoords(canvas, backgroundTexture, texCoords);

                #endregion

                if (creatingNode != null)
                {
                    selectedDialogue.CreateNode(creatingNode);
                    creatingNode = null;
                }

                if (deletingNode != null)
                {
                    selectedDialogue.DeleteNode(deletingNode);
                    deletingNode = null;
                }

                foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    DrawConnections(node);
                }
                foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    DrawNode(node);
                    
                }  

                EditorGUILayout.EndScrollView();
            }  
        }

        // Creates a List and finds all objects of type T
        public static List<T> FindAssetsByType<T>() where T : UnityEngine.Object
        {
            List<T> assets = new List<T>();
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }
            return assets;
        }

        // All Mouse Interaction

        private void ProcessEvents()
        {

            if (Event.current.button == 0)
            {
                if (Event.current.type == EventType.MouseDown && draggingNode == null)
                {
                    draggingNode = GetNodeAtPoint(Event.current.mousePosition + scrollPosition);
                    if (draggingNode != null)
                    {
                        draggingOffset = draggingNode.GetRect().position - Event.current.mousePosition;
                        Selection.activeObject = draggingNode;
                    }
                    else
                    {
                        draggingCanvas = true;
                        draggingCanvasOffset = Event.current.mousePosition + scrollPosition;
                        Selection.activeObject = selectedDialogue;
                    }
                }
                else if (Event.current.type == EventType.MouseDrag && draggingNode != null)
                {

                    draggingNode.SetPosition(Event.current.mousePosition + draggingOffset);

                    GUI.changed = true;
                }
                else if (Event.current.type == EventType.MouseDrag && draggingCanvas)
                {
                    scrollPosition = draggingCanvasOffset - Event.current.mousePosition;

                    GUI.changed = true;

                }
                else if (Event.current.type == EventType.MouseUp && draggingNode != null)
                {
                    draggingNode = null;

                }
                else if (Event.current.type == EventType.MouseUp && draggingCanvas)
                {
                    draggingCanvas = false;
                }
            }
            
            
            if (Event.current.type == EventType.ContextClick && Event.current.button == 1)
            {
                GenericMenu menu = new GenericMenu();
                selectedNode = GetNodeAtPoint(Event.current.mousePosition + scrollPosition);
                nodeOffset = GUIUtility.ScreenToGUIPoint(Event.current.mousePosition);

                    if (selectedNode != null)
                    {
                        menu.AddDisabledItem(new GUIContent($"ID: {selectedNode.name}"));
                        menu.AddItem(new GUIContent("Connect Node"), false, LinkNode);
                        menu.AddItem(new GUIContent("Create Child"), false, CreateChildNode);
                        menu.AddItem(new GUIContent("Delete Node"), false, DeleteNode);
                    }
                    else
                    {
                        menu.AddItem(new GUIContent("Create Node"), false, CreateNode);
                        menu.AddItem(new GUIContent("Load Dialogue"), false, LoadDialogue);
                    }
                
                
                menu.ShowAsContext();

                Event.current.Use();
            } 
            

            if (Event.current.commandName == "UndoRedoPerformed")
            {
                GUI.changed = true;
            }
        }

        private void CreateNode()
        {
            selectedDialogue.CreateNode(nodeOffset);
            nodeOffset = Vector2.zero;
        }

        private void CreateChildNode()
        {
            creatingNode = selectedNode;
            selectedNode = null;
        }

        private void DeleteNode()
        {
            deletingNode = selectedNode;
        }
        
        private void LinkNode()
        {
            linkingParentNode = selectedNode;
        }

        private void LoadDialogue()
        {
            selectedDialogue = null;
        }


        private DialogueNode GetNodeAtPoint(Vector2 point)
        {
            DialogueNode foundNode = null;

            foreach (DialogueNode node in selectedDialogue.GetAllNodes())
            {
                if (node.GetRect().Contains(point))
                {
                    foundNode = node;
                }
 
            }

            return foundNode;
        }

        #endregion

        #region Node Methods

        private void DrawNode(DialogueNode node)
        {
            GUIStyle style;

            if (node.IsPlayerSpeaking())
            {
                style = playerNodeStyle;
            }
            else if (selectedDialogue.GetRootNode() == node)
            {
                style = startNodeStyle;
            } 
            else if (node.GetChildren().Count == 0)
            {
                style = endNodeStyle;
            }
            else
            {
                 style = nodeStyle;
            }

            
            GUILayout.BeginArea(node.GetRect(), style);
            EditorGUI.BeginChangeCheck();

            node.SetText(EditorGUILayout.TextField(node.GetText(), GUILayout.Height(node.GetTextBoxSize())));
           

            GUILayout.BeginHorizontal();

            Color defaultGUIColor = GUI.backgroundColor;

            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Delete"))
            {
                deletingNode = node;

            }
            GUI.backgroundColor = defaultGUIColor;
            DrawLinkButtons(node);

            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Create"))
            {
                creatingNode = node;

            }
            GUI.backgroundColor = defaultGUIColor;
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private void ChangeNodeStyle(GUIStyle nodeStyleToChange, NodeColor nodeColor, Color textColor)
        {
            GUIStyle tempStyle = nodeStyleToChange;
            // giving enum a string value is complicated but look at this cool work around, check NodeColor for the node colors
            tempStyle.normal.background = EditorGUIUtility.Load($"node{(int)nodeColor}") as Texture2D;
            tempStyle.normal.textColor = textColor;
            tempStyle.padding = new RectOffset(20, 20, 20, 20);
            tempStyle.border = new RectOffset(12, 12, 12, 12);
        }

        private void DrawLinkButtons(DialogueNode node)
        {
            if (linkingParentNode == null)
            {
                if (GUILayout.Button("Link"))
                {
                    linkingParentNode = node;
                }
            }
            else if(linkingParentNode == node)
            {
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("Cancel"))
                {
                    linkingParentNode = null;
                }
            }
            else if (linkingParentNode.GetChildren().Contains(node.name))
            {
                GUI.backgroundColor = Color.grey;
                if (GUILayout.Button("Unlink"))
                {
                    linkingParentNode.RemoveChild(node.name);
                    linkingParentNode = null;
                }
            }
            else
            {
                GUI.backgroundColor = Color.blue;
                if (GUILayout.Button("Child"))
                {
                    Undo.RecordObject(selectedDialogue, "Add Dialogue Link");
                    linkingParentNode.AddChild(node.name);
                    linkingParentNode = null;
                }
            }
        }

        // Here you can manipulate values to change the look of the connection lines
        private void DrawConnections(DialogueNode node)
        {
            Vector3 startPosition = new Vector2(node.GetRect().xMax, node.GetRect().center.y);
            foreach (DialogueNode childNode in selectedDialogue.GetAllChildren(node))
            {
                Vector3 endPosition = new Vector2(childNode.GetRect().xMin, childNode.GetRect().center.y);
                Vector3 controlPointOffset = endPosition - startPosition;
                controlPointOffset.y = 0;
                controlPointOffset.x *= 0.8f;
                Handles.DrawBezier(
                    startPosition, endPosition, 
                    startPosition + controlPointOffset, 
                    endPosition - controlPointOffset, 
                    Color.white, null,  5f);
            }
        }

        #endregion
    }
}
#endif
