using UnityEngine;

namespace DialogueTool
{

    public class Interactable : MonoBehaviour
    {
        [SerializeField] private bool _clickable;

        private void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                _clickable = true;
            }
        }

        private void OnTriggerExit(Collider collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                _clickable = false;
            }
        }

        public bool GetClickable()
        {
            return _clickable;
        }
    }
}
