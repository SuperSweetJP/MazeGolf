using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Projection : MonoBehaviour
{
    private Scene _simulationScene;
    private PhysicsScene _physicsScene;
    [SerializeField] private Transform _mazeParent;
    [SerializeField] private GameObject ghostPlayer;
    public GameObject actualPlayer;
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
                    ghostPlayer.transform.Find("ParticleSystem").gameObject.SetActive(false);
                    actualPlayer = obj.gameObject;
                }
            }
        }
    }

    [SerializeField] private LineRenderer _line;
    [SerializeField] private int _maxPhisicsFrames = 100;

    public void SimulateTrajectory(Vector2 startPos, Vector2 currentPos)
    {
        ghostPlayer.transform.position = actualPlayer.transform.position;
        ghostPlayer.GetComponent<Rigidbody>().linearVelocity = actualPlayer.GetComponent<Rigidbody>().linearVelocity;
        ghostPlayer.GetComponent<Rigidbody>().angularVelocity = actualPlayer.GetComponent<Rigidbody>().angularVelocity;

        var _ballControler = playerController.GetComponent<BallController>();
        _ballControler.Shoot(ghostPlayer.GetComponent<Rigidbody>(), currentPos - startPos, true);

        _line.positionCount = _maxPhisicsFrames;

        // set line start to player location
        _line.SetPosition(0, ghostPlayer.transform.position);
        for (int i = 1; i < _maxPhisicsFrames; i++)
        {
            _physicsScene.Simulate(Time.fixedDeltaTime);
            _line.SetPosition(i, ghostPlayer.transform.position);
        }
        //Destroy(ghostPlayer.gameObject);
    }
}
