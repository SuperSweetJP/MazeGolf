using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;
using static UnityEngine.Timeline.DirectorControlPlayable;

public class BallController : MonoBehaviour
{
    public float forceAmount = 10f; // Adjust force amount in the Inspector
    private Rigidbody rb;
    private GameObject playerBall;

    private Vector3 mousePressDownPos;
    private Vector3 mouseReleasePos;
    // Projection
    [SerializeField] Projection projection;
    private bool projectTrajectory = false;
    private Vector2 touchPreviousFrame;
    private Vector3 previousBallposition;

    // Touch 
    private PlayerInput playerInput;
    private InputAction touchPressAction;
    private InputAction touchPositionAction;
    private Vector2 touchStartPosition;
    private Vector2 touchEndPosition;
    private InputAction keyPressAction;

    private Transform mazeConstructor;
    private MazeConstructor mazeConstructorScript;

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

    public void setPlayer(GameObject player)
    {
        playerBall = player;
        rb = player.GetComponent<Rigidbody>();
    }

    void Start()
    {
        mazeConstructor = GameObject.Find("MazeConstructor").transform;
        mazeConstructorScript = mazeConstructor.GetComponent<MazeConstructor>();
        projection = mazeConstructor.GetComponent<Projection>();

        foreach (Transform obj in mazeConstructor)
        {
            if (obj.tag == "Player")
            {
                playerBall = obj.gameObject;
                break;
            }
        }
        rb = playerBall.GetComponent<Rigidbody>(); // Reference to the Rigidbody
        playerBall.GetComponent<MeshRenderer>().material = canShootMaterial;
    }

    private void OnGotKeyPress(InputAction.CallbackContext context)
    {
        if (context.control.name == "r")
        {
            // regenerate maze
            mazeConstructorScript.ReGenerateMaze();
        }
    }

    private void OnTouchStart(InputAction.CallbackContext context)
    {
        touchStartPosition = touchPositionAction.ReadValue<Vector2>();
        // Start projection
        projectTrajectory = true;
    }

    private void OnTouchEnd(InputAction.CallbackContext context)
    {
        // end projection 
        projectTrajectory = false;
        touchEndPosition = touchPositionAction.ReadValue<Vector2>();
        Shoot(rb, touchEndPosition - touchStartPosition, false);
    }

    public void Shoot(Rigidbody rigidb, Vector3 Force, bool isProjection)
    {
        if (!isProjection)
        {
            Debug.Log($"actual {Force}");
            if (!canShootBool)
            {
                Debug.Log("Can't shoot");
                return;
            }
            canShootModify(false);
        }
        else
        {
            Debug.Log($"projection {Force}");
        }
        rigidb.AddForce(new Vector3(Force.x, Force.y, Force.y) * forceAmount);
    }

    void canShootModify(bool waitForShoot)
    {
        if (!waitForShoot)
        {
            stationaryTime = 0f;
            playerBall.GetComponent<MeshRenderer>().sharedMaterial = waitMaterial;
            canShootBool = false;
        }
        else
        {
            playerBall.GetComponent<MeshRenderer>().material = canShootMaterial;
            canShootBool = true;
        }
    }

    void Update()
    {
        // get current touch position
        // & previousBallposition == playerBall.transform.position
        if (projectTrajectory & touchPreviousFrame != touchPositionAction.ReadValue<Vector2>() )
        {
            touchPreviousFrame = touchPositionAction.ReadValue<Vector2>();
            projection.SimulateTrajectory(touchStartPosition, touchPreviousFrame);
        } else if (projectTrajectory & previousBallposition != playerBall.transform.position)
        {
            previousBallposition = playerBall.transform.position;
            projection.SimulateTrajectory(touchStartPosition, touchPreviousFrame);
        }

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