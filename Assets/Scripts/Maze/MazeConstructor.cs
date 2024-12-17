using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class MazeConstructor : MonoBehaviour
{
    //1
    public bool showDebug;
    [Range(0, 1)]
    public float placementTreshold = .68f;

    public GameObject agentGO; 
    [SerializeField] private Material floorMat;
    [SerializeField] private Material wallMat;
    [SerializeField] private Material roofMat;
    [SerializeField] private PhysicsMaterial wallPhysicsMat;

    [SerializeField] private GameObject playerControler;

    private MazeDataGenerator dataGenerator;
    private MazeMeshGenerator meshGenerator;

    public int xSize = 13;
    public int ySize = 15;
    public float placementThreshold = .68f;    // chance of empty space
    public float hallWidth = 3.75f;
    public float hallHeight = 3.5f;

    public GameObject targetObject;
    public int score = 0;
    public int maxTargets = 4;
    public List<GameObject> targetGOList = new List<GameObject>();
    private GameObject mazeGo;
    public bool allTargetsReached = false;

    public Vector3 transformPos;
    private GameObject agentObject;

    public int[,] data
    {
        get; private set;
    }

    void Awake()
    {
        // default to walls surrounding a single empty cell
        data = new int[,]
        {
            {1, 1, 1},
            {1, 0, 1},
            {1, 1, 1}
        };

        dataGenerator = new MazeDataGenerator();
        meshGenerator = new MazeMeshGenerator();
        transformPos = transform.position;

        GenerateNewMaze(xSize, ySize, placementThreshold);
        setupWorld();
    }

    void Start()
    {

    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    Object.Destroy(agentObject);
        //    foreach (GameObject i in targetGOList)
        //    {
        //        Object.Destroy(i);
        //    }

        //    setupWorld();
        //}

        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    Object.Destroy(agentObject);
        //    foreach (GameObject i in targetGOList)
        //    {
        //        Object.Destroy(i);
        //    }
        //    Object.Destroy(mazeGo);
        //    GenerateNewMaze(xSize, ySize, placementThreshold);
        //    setupWorld();
        //}
    }

    public void RespawnPlayer()
    {
        Object.Destroy(agentObject);
        instantiatePlayer();
    }

    public void ReGenerateMaze()
    {
        Object.Destroy(agentObject);
        foreach (GameObject i in targetGOList)
        {
            Object.Destroy(i);
        }
        Object.Destroy(mazeGo);
        GenerateNewMaze(xSize, ySize, placementThreshold);
        setupWorld();
        this.GetComponent<Projection>().SetGhostMaze(mazeGo);
    }

    public void GenerateNewMaze(int sizeRows, int sizeCols, float plTresh)
    {
        if (sizeRows % 2 == 0 && sizeCols % 2 == 0)
        {
            Debug.LogError("Odd numbers work better for dungeon size.");
        }

        data = dataGenerator.FromDimensions(sizeRows, sizeCols, plTresh);

        DisplayMaze();
    }

    private void DisplayMaze()
    {
        mazeGo = new GameObject();
        mazeGo.transform.position = Vector3.zero;
        mazeGo.transform.parent = gameObject.transform;
        mazeGo.name = "Procedural Maze";
        mazeGo.tag = "Generated";


        MeshFilter mf = mazeGo.AddComponent<MeshFilter>();
        mf.mesh = meshGenerator.FromData(data, hallWidth, hallHeight, transformPos);

        MeshCollider mc = mazeGo.AddComponent<MeshCollider>();
        mc.sharedMesh = mf.mesh;
        mc.material = wallPhysicsMat;

        MeshRenderer mr = mazeGo.AddComponent<MeshRenderer>();
        mr.materials = new Material[3] { floorMat, wallMat, roofMat };
    }


    void OnGUI()
    {
        //1
        if (!showDebug)
        {
            return;
        }
        //2
        int[,] maze = data;
        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);
        string msg = "";
        //3
        for (int i = rMax; i >= 0; i--)
        {
            for (int j = 0; j <= cMax; j++)
            {
                if (maze[i, j] == 0)
                {
                    msg += "0";
                }
                else
                {
                    msg += "1";
                }
            }
            msg += "\n";
        }
        //4
        GUI.Label(new Rect(20, 20, 500, 500), msg);
    }

    public Vector3 randLoc
    {
        get; private set;
    }

    public void setupWorld()
    {
        SpawnTargets();
        //place player
        FindRandomPosition();
        //this.transform.position = new Vector3(mazeConstructor.randLoc.x * mazeConstructor.hallWidth, this.transform.localScale.y/2, mazeConstructor.randLoc.z * mazeConstructor.hallWidth);
        instantiatePlayer();
        //rollerAgentScript = agentObject.GetComponent<RollerAgent>();
    }

    public void instantiatePlayer()
    {
        agentObject = Instantiate(agentGO, new Vector3(transformPos.x + randLoc.x * hallWidth, transformPos.y + agentGO.transform.localScale.y / 2, transformPos.z + randLoc.z * hallWidth), Quaternion.identity);
        agentObject.transform.parent = gameObject.transform;
        playerControler.GetComponent<BallController>().setPlayer(agentObject);
        this.GetComponent<Projection>().actualPlayer = agentObject;
    }

    public void DisposeOldMaze()
    {
        //this will cause problems down the line???/
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Generated");
        foreach (GameObject go in objects)
        {
            Destroy(go);
        }
    }

    public void FindRandomPosition()
    {
        int[,] maze = data;
        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);
        int freeSpace = 0;

        //get count of free spaces
        for (int i = rMax; i >= 0; i--)
        {
            for (int j = 0; j <= cMax; j++)
            {
                if (maze[i, j] == 0)
                {
                    freeSpace++;
                }
            }
        }

        bool notValidLoc = true;
        int xPos = 0;
        int yPos = 0;

        if(freeSpace>0)
        {
            while (notValidLoc)
            {
                xPos = Random.Range(0, cMax);
                yPos = Random.Range(0, rMax);
                if (maze[yPos, xPos] == 0)
                {
                    Vector3 v3 = new Vector3(xPos, 0, yPos);
                    randLoc = v3;
                    //set poition taken in the maze data array, to avoid overlap on next spawns
                    // ToDo: Fix this
                    // data[yPos, xPos] = 1;
                    notValidLoc = false;
                    return;
                }
            }
        }
        else
        {
            Debug.Log("Theres no free space to spawn");
        }

    }

    public bool checkFreeSpaces()
    {
        int[,] maze = data;
        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);
        int freeSpace = 0;
        //get count of free spaces
        for (int i = rMax; i >= 0; i--)
        {
            for (int j = 0; j <= cMax; j++)
            {
                if (maze[i, j] == 0)
                {
                    freeSpace++;
                }
            }
        }
        if (freeSpace > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public Vector3 GetRandVector()
    {
        bool notValidLoc = true;
        int xPos = 0;
        int yPos = 0;
        Vector3 randV3 = new Vector3();
        int[,] maze = data;
        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);

        while (notValidLoc)
        {
            xPos = Random.Range(0, cMax);
            yPos = Random.Range(0, rMax);
            if (maze[yPos, xPos] == 0)
            {
                randV3 = new Vector3(xPos, 0, yPos);
                //set poition taken in the maze data array, to avoid overlap on next spawns
                //data[yPos, xPos] = 1;
                notValidLoc = false;
            }
        }
        return randV3;
    }

    public void SpawnTargets()
    {
        Vector3 targetLoc = new Vector3();
        int i = 0;
        while (i < maxTargets)
        {
            targetLoc = GetRandVector();
            GameObject targetGO = Instantiate(targetObject, new Vector3(transformPos.x + targetLoc.x * hallWidth, transformPos.y + targetObject.transform.localScale.y / 2, transformPos.z + targetLoc.z * hallWidth), Quaternion.identity);
            //add gameobject to an array
            targetGOList.Add(targetGO);
            targetGO.transform.parent = gameObject.transform;
            TargetObject targetObjectScript = targetGO.GetComponent<TargetObject>();
            targetObjectScript.xPos = (int)targetLoc.x;
            targetObjectScript.zPos = (int)targetLoc.z;
            i++;
        }
    }


}

