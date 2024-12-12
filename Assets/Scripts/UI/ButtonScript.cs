using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public MazeConstructor mazeConstructor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject generatorObject = GameObject.Find("MazeConstructor");
        mazeConstructor = generatorObject.GetComponent<MazeConstructor>();
    }

    public void OnButtonClick()
    {
        mazeConstructor.ReGenerateMaze();
    }
}
