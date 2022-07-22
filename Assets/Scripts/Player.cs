using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private const int JumpMultiplier = 6;
    private const float OverlapRadius = 0.1f;
    private const int DefaultCollisionBetweenPlayerAndChild = 1;

    [SerializeField] private Transform groundCheckTransform;
    [SerializeField] private LayerMask playerMask;

    private bool wasSpacePressed;
    private float horizontalInput;
    private Rigidbody rigidbodyComponent;

    // Start is called before the first frame update
    void Start()
    {
        wasSpacePressed = false;
        horizontalInput = 0;
        rigidbodyComponent = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // NOTE: Both Input Handlings can be true at the same time as it is possible to select "Both"
        //       under "Active Input Handling".

#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame == true)
        {
            wasSpacePressed = true;
            Debug.Log("Input System: Space was pressed.");
        }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        if (wasSpacePressed == false)
        {
            if (Input.GetKeyDown(KeyCode.Space) == true)
            {
                wasSpacePressed = true;
                Debug.Log("Input Manager: Space was pressed.");
            }
        }
        horizontalInput = Input.GetAxis("Horizontal");
#endif
    }

    private void FixedUpdate()
    {
        rigidbodyComponent.velocity = new Vector3(horizontalInput, rigidbodyComponent.velocity.y, 0);
        
        //if (Physics.OverlapSphere(groundCheckTransform.position, OverlapRadius).Length > DefaultCollisionBetweenPlayerAndChild)
        if (Physics.OverlapSphere(groundCheckTransform.position, OverlapRadius, playerMask).Length != 0)
        {
            if (wasSpacePressed)
            {
                rigidbodyComponent.AddForce(Vector3.up * JumpMultiplier, ForceMode.VelocityChange);
                wasSpacePressed = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7)
        {
            Destroy(other.gameObject);
        }
    }
}
