using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetObject : MonoBehaviour
{
    //public Material doneMat;
    public float distLimit = 1.0f;
    private Renderer thisRenderer;
    private GameObject playerObj;
    // private bool playerCollider = false;
    private MazeConstructor mazeConstructor;
    private Vector3 newLocation = new Vector3();
    public int xPos;
    public int zPos;
    private Vector3 transformPos;
    private float hallWidth;
    private float hallHeight;
    //private RollerAgent agentScript;
    //private GameObject agentObject;

    private void Start()
    {
        mazeConstructor = transform.root.gameObject.GetComponent<MazeConstructor>();
        //agentObject = transform.root.Find("RollerAgnet(Clone)").gameObject;
        //agentScript = agentObject.GetComponent<RollerAgent>();
        hallWidth = mazeConstructor.hallWidth;
        hallHeight = mazeConstructor.hallHeight;
        transformPos = mazeConstructor.transformPos;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            // playerCollider = true;
            mazeConstructor.score += 1;
            //add reward
            //agentScript.injectReward = true;
            //get location and move object to new location
            if (mazeConstructor.checkFreeSpaces())
            {
                newLocation = mazeConstructor.GetRandVector();                
                //mazeConstructor.data[xPos, zPos] = 0;
                this.transform.position = new Vector3(transformPos.x + newLocation.x * hallWidth, transformPos.y + this.transform.localScale.y / 2, transformPos.z + newLocation.z * hallWidth);
                xPos = (int)newLocation.x;
                zPos = (int)newLocation.z;
            }
            else
            {
                Debug.Log("Target: No free space to move the target");
            }
        }
    }

    private void OnTriggerExit(Collider col)
    {
        //if (col.gameObject.tag == "Player")
        //{
        //    playerCollider = false;
        //}
    }
}
