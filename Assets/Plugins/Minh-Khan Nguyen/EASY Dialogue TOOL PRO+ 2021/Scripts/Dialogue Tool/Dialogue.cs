using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace DialogueTool
{
    /// <summary>
    /// A Dialogue Scriptable Object.
    /// 
    /// In here you can find methods to create nodes and manipulate them.
    /// </summary>

    [CreateAssetMenu(fileName = "new Dialogue", menuName = "Dialogue", order = 0)]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {
        #region Fields / Variables
        [Tooltip("A list of all the childnodes.")]
        [SerializeField]
        private List<DialogueNode> nodes = new List<DialogueNode>();

        [Tooltip("Changes the start position of the newly created childnode.")]
        [SerializeField]
        private Vector2 newNodeOffset = new Vector2(250, 0);

        private Dictionary<string, DialogueNode> nodeLookUp = new Dictionary<string, DialogueNode>();
        #endregion

        #region Get nodes / child node

        // is called when an object is changed in the inspector
        private void OnValidate()
        {
            nodeLookUp.Clear();
            foreach (DialogueNode node in GetAllNodes())
            {
                nodeLookUp[node.name] = node;
            }
        }

        /// <summary>
        /// The GetAllNodes method takes no parameters and returns all nodes saved in a list.
        /// </summary>
        /// <returns> All DialogueNode items from a list. </returns>
        public IEnumerable<DialogueNode> GetAllNodes()
        {
            return nodes;
        }

        /// <summary>
        /// The GetRoodNode method takes no parameters and returns the first node saved in a list.
        /// </summary>
        /// <returns> The first DialogueNode item from a list. </returns>
        public DialogueNode GetRootNode()
        {
             return nodes[0];
        }

        /// <summary>
        /// The GetLastNode method takes no parameters and returns the last node saved in a list.
        /// </summary>
        /// <returns> The last DialogueNode item from a list. </returns>
        public DialogueNode GetLastNode()
        {
            return nodes[nodes.Count - 1];
        }

        /// <summary>
        /// The GetAllChildren method takes one parameter to compare all children ID with the current node.
        /// 
        /// It compares all children with the current node by looking it up and comparing the ID and DialogueNode type.
        /// </summary>
        /// <param name="currentNode"> The currently selected node to be compared with the child nodes. </param>
        /// <returns> All child nodes get returned, belonging to this node (excluding the selected node). </returns>
        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode currentNode)
        {
            foreach (string childID in currentNode.GetChildren())
            {
                if (nodeLookUp.ContainsKey(childID))
                {
                    yield return nodeLookUp[childID];
                }
            }
        }

        public IEnumerable<DialogueNode> GetPlayerChildren(DialogueNode currentNode)
        {
            foreach (DialogueNode node in GetAllChildren(currentNode))
            {
                if (node.IsPlayerSpeaking())
                {
                    yield return node;
                }
            }
        }


        public IEnumerable<DialogueNode> GetAIChildren(DialogueNode currentNode)
        {
            foreach (DialogueNode node in GetAllChildren(currentNode))
            {
                if (!node.IsPlayerSpeaking())
                {
                    yield return node;
                }
            }
        }

        #endregion

        #region Node methods

#if UNITY_EDITOR


        /// <summary>
        /// The CreateNode method creates a new node and attaches / childs the new node to a parent node.
        /// </summary>
        /// <param name="parent"> The parent node the child node gets attached to. If a node is selected beforehand. </param>
        /// <seealso cref="CreateNode(Vector2)"/>
        public void CreateNode(DialogueNode parent)
        {
            DialogueNode newNode = MakeNode(parent);
            Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue Node");
            Undo.RecordObject(this, "Added Dialogue Node");
            AddNode(newNode);
        }

        /// <summary>
        /// The CreateNode method creates a new node and sets it position to the offset.
        /// </summary>
        /// <param name="offset"> The offset is a Vector2 and defines the position the newly created node is placed. </param>
        /// <seealso cref="CreateNode(Vector2)"/>
        public void CreateNode(Vector2 offset)
        {
            DialogueNode newNode = MakeNode(offset);
            Undo.RegisterCreatedObjectUndo(newNode, "Created seperate Dialogue Node");
            Undo.RecordObject(this, "Added seperate Dialogue Node");
            AddNode(newNode);
        }
        private void AddNode(DialogueNode newNode)
        {
            nodes.Add(newNode);
            OnValidate();
        }

        /// <summary>
        /// The DeleteNode deletes a selected node.
        /// </summary>
        /// <param name="nodeToDelete"> The currently selected node. </param>
        public void DeleteNode(DialogueNode nodeToDelete)
        {
            Undo.RecordObject(this, "Removed Dialogue Node");
            nodes.Remove(nodeToDelete);
            OnValidate();
            CleanDanglingChildren(nodeToDelete);
            Undo.DestroyObjectImmediate(nodeToDelete);
        }

        // Overloading 1, implemented for right click => CreateNode Method
        private DialogueNode MakeNode(Vector2 offset)
        {
            DialogueNode newNode = CreateInstance<DialogueNode>();
            newNode.name = Guid.NewGuid().ToString();

            newNode.SetPosition(offset);

            return newNode;
        }

        // Overloading 2, implemented for createNode Button Method
        private DialogueNode MakeNode(DialogueNode parent)
        {
            DialogueNode newNode = CreateInstance<DialogueNode>();
            newNode.name = Guid.NewGuid().ToString();

            if (parent != null)
            {
                parent.AddChild(newNode.name);

                newNode.SetPlayerSpeaking(!parent.IsPlayerSpeaking());
                newNode.SetPosition(parent.GetRect().position + newNodeOffset);
            }

            return newNode;
        }

        // Deletes all connected child nodes, doesn't work tho lol
        private void CleanDanglingChildren(DialogueNode nodeToDelete)
        {
            foreach (DialogueNode node in GetAllNodes())
            {
                node.RemoveChild(nodeToDelete.name);
            }
        }
#endif
        #endregion

        #region ISerializationCallbackReceiver callbacks
        // Gets called before Awake
        // A node always gets created
        // Adds created nodes to the Dialogue Editor scriptable object
        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (nodes.Count == 0)
            {
                DialogueNode newNode = MakeNode(null);
                AddNode(newNode);
            }

            if ( AssetDatabase.GetAssetPath(this) != "")
            {
                foreach (DialogueNode node in GetAllNodes())
                {
                    if(AssetDatabase.GetAssetPath(node) == "")
                    {
                        AssetDatabase.AddObjectToAsset(node, this);
                    }
                }
            }
#endif
        }

        // Obsolete but necessary for inheriting ISerializationCallbackReceiver
        public void OnAfterDeserialize()
        {
            
        }

        #endregion
    }

}