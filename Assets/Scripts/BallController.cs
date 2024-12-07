using UnityEngine;

public class BallController : MonoBehaviour
{
    public float forceAmount = 10f; // Adjust force amount in the Inspector
    private Rigidbody rb;

    private Vector3 mousePressDownPos;
    private Vector3 mouseReleasePos;

    [SerializeField]
    private bool canShootBool = true;
    public float velocityThreshold = 1.0f;
    private float stationaryTime = 0f;
    public float stopDelay = 0.5f;

    public Material canShootMaterial;
    public Material waitMaterial;

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Reference to the Rigidbody
        GetComponent<MeshRenderer>().material = canShootMaterial;
    }

    private void OnMouseDown()
    {
        mousePressDownPos = Input.mousePosition;
    }

    private void OnMouseUp()
    {
        mouseReleasePos = Input.mousePosition;
        Shoot(mouseReleasePos - mousePressDownPos);
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
            Debug.Log("changing material to wait");
            stationaryTime = 0f;
            GetComponent<MeshRenderer>().sharedMaterial = waitMaterial;
            Debug.Log("setting canShootBool to false");
            canShootBool = false;
        }
        else
        {
            Debug.Log("changing material to shoot");
            GetComponent<MeshRenderer>().material = canShootMaterial;
            canShootBool = true;
        }
    }

    void FixedUpdate()
    {
        if (!canShootBool)
        {
            Debug.Log(rb.linearVelocity.magnitude);
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