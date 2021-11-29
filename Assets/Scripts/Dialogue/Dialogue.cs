using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace DialogueTool
{
    [CreateAssetMenu(fileName = "new Dialogue", menuName = "Dialogue", order = 0)]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField]
        List<DialogueNode> nodes = new List<DialogueNode>();

        Dictionary<string, DialogueNode> nodeLookUp = new Dictionary<string, DialogueNode>();

        #if UNITY_EDITOR
        // Preprocessor, runs only in editor
        // runs before any C# code
        private void Awake()
        {
           // Debug.Log("Awake from " + name);
           

        }
#endif
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

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {
            // List<DialogueNode> result = new List<DialogueNode>();
            foreach (string childID in parentNode.children)
            {
                if (nodeLookUp.ContainsKey(childID))
                    yield return nodeLookUp[childID];

            }
        }

        public void CreateNode(DialogueNode parent)
        {
            DialogueNode newNode = CreateInstance<DialogueNode>();
            newNode.name = Guid.NewGuid().ToString();
            Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue Node");

            if (parent != null)
            {
                parent.children.Add(newNode.name);
            }

            nodes.Add(newNode);
            AssetDatabase.AddObjectToAsset(newNode, this);
            OnValidate();
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            nodes.Remove(nodeToDelete);
            OnValidate();
            CleanDanglingChildren(nodeToDelete);
            Undo.DestroyObjectImmediate(nodeToDelete);
        }

        private void CleanDanglingChildren(DialogueNode nodeToDelete)
        {
            foreach (DialogueNode node in GetAllNodes())
            {
                node.children.Remove(nodeToDelete.name);
               
            }
        }

        public void OnBeforeSerialize()
        {
            if (nodes.Count == 0)
            {
                CreateNode(null);
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
        }

        public void OnAfterDeserialize()
        {
            
        }
    }
}