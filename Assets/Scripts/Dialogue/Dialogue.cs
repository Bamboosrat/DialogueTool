using System.Collections.Generic;
using UnityEngine;
using System;

namespace DialogueTool
{
    [CreateAssetMenu(fileName = "new Dialogue", menuName = "Dialogue", order = 0)]
    public class Dialogue : ScriptableObject
    {
        [SerializeField]
        List<DialogueNode> nodes = new List<DialogueNode>();

        Dictionary<string, DialogueNode> nodeLookUp = new Dictionary<string, DialogueNode>();

        #if UNITY_EDITOR
        // Preprocessor, runs only in editor
        // runs before any C# code
        private void Awake()
        {
            Debug.Log("Awake from " + name);
            if(nodes.Count == 0)
            {
                DialogueNode rootNode = new DialogueNode();
                rootNode.uniqueID = Guid.NewGuid().ToString();
                nodes.Add(rootNode);
            }

        }
#endif
        // is called when an object is changed in the inspector
        private void OnValidate()
        {
            nodeLookUp.Clear();
            foreach (DialogueNode node in GetAllNodes())
            {
                nodeLookUp[node.uniqueID] = node;
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
            DialogueNode newNode = new DialogueNode();
            newNode.uniqueID = Guid.NewGuid().ToString();
            parent.children.Add(newNode.uniqueID);
            nodes.Add(newNode);
            OnValidate();
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            nodes.Remove(nodeToDelete);
            OnValidate();
            CleanDanglingChildren(nodeToDelete);
        }

        private void CleanDanglingChildren(DialogueNode nodeToDelete)
        {
            foreach (DialogueNode node in GetAllNodes())
            {
                node.children.Remove(nodeToDelete.uniqueID);
               
            }
        }
    }
}