using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace DialogueTool
{
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

        // Flexible data container
        public IEnumerable<DialogueNode> GetAllNodes()
        {
            return nodes;
        }

        public DialogueNode GetRootNode()
        {
             return nodes[0];
        }

        public DialogueNode GetLastNode()
        {
            return nodes[nodes.Count - 1];
        }

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



        public void CreateNode(DialogueNode parent)
        {
            DialogueNode newNode = MakeNode(parent);
            Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue Node");
            Undo.RecordObject(this, "Added Dialogue Node");
            AddNode(newNode);
        }

        public void CreateNode(Vector2 offset)
        {
            DialogueNode newNode = MakeNode(offset);
            Undo.RegisterCreatedObjectUndo(newNode, "Created seperate Dialogue Node");
            Undo.RecordObject(this, "Added seperate Dialogue Node");
            AddNode(newNode);
        }



        public void DeleteNode(DialogueNode nodeToDelete)
        {
            Undo.RecordObject(this, "Removed Dialogue Node");
            nodes.Remove(nodeToDelete);
            OnValidate();
            CleanDanglingChildren(nodeToDelete);
            Undo.DestroyObjectImmediate(nodeToDelete);
        }

        private void AddNode(DialogueNode newNode)
        {
            nodes.Add(newNode);
            OnValidate();
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