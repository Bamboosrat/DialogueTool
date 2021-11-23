using System.Collections.Generic;
using UnityEngine;

namespace DialogueTool
{
    [CreateAssetMenu(fileName = "new Dialogue", menuName = "Dialogue", order = 0)]
    public class Dialogue : ScriptableObject
    {
        [SerializeField]
        List<DialogueNode> nodes = new List<DialogueNode>();

        #if UNITY_EDITOR
        // Preprocessor, runs only in editor
        // runs before any C# code
        private void Awake()
        {
            Debug.Log("Awake from " + name);
            if(nodes.Count == 0)
            {
                nodes.Add(new DialogueNode());
            }

        }
        #endif

        // Flexible data container
        public IEnumerable<DialogueNode> GetAllNodes()
        {
            return nodes;
        }
    }
}