using UnityEngine;
using DialogueTool;

namespace Test.Control
{
    [RequireComponent(typeof(PlayerMotor))]
    public class PlayerMovement : MonoBehaviour
    {
        public LayerMask movementMask;
        PlayerMotor motor;
        Camera cam;

        PlayerConversant playerConversant;


        // Start is called before the first frame update
        void Start()
        {
            cam = Camera.main;
            motor = GetComponent<PlayerMotor>();
            playerConversant = GetComponent<PlayerConversant>();
        }

        // Update is called once per frame
        void Update()
        {
            if (playerConversant.GetPlayerState() != PlayerState.Dialogue)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = cam.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out RaycastHit hit, 100, movementMask))
                    {
                        motor.MoveToPoint(hit.point);
                    }
                }
            }
            else
            {
                motor.MoveToPoint(transform.position);
            }


        }
    }
}
