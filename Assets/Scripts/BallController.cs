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
    public bool projectionEnabled = true;
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
    // Mouse
    private Vector2 mouseReleasePosition;

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
        Application.targetFrameRate = 60;
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

    public void setProjection()
    {
        if (projectionEnabled)
        {
            projectionEnabled = false;
        } else
        {
            projectionEnabled = true;
        }

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
        if (projectionEnabled)
        {
            mazeConstructor.GetComponent<LineRenderer>().enabled = true;
            projectTrajectory = true;
        }
        rect1.gameObject.SetActive(true);
        rect2.gameObject.SetActive(true);
    }

    // Canceled state is probably not the correct one to use here.
    private void OnTouchEnd(InputAction.CallbackContext context)
    {
        // end projection 
        if (projectionEnabled)
        {
            projectTrajectory = false;
            mazeConstructor.GetComponent<LineRenderer>().enabled = false;
        }
        rect1.gameObject.SetActive(false);
        rect2.gameObject.SetActive(false);
        //touchEndPosition = touchPositionAction.ReadValue<Vector2>();
        //Debug.Log(context.control.device);

        // Workaround to get position at mouse up, Input system probably needs to be used differently than this.
        if (context.control.device is Mouse)
        {
            touchEndPosition = mouseReleasePos;
        }
        else
        {
            touchEndPosition = touchPositionAction.ReadValue<Vector2>();
        }
        Shoot(rb, touchEndPosition - touchStartPosition, false);
    }

    public void Shoot(Rigidbody rigidb, Vector3 Force, bool isProjection)
    {
        if (!isProjection)
        {
            if (!canShootBool)
            {
                Debug.Log("Can't shoot");
                return;
            }
            canShootModify(false);
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

    void FixedUpdate()
    {
        // get current touch position
        // & previousBallposition == playerBall.transform.position
        if (projectionEnabled)
        {
            if (projectTrajectory & touchPreviousFrame != touchPositionAction.ReadValue<Vector2>())
            {
                touchPreviousFrame = touchPositionAction.ReadValue<Vector2>();
                projection.SimulateTrajectory(touchStartPosition, touchPreviousFrame);
            }
            else if (projectTrajectory & previousBallposition != playerBall.transform.position)
            {
                previousBallposition = playerBall.transform.position;
                projection.SimulateTrajectory(touchStartPosition, touchPreviousFrame);
            }
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

    [SerializeField] RectTransform rect1;
    [SerializeField] RectTransform rect2;

    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            mouseReleasePos = Mouse.current.position.ReadValue();
            Debug.Log(mouseReleasePos);
        }
        rect1.position = touchStartPosition;
        rect2.position = touchPositionAction.ReadValue<Vector2>();

    }
}