using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using static UnityEngine.Timeline.DirectorControlPlayable;

public class BallController : MonoBehaviour
{
    public float forceAmount = 10f; // Adjust force amount in the Inspector
    private Rigidbody rb;

    private Vector3 mousePressDownPos;
    private Vector3 mouseReleasePos;

    // Touch 
    private PlayerInput playerInput;
    private InputAction touchPressAction;
    private InputAction touchPositionAction;
    private Vector2 touchStartPosition;
    private Vector2 touchEndPosition;
    private InputAction keyPressAction;

    private MazeConstructor mazeConstructor;

    [SerializeField]
    private bool canShootBool = true;
    public float velocityThreshold = 1.0f;
    private float stationaryTime = 0f;
    public float stopDelay = 0.5f;

    public Material canShootMaterial;
    public Material waitMaterial;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        touchPressAction = playerInput.actions["TouchPress"];
        touchPositionAction = playerInput.actions["TouchPosition"];
        keyPressAction = playerInput.actions["KeyPress"];
    }

    private void OnEnable()
    {
        // subscribe to input action
        touchPressAction.performed += OnTouchStart;
        touchPressAction.canceled += OnTouchEnd;
        keyPressAction.performed += OnGotKeyPress;
    }

    private void OnDisable()
    {
        touchPressAction.performed -= OnTouchStart;
        touchPressAction.canceled -= OnTouchEnd;
        keyPressAction.performed -= OnGotKeyPress;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Reference to the Rigidbody
        GetComponent<MeshRenderer>().material = canShootMaterial;
        mazeConstructor = transform.root.gameObject.GetComponent<MazeConstructor>();
    }

    private void OnGotKeyPress(InputAction.CallbackContext context)
    {
        if (context.control.name == "r")
        {
            // regenerate maze
            mazeConstructor.ReGenerateMaze();
        }
    }

    private void OnTouchStart(InputAction.CallbackContext context)
    {
        touchStartPosition = touchPositionAction.ReadValue<Vector2>();
    }

    private void OnTouchEnd(InputAction.CallbackContext context)
    {
        touchEndPosition = touchPositionAction.ReadValue<Vector2>();
        Shoot(touchEndPosition - touchStartPosition);
    }

    void Shoot(Vector3 Force)
    {
        if (!canShootBool)
        {
            Debug.Log("Can't shoot");
            return;
        }

        canShootModify(false);
        rb.AddForce(new Vector3(Force.x, Force.y, Force.y) * forceAmount);
    }

    void canShootModify(bool waitForShoot)
    {
        if (!waitForShoot)
        {
            stationaryTime = 0f;
            GetComponent<MeshRenderer>().sharedMaterial = waitMaterial;
            canShootBool = false;
        }
        else
        {
            GetComponent<MeshRenderer>().material = canShootMaterial;
            canShootBool = true;
        }
    }

    void FixedUpdate()
    {
        if (!canShootBool)
        {
            if (rb.linearVelocity.magnitude <= velocityThreshold)
            {
                stationaryTime += Time.deltaTime;
                if (stationaryTime >= stopDelay)
                {
                    canShootModify(true);
                }
            }
        }
    }
}