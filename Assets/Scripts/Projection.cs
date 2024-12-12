using UnityEngine;
using UnityEngine.SceneManagement;

public class Projection : MonoBehaviour
{
    private Scene _simulationScene;
    private PhysicsScene _physicsScene;
    [SerializeField] private Transform _mazeParent;
    [SerializeField] private GameObject ghostPlayer;
    [SerializeField] private GameObject playerController;

    private void Start()
    {
        _mazeParent = this.transform;
        CreatePhysicsScene();
    }


    void CreatePhysicsScene()
    {
        _simulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
        _physicsScene = _simulationScene.GetPhysicsScene();

        foreach (Transform obj in _mazeParent)
        {
            if (obj.tag != "Target") {
                var ghostObj = Instantiate(obj.gameObject, obj.position, obj.rotation);
                ghostObj.GetComponent<Renderer>().enabled = false;
                SceneManager.MoveGameObjectToScene(ghostObj, _simulationScene);
                if (obj.tag == "Player")
                {
                    ghostPlayer = ghostObj;
                }
            }
        }
    }

    public void SimulateTrajectory(Vector2 startPos, Vector2 currentPos)
    {
        Debug.Log($"{startPos} {currentPos}");
        var _ballControler = playerController.GetComponent<BallController>();
        _ballControler.Shoot(ghostPlayer.GetComponent<Rigidbody>(), currentPos - startPos, true);


        // I would use the shoot script here from the BallController script.
        // I need to modify the Shoot method, for it to accept game object.

        // In addition, currently shoot is being called onTouchEnd. I need to trigger this by passing in current finger location.
        // I call this method from the Update of BallController. There I need to get the current finger.
    }
}
