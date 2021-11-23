using UnityEngine;

namespace Assets.Scripts
{
    [CreateAssetMenu(fileName = "new Dialogue", menuName = "Dialogue", order = 0)]
    public class Dialogue : ScriptableObject
    {
        [SerializeField] private DialogueNode[] nodes;

    }
}