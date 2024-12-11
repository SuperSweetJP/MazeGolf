using UnityEngine;

public class BoundaryCheck : MonoBehaviour
{
    private MazeConstructor mazeConstructor;

    private void Start()
    {
        GameObject generatorObject = GameObject.Find("MazeConstructor");
        mazeConstructor = generatorObject.GetComponent<MazeConstructor>();
    }


    private void OnTriggerExit(Collider other)
    {
        // Check if the object leaving the boundary is the ball
        if (other.CompareTag("Player"))
        {
            Debug.Log("Ball is out of bounds!");
            mazeConstructor.RespawnPlayer();
        }
    }

    //private void RespawnBall(GameObject ball)
    //{
    //    // Example: Reset ball position to a start point
    //    ball.transform.position = new Vector3(0, 1, 0);
    //    Rigidbody rb = ball.GetComponent<Rigidbody>();
    //    if (rb != null)
    //    {
    //        rb.linearVelocity = Vector3.zero; // Stop any momentum
    //        rb.angularVelocity = Vector3.zero;
    //    }
    //}
}
