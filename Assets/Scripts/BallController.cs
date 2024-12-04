using UnityEngine;

public class BallController : MonoBehaviour
{
    public float forceAmount = 10f; // Adjust force amount in the Inspector
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Reference to the Rigidbody
    }

    void Update()
    {
        // Get player input
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Calculate force direction
        Vector3 force = new Vector3(moveHorizontal, 0, moveVertical);

        // Apply force to the ball
        rb.AddForce(force * forceAmount);
    }
}